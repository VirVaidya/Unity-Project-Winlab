using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectGenerator : MonoBehaviour
{
    GameObject protein;
    int ID = 1;
    GameObject Connects = new GameObject();
   
    public ConnectGenerator(GameObject protein)
    {
        this.protein = protein;
        Connects.name = "Connects";
        Connects.transform.parent = this.protein.transform;
        Connects.AddComponent<MeshFilter>();
        Connects.AddComponent<MeshRenderer>();
    }

    public void GenerateConnect(string line, AtomGenerator atom)
    {
        string[] attributes = line.Split(',');
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
    }

    public void makemeshes()
    {
       
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
