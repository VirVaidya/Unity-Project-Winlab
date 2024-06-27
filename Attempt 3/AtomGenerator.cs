using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;
using Unity.XR.CoreUtils;
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
    HashSet<string> atomnames = new HashSet<string>();
    
   

    


    public AtomGenerator(GameObject protein, CSVWriter list)
    {
        this.protein = protein;
        this.list = list;
        atoms.transform.parent = this.protein.transform;
        atoms.name = "Atoms";
        atominfo = list.GetAtomInfo();
        
    }

   public void skipTER()
    {
        ID++;
    }

    public void GenerateAtom(string line)
    {
        string[] attributes = line.Split(',');
        //atominfo.Add(attributes);
        //Debug.Log(attributes[2] + ", " + attributes[3] + ", " + attributes[4]);
        float atomX = float.Parse(attributes[4]);
        float atomY = float.Parse(attributes[5]);
        float atomZ = float.Parse(attributes[6]);
        string element = attributes[8];
        Vector3 position = new Vector3(atomX, atomY, atomZ);
        GameObject atom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        atom.name = "atom-" + ID.ToString();
        //atom.transform.parent = atoms.transform;
        
        AssignColor(atom, attributes[8]);
       
        if (!atomnames.Contains(element)) {
           GameObject atomparent = new GameObject(element);
           atomparent.AddComponent<MeshFilter>();
           atomparent.AddComponent<MeshRenderer>();
           AssignColor(atomparent, element); 
           atomparent.transform.parent = this.atoms.transform;
           atom.transform.parent = atomparent.transform;
           atomnames.Add(element);
        } else {
           atom.transform.parent = atoms.transform.Find(element);
        }

        
        
        atom.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        ElementColor.AssignScale(atom, attributes[8]);
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
    public void CombineAllMeshes(GameObject atomparent)
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
            //combine[i].transform = meshfilter[i].transform.localToWorldMatrix;
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

    public List<string[]> getatominfo()
    {
        return atominfo;
    }

    public void displayatominfo(int index)
    {
        string temp = "";
        for (int i = 0; i < atominfo[index].Length; i++)
        {
            temp += atominfo[index][i] + " ";
        }

        //Debug.Log(temp);
    }

    void AssignColor(GameObject atom, string element)
    {
        Renderer renderer = atom.GetComponent<Renderer>();
        renderer.material.color = ElementColor.AssignColor(element);
   
        //atom.tag = element;
    }
}
