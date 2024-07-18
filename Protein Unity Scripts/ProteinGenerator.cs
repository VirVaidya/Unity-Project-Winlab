using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UIElements;


/* General script to build the entire protein
 * Writes a CSV File to eliminate unnecessary data in PDB
 * Iterates through CSV in order to generate different structures
 * Combines similar meshes together for improved performance
 */
public class ProteinGenerator : MonoBehaviour
{
    //[SerializeField] string pdbID;
    string proteinFile, csvFile, csvPath;
    GameObject protein;

    AtomGenerator atoms;
    HelixGenerator helixs;
    ConnectGenerator connects;
    SheetGenerator sheets;
    PhosphateSequenceGenerator rnahelixs;
    LinkGenerator links;
    ElementColor ColorSheet;
    GameObject hitbox;
    GameObject Title;
    GameObject WaterMolecule;
    string name;

    GameObject instantiatedPrefab;
    string filetype;
    string csvlocation = $"Assets/Scripts/Attempt 3/CSV";
    string inputlocation = $"Assets/Input";
    // Start is called before the first frame update
    //public MonoBehaviour[] scriptsToAttach;

    private void Start()
    {
        ColorSheet = new ElementColor();
        ColorSheet.createmap();
        WaterMolecule = LoadPrefab("WaterMolecule");
    }

    void Update()
    {
        if (Title != null)
        {
            Title.transform.LookAt(Camera.main.transform);
            Title.transform.Rotate(0, 180, 0, Space.Self);
        }
    }
    public async void GenerateProtein(string pdbID)
    {

        if (protein != null)
        {
            Destroy(protein);
            Destroy(hitbox);
        }
        
        if (!File.Exists($"Assets/Scripts/Attempt 3/CSV/{pdbID}.csv"))
        {
            await FetchPDB.FetchAndSavePDBAsync(pdbID);
        }

        if (File.Exists($"Assets/Input/{pdbID}.pdb"))
        {
            proteinFile = $"Assets/Input/{pdbID}.pdb";
        }
        else if (File.Exists($"Assets/Input/{pdbID}.cif"))
        {
            proteinFile = $"Assets/Input/{pdbID}.cif";
        }
        else
        {
            Debug.LogError("COULD NOT LOCATE PDB or CIF in inputs");
        }

        csvFile = $"{pdbID}.csv";
        csvPath = "Assets/Scripts/Attempt 3/CSV";

        filetype = proteinFile.Split(".")[1]; 
        int framerate = 120;
        Application.targetFrameRate = framerate;
        protein = new GameObject();
        protein.transform.parent = null; //ensures it is in worldspace and not in the canvas
        protein.name = "Protein";
        hitbox = new GameObject("Protein Hitbox");
        BoxCollider boxCollider = hitbox.AddComponent<BoxCollider>();
        ReferenceFileWriter writer;
        if (proteinFile.Contains(".pdb"))
        {
            writer = new CSVWriter(proteinFile, csvFile, csvPath);
        } else
        {
            writer = new WriteCSV_CIF(proteinFile, csvFile, csvPath);
        }
        
        // Load a unique model for watermolecule "atoms"
        atoms = new AtomGenerator(protein, writer, WaterMolecule);
        helixs = new HelixGenerator(protein);
        connects = new ConnectGenerator(protein, atoms);
        sheets = new SheetGenerator(protein);

        writer.WriteCSV();
        rnahelixs = new PhosphateSequenceGenerator(protein, atoms);
        links = new LinkGenerator(protein, atoms);
        CreateStructure();
        rnahelixs.createHelix();


        protein.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);


        atoms.makemeshes();
        helixs.makemeshes();
        sheets.makemeshes();
        rnahelixs.makemeshes();
        connects.makemeshes();
        Bounds bounds = CalculateAccurateBounds(protein);

        boxCollider.size = bounds.size;
        boxCollider.center = bounds.center - hitbox.transform.position;

        protein.transform.SetParent(hitbox.transform, true);

        // Position the hitbox at the protein's position

