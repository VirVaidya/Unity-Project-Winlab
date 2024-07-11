using System.Collections.Generic;
using UnityEngine;

public class SheetGenerator : MonoBehaviour
{
    GameObject protein;
    int ID = 1;

    private float scale = 1;

    private static int numPoints = 100; // How many points are placed to create an overall curve
    private Vector3[] positions = new Vector3[numPoints];
    private GameObject sheets;

    public SheetGenerator(GameObject protein)
    {
        this.protein = protein;
        sheets = new GameObject("Sheets");
        sheets.transform.parent = this.protein.transform;
    }

    public void GenerateSheet(string line, AtomGenerator atoms)
    {
        string[] attributes = line.Split(',');
        int startresidue = int.Parse(attributes[2]);
        int endresidue = int.Parse(attributes[4]);
        string chainID1 = attributes[1].Trim();
        string chainID2 = attributes[3].Trim();

        List<string[]> range = GetBetaCarbons(startresidue, endresidue, chainID1, chainID2, atoms);
        if (range.Count > 3)
        {
            List<Vector3> controlPoints = new List<Vector3>();
            for (int i = 0; i < range.Count; i++)
            {
                float ax = float.Parse(range[i][4]) * scale;
                float ay = float.Parse(range[i][5]) * scale;
                float az = float.Parse(range[i][6]) * scale;
                Vector3 v = new Vector3(ax, ay, az);
                controlPoints.Add(v);
            }

            CreateCatmullRomCurve(controlPoints);
        }
    }

    void CreateCatmullRomCurve(List<Vector3> controlPoints)
    {
        GameObject parentsheet = new GameObject("Sheet " + ID);
        parentsheet.AddComponent<MeshFilter>();
        parentsheet.AddComponent<MeshRenderer>();
        parentsheet.GetComponent<Renderer>().material.color = Color.gray;
        parentsheet.transform.parent = sheets.transform;

        for (int i = 1; i < controlPoints.Count - 2; i++)
        {
            Vector3 p0 = controlPoints[i - 1];
            Vector3 p1 = controlPoints[i];
            Vector3 p2 = controlPoints[i + 1];
            Vector3 p3 = controlPoints[i + 2];

            for (int j = 0; j < numPoints; j++)
            {
                float t = j / (float)(numPoints - 1);
                positions[j] = CatmullRom(p0, p1, p2, p3, t);
                if (j > 0)
                {
                    CreateConnection(parentsheet, positions[j - 1], positions[j]); // Generate shapes along curve
                }
            }
        }

        ID++;
    }

    // Catmull-Rom spline interpolation function
    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        Vector3 a = 0.5f * ((2f * p1) + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 + (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
        return a;
    }

    void CreateConnection(GameObject parent, Vector3 v1, Vector3 v2)
    {
        GameObject connect = GameObject.CreatePrimitive(PrimitiveType.Cube);
        connect.transform.parent = parent.transform;
        Renderer renderer = connect.GetComponent<Renderer>();
        connect.transform.localScale = new Vector3(0.25f, Vector3.Distance(v1, v2), 0.75f);
        connect.transform.localPosition = new Vector3((v1.x + v2.x) / 2, (v1.y + v2.y) / 2, (v1.z + v2.z) / 2);
        connect.transform.LookAt(v2);
        connect.transform.Rotate(90.0f, 0f, 0f, Space.Self);
        renderer.material.color = Color.gray;
    }

    public void makemeshes()
    {
        int childCount = sheets.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = sheets.transform.GetChild(i);
            AtomGenerator.CombineAllMeshes(child.gameObject);
        }
    }

    List<string[]> GetBetaCarbons(int start, int end, string chainID1, string chainID2, AtomGenerator atoms)
    {
        List<string[]> toReturn = new List<string[]>();
        List<string[]> atomsInRange = GetAtomsInRange(start, end, chainID1, chainID2, atoms);
        foreach (string[] atom in atomsInRange)
        {
            if (atom[9].Equals("CA")) // Actually Alpha Carbons
            {
                toReturn.Add(atom);
            }
        }

        return toReturn;
    }

    List<string[]> GetAtomsInRange(int start, int end, string chainId1, string chainId2, AtomGenerator atoms)
    {
        List<string[]> toReturn = new List<string[]>();
        List<string[]> allatoms = atoms.getatominfo();

        int i = FindStart(start, chainId1, atoms);
        while (int.Parse(allatoms[i][3]) <= end || allatoms[i][2] != chainId2) // Locates all atoms in between the start point and the end point of a certain chainID
        {
            if (allatoms[i][2] == chainId1)
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
        while (int.Parse(atominfo[i][3]) < start || atominfo[i][2] != chainId) // Iterates till it finds the starting atom on that chainID.
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
