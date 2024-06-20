using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public class AtomGenerator : MonoBehaviour
{
    GameObject protein;
    CSVWriter list;
    int ID = 1;
    List<string[]> atominfo;
    GameObject atoms = new GameObject();
   
  
    

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

        GameObject atom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        atom.name = "atom-" + ID.ToString();
        atom.transform.parent = atoms.transform;
        atom.transform.localPosition = new Vector3(atomX, atomY, atomZ);
        atom.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        AssignColor(atom, attributes[8]);


        ID++;
    }

   
    public List<string[]> getatominfo() { 
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
