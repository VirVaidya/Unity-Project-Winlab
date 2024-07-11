using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using Unity.Collections.LowLevel.Unsafe;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.WindowsMR.Input;

public class AtomGenerator : MonoBehaviour
{
    GameObject protein;
    CSVWriter list;
    int ID = 1;
    List<string[]> atominfo;
    GameObject atoms = new GameObject();
    Dictionary<string, List<GameObject>> elementAtoms = new Dictionary<string, List<GameObject>>();
    HashSet<string> atomnames = new HashSet<string>();
    

  
    GameObject waterPrefab;

    public AtomGenerator(GameObject protein, CSVWriter list, GameObject waterprefab)
    {
        this.protein = protein;
        this.list = list;
        atoms.transform.parent = this.protein.transform;
        atoms.name = "Atoms";
        atominfo = list.GetAtomInfo();
        this.waterPrefab = waterprefab;
    }

   
    public void skipTER()
    {
        ID++;
    }

    public void GenerateAtom(string line)
    {
        string[] attributes = line.Split(','); // Gets information from CSV for atoms
        float atomX = float.Parse(attributes[4]);
        float atomY = float.Parse(attributes[5]);
        float atomZ = float.Parse(attributes[6]);
        string element = attributes[8];
        Vector3 position = new Vector3(atomX, atomY, atomZ);
        GameObject atom;
        //Debug.Log(attributes[1] + " " + element);
        if ((element.Trim() == "O") && (attributes[1].Trim() == "HOH")) // Use the waterprefab for HOH molecules instead of placing "O" atoms
        {
            element = "HOH";
            atom = Instantiate(waterPrefab);
            atom.transform.localPosition = position;
        } else
        {
           atom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }

        atom.name = "atom-" + ID.ToString();

        if (!atomnames.Contains(element)) { // Checks and adds each new element to set so duplicate meshes are not created.
           GameObject atomparent = new GameObject(element);
           atomparent.AddComponent<MeshFilter>();
           atomparent.AddComponent<MeshRenderer>();
           AssignColor(atomparent, element);
           atomparent.transform.parent = this.atoms.transform; //Assign the new element as a child of the overall atoms object
           atom.transform.parent = atomparent.transform;
           atomnames.Add(element);
        } else {
           atom.transform.parent = atoms.transform.Find(element); // If element was already created, just assigns it as a child of that object
        }

        
        
        atom.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        ElementColor.AssignScale(atom, attributes[8]); //Resize atom based on element
        atom.transform.localPosition = new Vector3(atomX, atomY, atomZ);
        ID++;
    }

    public void makemeshes()
    {
        int childCount = atoms.transform.childCount;
        for (int i = 0; i < childCount; i++) { 
           Transform child = atoms.transform.GetChild(i);
           CombineAllMeshes(child.gameObject); 
        }
    }
    public static void CombineAllMeshes(GameObject atomparent) // Combines all similar objects into one overall object instead of many individual ones (Improve performance)
    {
        MeshFilter[] meshfilter = atomparent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshfilter.Length];
        Matrix4x4 parentTransform = atomparent.transform.localToWorldMatrix;
        Matrix4x4 parentInverse = parentTransform.inverse;
        int i = 0;
        Vector3 originalPosition = atomparent.transform.localPosition;
        Vector3 originalScale = atomparent.transform.localScale;


        while (i < meshfilter.Length)
        {
            combine[i].mesh = meshfilter[i].sharedMesh;
            combine[i].transform = parentInverse * meshfilter[i].transform.localToWorldMatrix;
            meshfilter[i].gameObject.SetActive(false);

          
            i++;
        }
        MeshFilter mf = atomparent.GetComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mf.mesh.CombineMeshes(combine);

      
        Renderer mr = atomparent.GetComponent<Renderer>();
        mr.material.color = meshfilter[0].gameObject.GetComponent<Renderer>().material.color;
        
        atomparent.transform.localPosition = originalPosition;
        atomparent.transform.localScale = originalScale;
        atomparent.SetActive(true);
    }

    public List<string[]> getatominfo() // Converts the csv information to string array
    {
        return atominfo;
    }

    public void displayatominfo(int index) //For Debugging Purposes
    {
        string temp = "";
        for (int i = 0; i < atominfo[index].Length; i++)
        {
            temp += atominfo[index][i] + " ";
        }
    }

    void AssignColor(GameObject atom, string element)
    {
        Renderer renderer = atom.GetComponent<Renderer>();
        renderer.material.color = ElementColor.AssignColor(element); //Assigns color based on atom element
        
    }
}
