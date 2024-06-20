using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;


public class HelixGenerator : MonoBehaviour
{
    GameObject protein;
    int ID = 1;

    private float scale = 1;

    private static int numPoints = 15;
    private Vector3[] positions = new Vector3[numPoints];
    GameObject Helices = new GameObject();

    public HelixGenerator(GameObject protein)
    {
        this.protein = protein;
        Helices.name = "Helices";
        Helices.transform.parent = this.protein.transform;
    }

    public void GenerateHelix(string line, AtomGenerator atoms)
    {
        string[] attributes = line.Split(',');
        int startresidue = int.Parse(attributes[1]);
        int endresidue = int.Parse(attributes[2]);
        string chainID = attributes[3];

        List<string[]> range = GetAlphaCarbons(startresidue, endresidue, chainID, atoms);
        for(int i=0; i<range.Count()-2; i += 2)
        {
            float a1x = float.Parse(range[i][4]) * scale;
            float a1y = float.Parse(range[i][5]) * scale;
            float a1z = float.Parse(range[i][6]) * scale;
            Vector3 v1 = new Vector3(a1x, a1y, a1z);

            float a2x = float.Parse(range[i+1][4]) * scale;
            float a2y = float.Parse(range[i+1][5]) * scale;
            float a2z = float.Parse(range[i+1][6]) * scale;
            Vector3 v2 = new Vector3(a2x, a2y, a2z);

            float a3x = float.Parse(range[i + 2][4]) * scale;
            float a3y = float.Parse(range[i + 2][5]) * scale;
            float a3z = float.Parse(range[i + 2][6]) * scale;
            Vector3 v3 = new Vector3(a3x, a3y, a3z);

            CreateCurve(v1, v2, v3);
        }
    }

    void CreateConnection(Vector3 v1, Vector3 v2)
    {
        GameObject connect = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        connect.transform.parent = Helices.transform;
        connect.name = "helix-" + ID;
        Renderer renderer = connect.GetComponent<Renderer>();
        connect.transform.localScale = new Vector3(1.5f, Vector3.Distance(v1, v2), 1.5f);
        connect.transform.localPosition = new Vector3((v1.x + v2.x) / 2, (v1.y + v2.y) / 2, (v1.z + v2.z) / 2);
        connect.transform.LookAt(v2);
        connect.transform.Rotate(90.0f, 0f, 0f, Space.Self);
        renderer.material.color = Color.green;
        ID++;
    }

    void CreateCurve(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        positions[0] = p0;
        for (int i = 1; i < numPoints; i++)
        {
            float t = i / (float)numPoints;
            positions[i] = CalculateCurve(p0, p1, p2, t);
            CreateConnection(positions[i - 1], positions[i]);
        }
        ID++;
    }

    Vector3 CalculateCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        
        return p;
    }

    List<string[]> GetAlphaCarbons(int start, int end, string chainID, AtomGenerator atoms)
    {
        List<string[]> toReturn = new List<string[]>();
        List<string[]> atomsInRange = GetAtomsInRange(start, end, chainID, atoms);
        foreach (string[] atom in atomsInRange)
        {
            if (atom[9].Equals("CA"))
            {
                toReturn.Add(atom);
            }
        }
        
        return toReturn;
    }
    
    List<string[]> GetAtomsInRange(int start, int end, string chainId, AtomGenerator atoms)
    {
        List<string[]> toReturn = new List<string[]>();
        List<string[]> allatoms = atoms.getatominfo();

        int i = FindStart(start, chainId, atoms);
        while (int.Parse(allatoms[i][3]) <= end)
        {
            if (allatoms[i][2] == chainId)
            {
                toReturn.Add(allatoms[i]);
            }
            i++;
        }

        return toReturn;
    }

    int FindStart(int start, string chainId, AtomGenerator atoms)
    {
        List<string[]> atominfo = atoms.getatominfo();

        int i = 0;
        while (int.Parse(atominfo[i][3]) < start || atominfo[i][2] != chainId)
        {
            if (i == atominfo.Count - 1)
            {
                return -1;
            }
            else
            {
                i++;
            }
        }

        return i;
    }
}
