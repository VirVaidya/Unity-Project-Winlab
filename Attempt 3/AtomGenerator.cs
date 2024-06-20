using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AtomGenerator : MonoBehaviour
{
    GameObject protein;
    CSVWriter list;
    int ID = 1;
    List<string[]> atominfo;
    GameObject atoms = new GameObject();
    Dictionary<string, List<GameObject>> elementAtoms = new Dictionary<string, List<GameObject>>();





    public AtomGenerator(GameObject protein, CSVWriter list)
    {
        this.protein = protein;
        this.list = list;
        atoms.transform.parent = this.protein.transform;
        atoms.name = "Atoms";
        atominfo = list.GetAtomInfo();
        
    }

    public void GenerateAtom(string line)
    {
        string[] attributes = line.Split(',');
        atominfo.Add(attributes);
        //Debug.Log(attributes[2] + ", " + attributes[3] + ", " + attributes[4]);
        float atomX = float.Parse(attributes[4]);
        float atomY = float.Parse(attributes[5]);
        float atomZ = float.Parse(attributes[6]);
        string element = attributes[8];
        Vector3 position = new Vector3(atomX, atomY, atomZ);
        GameObject atom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        atom.name = "atom-" + ID.ToString();
        atom.transform.parent = atoms.transform;
        atom.transform.localPosition = new Vector3(atomX, atomY, atomZ);
        atom.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        AssignColor(atom, attributes[8]);

        if (!elementAtoms.ContainsKey(element))
        {
            elementAtoms[element] = new List<GameObject>();
        }
        elementAtoms[element].Add(atom);
        ID++;
    }
    public void CombineMesh()
    {
        foreach (var element in elementAtoms.Keys)
        {
            List<GameObject> atoms = elementAtoms[element];
            CombineInstance[] combineInstances = new CombineInstance[atoms.Count];

            for (int i = 0; i < atoms.Count; i++)
            {
                MeshFilter meshFilter = atoms[i].GetComponent<MeshFilter>();
                combineInstances[i].mesh = meshFilter.sharedMesh;
                combineInstances[i].transform = atoms[i].transform.localToWorldMatrix;
                atoms[i].SetActive(false);
            }

            GameObject combinedObject = new GameObject(element + "Atoms");
            combinedObject.transform.parent = transform;
            combinedObject.transform.localPosition = Vector3.zero;

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combineInstances);

            MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
            combinedMeshFilter.mesh = combinedMesh;

            MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();
            combinedMeshRenderer.material = atoms[0].GetComponent<Renderer>().material;

            combinedObject.SetActive(true);
        }
    }
    Mesh CreateSphereMesh()
    {
        GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Mesh sphereMesh = tempSphere.GetComponent<MeshFilter>().mesh;
        Destroy(tempSphere);
        return sphereMesh;
    }

    public List<string[]> getatominfo()
    {
        return atominfo;
    }

    public void displayatominfo(int index)
    {
        string temp = "";
        for (int i = 0; i < atominfo.Count; i++)
        {
            temp += atominfo[index][i] + " ";
        }

        //Debug.Log(temp);
    }

    void AssignColor(GameObject atom, string element)
    {
        Renderer renderer = atom.GetComponent<Renderer>();
        renderer.material.color = ElementColor.AssignColor(element);
        atom.tag = element;
    }
}
