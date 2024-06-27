using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.UIElements;

public class ProteinGenerator : MonoBehaviour
{
    [SerializeField] string pdbFile, csvFile, csvPath;
    GameObject protein;

    AtomGenerator atoms;
    HelixGenerator helixs;
    ConnectGenerator connects;
    SheetGenerator sheets;
    PhosphateSequenceGenerator rnahelixs;
    LinkGenerator links;
    ElementColor ColorSheet;

    
    MeshCombiner mc = new MeshCombiner();
    // Start is called before the first frame update
    public MonoBehaviour[] scriptsToAttach;
    void Start()
    {
        ColorSheet = new ElementColor();
        int framerate = 120;
        Application.targetFrameRate = framerate;
        protein = new GameObject();
        protein.name = "Protein";
        GameObject hitbox = new GameObject("Protein Hitbox");
        BoxCollider boxCollider = hitbox.AddComponent<BoxCollider>();
        CSVWriter writer = new CSVWriter(pdbFile, csvFile, csvPath);
        ColorSheet.createmap();
        

        atoms = new AtomGenerator(protein, writer);
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

    Bounds CalculateAccurateBounds(GameObject obj)
    {
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0)
        {
            return new Bounds(obj.transform.position, Vector3.zero);
        }

        Bounds bounds = meshFilters[0].mesh.bounds;
        Transform objTransform = meshFilters[0].transform;
        bounds = TransformBounds(bounds, objTransform);

        for (int i = 1; i < meshFilters.Length; i++)
        {
            Bounds newBounds = TransformBounds(meshFilters[i].mesh.bounds, meshFilters[i].transform);
            bounds.Encapsulate(newBounds);
        }

        return bounds;
    }

    Bounds TransformBounds(Bounds bounds, Transform transform)
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
            while ((line = reader.ReadLine()) != null)
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
                    atoms.skipTER();
                }
                else if (line.StartsWith("LINK"))
                {
                    links.CreateConnect(line);
                }
            }
        }
    }
}
            
   