        hitbox.transform.position = bounds.center;
        hitbox.transform.position = bounds.center;
        protein.AddComponent<VoiceCommandHandler>();
        hitbox.AddComponent<NearInteractionGrabbable>();
        hitbox.AddComponent<ObjectManipulator>();
        hitbox.transform.position = new Vector3(0, 0, 0);

        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraPosition = cameraTransform.position;
        hitbox.transform.position = cameraPosition;
        AddTitle(hitbox, name);
    }


    private void OnApplicationQuit()
    {
        DeleteAllFiles(csvlocation);
        DeleteAllFiles(inputlocation);
    }

    public static void DeleteAllFiles(string directoryPath)
    {
        try
        {

            if (Directory.Exists(directoryPath))
            {
                // Get all files in the directory
                string[] files = Directory.GetFiles(directoryPath);

                // Iterate through all files and delete them
                foreach (string file in files)
                {
                    File.Delete(file);
                    Console.WriteLine($"Deleted file: {file}");
                }

                Console.WriteLine("All files deleted successfully.");
            }
            else
            {
                Console.WriteLine($"The directory {directoryPath} does not exist.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while deleting files: {ex.Message}");
        }
    }
    public GameObject LoadPrefab(string prefabName)
    {
        // Load the prefab from the Resources folder
        GameObject prefab = Resources.Load(prefabName, typeof(GameObject)) as GameObject;

        if (prefab != null)
        {
            return prefab;
        }
        else
        {
            Debug.LogError($"Failed to load {prefabName} prefab. Ensure it is placed inside the Resources folder.");
            return null;
        }
    }

    // Call this function in your Start method


    Bounds CalculateAccurateBounds(GameObject obj)
    {
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0)
        {
            Debug.LogWarning("No mesh filters found in the object.");
            return new Bounds(obj.transform.position, Vector3.zero);
        }

        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        bool hasBounds = false;

        foreach (MeshFilter mf in meshFilters)
        {
            Mesh mesh = mf.sharedMesh;
            if (mesh == null) continue;

            Vector3[] vertices = mesh.vertices;
            foreach (Vector3 vertex in vertices)
            {
                Vector3 worldPos = mf.transform.TransformPoint(vertex);
                if (worldPos == Vector3.zero)
                {
                    continue; // Skip the vertex at (0,0,0)
                }
                if (!hasBounds)
                {
                    bounds = new Bounds(worldPos, Vector3.zero);
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(worldPos);
                }
            }
        }

        return bounds;
    }


    Bounds TransformBounds(Bounds bounds, Transform transform) //helper method
    {
        Vector3 center = transform.TransformPoint(bounds.center);
        Vector3 size = bounds.size;
        Vector3 scale = transform.lossyScale;
        size = Vector3.Scale(size, scale);
        return new Bounds(center, size);
    }

    void AddTitle(GameObject parent, string text)
    {

        Title = new GameObject("Protein Title");

        TextMeshPro titleText = Title.AddComponent<TextMeshPro>();
        titleText.text = text;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.enableAutoSizing = true;
        titleText.fontSizeMin = 1; // Minimum font size
        titleText.fontSizeMax = 10; // Maximum font size
        RectTransform rectTransform = Title.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(10, 2); 
        Title.transform.SetParent(parent.transform);
        BoxCollider boxCollider = parent.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Vector3 hitboxCenter = boxCollider.transform.TransformPoint(boxCollider.center);
            
            Title.transform.localScale = new Vector3(boxCollider.size.x * 0.04f, boxCollider.size.y * 0.04f, boxCollider.size.z * 0.04f);
            Title.transform.position = hitboxCenter + new Vector3(0, boxCollider.size.y / 2 + 0.05f, 0); 
        }
        else
        {
            Debug.LogWarning("No BoxCollider found on the parent object.");
            
            Title.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
            Title.transform.localPosition = new Vector3(0, 1, 0); 
        }
    }



    void CreateStructure()
    {
        using (StreamReader reader = new StreamReader(Path.Combine(csvPath, csvFile)))
        {
            string line;
            while ((line = reader.ReadLine()) != null) //Iterates through CSV, writes lines as needed
            {
                if (line.StartsWith("ATOM") || line.StartsWith("HETATM"))
                {
                    atoms.GenerateAtom(line);
                }
                else if (line.StartsWith("HELIX"))
                {
                    helixs.GenerateHelix(line, atoms);
                }
                else if (line.StartsWith("CONECT"))
                {
                    connects.GenerateConnect(line, atoms, filetype);
                }
                else if (line.StartsWith("SHEET"))
                {
                    sheets.GenerateSheet(line, atoms);
                }
                else if (line.StartsWith("TER"))
                {
                    atoms.skipTER(); //For accurate atom positioning
                }
                else if (line.StartsWith("LINK"))
                {
                    links.CreateConnect(line);
                }
                else if (line.StartsWith("TITLE"))
                {
                    name = line.Substring(6).TrimStart(' ');
                }
            }
        }
    }
}

