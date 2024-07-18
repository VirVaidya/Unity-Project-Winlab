using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectGenerator : MonoBehaviour
{
    GameObject protein;
    int ID = 1;
    GameObject Connects = new GameObject();
    List<string[]> atoms;
    public ConnectGenerator(GameObject protein, AtomGenerator atom)
    {
        this.protein = protein;
        Connects.name = "Connects";
        Connects.transform.parent = this.protein.transform;
        Connects.AddComponent<MeshFilter>();
        Connects.AddComponent<MeshRenderer>();
        atoms = atom.getatominfo();
    }

    public void GenerateConnect(string line, AtomGenerator atom, string filetype)
    {
        string[] attributes = line.Split(',');
        if (filetype == "pdb")
        { 
            HashSet<string> mySet = new HashSet<string>();
            int i = 2;
            int atom1 = int.Parse(attributes[1]);
            //Creates a connection from the first atom listed to the rest of the atoms
            while (i < attributes.Length)
            {
                if (!mySet.Contains(attributes[i])) // Makes sure not to duplicate connections if one was already made
                {
                    CreateConnect(atom1, int.Parse(attributes[i]), atom);
                    mySet.Add(attributes[i]); // adds to set if all connections of that atom have been created
                }
                i++;
            }
            mySet.Add(attributes[1]); // add the atom after all its connections are done to set so that connections will not duplicate
        } else
        {
            Vector3[] points = findIndex(attributes[3], attributes[1], attributes[2], attributes[6], attributes[4], attributes[5].Trim());
            Vector3 v1 = points[0];
            Vector3 v2 = points[1];
            Vector3 zero = new Vector3(0, 0, 0);
            if (!(v1.Equals(zero) || v2.Equals(zero)))
            {
                GameObject connect = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                connect.transform.parent = Connects.transform;
                connect.name = "Connects-" + ID;
                Renderer renderer = connect.GetComponent<Renderer>();
                renderer.material.color = Color.white;
                connect.transform.localScale = new Vector3(0.15f, Vector3.Distance(v1, v2) / 2, 0.15f);
                connect.transform.localPosition = new Vector3((points[0].x + points[1].x) / 2, (points[0].y + points[1].y) / 2, (points[0].z + points[1].z) / 2);
                connect.transform.LookAt(v2);
                connect.transform.Rotate(90.0f, 0f, 0f, Space.Self);
            }
            ID++;
        }
    }

    Vector3[] findIndex(string atom1, string id1, string residue1, string atom2, string id2, string residue2)
    {
        Vector3[] pair = new Vector3[2];
        bool foundFirst = false;
        bool foundSecond = false;

        foreach (string[] atom in atoms)
        {
            //Does not create connection if first atom is labeled 'HOH'
            if (atom[1] != "HOH")
            {
                if (!foundFirst && atom[9] == atom1 && atom[2] == id1 && (atom[3] == residue1 || atom[11] == residue1))
                {
                    pair[0] = new Vector3(float.Parse(atom[4]), float.Parse(atom[5]), float.Parse(atom[6]));
                    foundFirst = true;
                }
                else if (!foundSecond && atom[9] == atom2 && atom[2] == id2 && (atom[3] == residue2 || atom[11] == residue2))
                {
                    pair[1] = new Vector3(float.Parse(atom[4]), float.Parse(atom[5]), float.Parse(atom[6]));
                    foundSecond = true;
                }
            }

            if (foundFirst && foundSecond) //Breaks if it found both atoms it needs for connection to save runtime
            {
                break;
            }
        }

        return pair;
    }

    public void makemeshes()
    {
       
            AtomGenerator.CombineAllMeshes(Connects);
            AtomGenerator.CombineAllMeshes(Connects);
    }
    
    void CreateConnect(int atom1, int atom2, AtomGenerator atom)
    {
     
        float a1x = float.Parse(atom.getatominfo()[findIndex(atom1, atom)][4]);
        float a1y = float.Parse(atom.getatominfo()[findIndex(atom1, atom)][5]);
        float a1z = float.Parse(atom.getatominfo()[findIndex(atom1, atom)][6]);
        Vector3 v1 = new Vector3(a1x, a1y, a1z);

        float a2x = float.Parse(atom.getatominfo()[findIndex(atom2, atom)][4]);
        float a2y = float.Parse(atom.getatominfo()[findIndex(atom2, atom)][5]);
        float a2z = float.Parse(atom.getatominfo()[findIndex(atom2, atom)][6]);
        Vector3 v2 = new Vector3(a2x, a2y, a2z);

        GameObject connect = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        connect.transform.parent = Connects.transform;
        connect.name = "connect-" + ID;
        connect.transform.localScale = new Vector3(0.1f, Vector3.Distance(v1,v2)/2, 0.1f);
        connect.transform.localPosition = new Vector3((a1x + a2x)/2, (a1y + a2y)/2, (a1z + a2z)/2);

        connect.transform.LookAt(v2);
        connect.transform.Rotate(90.0f, 0f, 0f, Space.Self);
        ID++;
        
    }

    //Locate the location of the target atom
    int findIndex(int atomNum, AtomGenerator atom)
    {
        List<string[]> atoms = atom.getatominfo(); //Gets atom info from CSV file in form of string array
        for(int i = 0; i < atoms.Count; i++)
        {
            if (int.Parse(atoms[i][10]) == atomNum) //Searches for the atom ID that should start the connection
            {
                return i;
            }
        }

        return -1; 
    }
}
