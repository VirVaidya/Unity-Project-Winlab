using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

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
    string pdbFile, csvFile, csvPath;
    GameObject protein;

    AtomGenerator atoms;
    HelixGenerator helixs;
    ConnectGenerator connects;
    SheetGenerator sheets;
    PhosphateSequenceGenerator rnahelixs;
    LinkGenerator links;
    ElementColor ColorSheet;
    GameObject hitbox;
    GameObject WaterMolecule;

    GameObject instantiatedPrefab;

    // Start is called before the first frame update
    //public MonoBehaviour[] scriptsToAttach;

    private void Start()
    {
        ColorSheet = new ElementColor();
        ColorSheet.createmap();
        WaterMolecule = LoadPrefab("WaterMolecule"); 
    }
    public async void GenerateProtein(string pdbID)
    {
        
        if (protein != null)
        {
            Destroy(protein);
            Destroy(hitbox);
        }
        
        await FetchPDB.FetchAndSavePDBAsync(pdbID);
        pdbFile = $"Assets/Input/{pdbID}.pdb";
        csvFile = $"{pdbID}.csv";
        csvPath = "Assets/Scripts/Attempt 3/CSV";

        
        int framerate = 120;
        Application.targetFrameRate = framerate;
        protein = new GameObject();
        protein.transform.parent = null; //ensures it is in worldspace and not in the canvas
        protein.name = "Protein";
        hitbox = new GameObject("Protein Hitbox");
        BoxCollider boxCollider = hitbox.AddComponent<BoxCollider>();
        CSVWriter writer = new CSVWriter(pdbFile, csvFile, csvPath);
        // Load a unique model for watermolecule "atoms"

        atoms = new AtomGenerator(protein, writer, WaterMolecule);
        helixs = new HelixGenerator(protein);
        connects = new ConnectGenerator(protein);
        sheets = new SheetGenerator(protein);
        writer.WriteCSV();
        rnahelixs = new PhosphateSequenceGenerator(protein, atoms);
        links = new LinkGenerator(protein, atoms);
        CreateStructure();
        rnahelixs.createHelix();


        protein.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);


        // Position the hitbox at the protein's position
       
       
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
                    connects.GenerateConnect(line, atoms);
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
            }
        }
    }
}
            
   