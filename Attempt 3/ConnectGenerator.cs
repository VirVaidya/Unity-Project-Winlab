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
    }

    public void GenerateConnect(string line, AtomGenerator atom)
    {
        string[] attributes = line.Split(',');
        HashSet<string> mySet = new HashSet<string>();
        int i = 2;
        int atom1 = int.Parse(attributes[1]);
        //Debug.Log(attributes.Length);
        while (i < attributes.Length)
  
        {
            //Debug.Log(i);
            if (!mySet.Contains(attributes[i]))
            {
                CreateConnect(atom1, int.Parse(attributes[i]), atom);
                mySet.Add(attributes[i]);
            }
            i++;
        }
        
    }

    void CreateConnect(int atom1, int atom2, AtomGenerator atom)
    {
     
        //Debug.Log(atom.getatominfo()[atom1 - 1][4]);
        float a1x = float.Parse(atom.getatominfo()[atom1 - 1][4]);
        float a1y = float.Parse(atom.getatominfo()[atom1 - 1][5]);
        float a1z = float.Parse(atom.getatominfo()[atom1 - 1][6]);
        Vector3 v1 = new Vector3(a1x, a1y, a1z);

        float a2x = float.Parse(atom.getatominfo()[atom2 - 1][4]);
        float a2y = float.Parse(atom.getatominfo()[atom2 - 1][5]);
        float a2z = float.Parse(atom.getatominfo()[atom2 - 1][6]);
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
}
