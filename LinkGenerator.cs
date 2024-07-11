using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkGenerator : MonoBehaviour
{
    GameObject protein;
    GameObject Links = new GameObject();
    int ID = 1;

    List<string[]> atoms;
    
    public LinkGenerator(GameObject protein, AtomGenerator atom)
    {
        this.protein = protein;
        Links.transform.name = "Links";
        Links.transform.parent = this.protein.transform;
        atoms = atom.getatominfo();
        Links.AddComponent<MeshFilter>();
        Links.AddComponent<MeshRenderer>();
    }

    //Create Connection if there is a covalent water bond
    public void CreateConnect(string line)
    {
        string[] attributes = line.Split(',');
        Vector3[] points = findIndex(attributes[1], attributes[2], attributes[3], attributes[4], attributes[5], attributes[6]);
        Vector3 zero = new Vector3(0, 0, 0);
        
        Vector3 v1 = points[0];
        Vector3 v2 = points[1];

        //Ensures only connections are made in relevant spots
        if (!(v1.Equals(zero) || v2.Equals(zero)))
        {
            GameObject connect = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            connect.transform.parent = Links.transform;
            connect.name = "Link-" + ID;
            Renderer renderer = connect.GetComponent<Renderer>();
            renderer.material.color = Color.blue;
            connect.transform.localScale = new Vector3(0.15f, Vector3.Distance(v1, v2) / 2, 0.15f);
            connect.transform.localPosition = new Vector3((points[0].x + points[1].x) / 2, (points[0].y + points[1].y) / 2, (points[0].z + points[1].z) / 2);
            connect.transform.LookAt(v2);
            connect.transform.Rotate(90.0f, 0f, 0f, Space.Self);
        }
        ID++;
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
                if (!foundFirst && atom[9] == atom1 && atom[2] == id1 && atom[3] == residue1)
                {
                    pair[0] = new Vector3(float.Parse(atom[4]), float.Parse(atom[5]), float.Parse(atom[6]));
                    foundFirst = true;
                }
                else if (!foundSecond && atom[9] == atom2 && atom[2] == id2 && atom[3] == residue2)
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

}
