using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HelixGenerator : MonoBehaviour
{
    GameObject protein;
    int ID = 1;

    private float scale = 1;

    private static int numPoints = 110; // How many points are placed to create an overall curve
    private List<Vector3> controlPoints = new List<Vector3>();
    private List<Vector3> aminopoints = new List<Vector3>();
    private GameObject Helices;
    private List<string> chains = new List<string>();

    public HelixGenerator(GameObject protein)
    {
        this.protein = protein;
        Helices = new GameObject("Helices");
        Helices.transform.parent = this.protein.transform;
    }

    public void GenerateHelix(string line, AtomGenerator atoms)
    {
        string[] attributes = line.Split(','); // Extract information from csv
        int startresidue = int.Parse(attributes[1]);
        int endresidue = int.Parse(attributes[2]);
        string chainID = attributes[3].Trim();

        List<string[]> range = GetAlphaCarbons(startresidue, endresidue, chainID, atoms);
        controlPoints.Clear();

        for (int i = 0; i < range.Count(); i++)
        {
            float ax = float.Parse(range[i][4]) * scale;
            float ay = float.Parse(range[i][5]) * scale;
            float az = float.Parse(range[i][6]) * scale;
            Vector3 v = new Vector3(ax, ay, az);
            controlPoints.Add(v);
        }

        if (!chains.Contains(chainID))
        {
            GeneratePolypeptideChain(chainID, atoms);
            chains.Add(chainID);
        }
       

        // Ensure there are enough control points for at least one segment to create the spline
        if (controlPoints.Count >= 4)
        {
            CreateCatmullRomCurveHelix();
        }
        else
        {
            Debug.LogWarning("Not enough control points to create a spline curve.");
        }
    }

    public void GeneratePolypeptideChain(string chainID, AtomGenerator atoms)
    {
        List<string[]> aminorange = GetAlphaCarbonsForChain(chainID, atoms);
        aminopoints.Clear();

        for (int i = 0; i < aminorange.Count(); i++)
        {
            float ax = float.Parse(aminorange[i][4]) * scale;
            float ay = float.Parse(aminorange[i][5]) * scale;
            float az = float.Parse(aminorange[i][6]) * scale;
            Vector3 v = new Vector3(ax, ay, az);
            aminopoints.Add(v);
        }

        // Ensure there are enough control points for at least one segment to create the spline
        if (controlPoints.Count >= 4)
        {
            CreateCatmullRomCurveAminos();
        }
        else
        {
            Debug.LogWarning("Not enough control points to create a spline curve.");
        }
    }

    List<string[]> GetAlphaCarbonsForChain(string chainID, AtomGenerator atoms)
    {
        List<string[]> toReturn = new List<string[]>();
        List<string[]> allatoms = atoms.getatominfo();

        foreach (var atom in allatoms)
        {
            if (atom[2] == chainID && atom[9] == "CA")
            {
                toReturn.Add(atom);
            }
        }

        return toReturn;
    }

    void CreateConnection(GameObject parent, Vector3 v1, Vector3 v2)
    {
        GameObject connect = GameObject.CreatePrimitive(PrimitiveType.Cube);
        connect.transform.parent = parent.transform;
        Renderer renderer = connect.GetComponent<Renderer>();
        connect.transform.localScale = new Vector3(0.20f, Vector3.Distance(v1, v2), 1.5f);
        connect.transform.localPosition = new Vector3((v1.x + v2.x) / 2, (v1.y + v2.y) / 2, (v1.z + v2.z) / 2);
        connect.transform.LookAt(v2);
        connect.transform.Rotate(90.0f, 0f, 0f, Space.Self);
        renderer.material.color = Color.green;
    }
    void CreateConnectionAmino(GameObject parent, Vector3 v1, Vector3 v2)
    {
        GameObject connect = GameObject.CreatePrimitive(PrimitiveType.Cube);
        connect.transform.parent = parent.transform;
        Renderer renderer = connect.GetComponent<Renderer>();
        connect.transform.localScale = new Vector3(0.20f, Vector3.Distance(v1, v2), 0.2f);
        connect.transform.localPosition = new Vector3((v1.x + v2.x) / 2, (v1.y + v2.y) / 2, (v1.z + v2.z) / 2);
        connect.transform.LookAt(v2);
        connect.transform.Rotate(90.0f, 0f, 0f, Space.Self);
        renderer.material.color = Color.green;
    }

    public void makemeshes()
    {
        int childCount = Helices.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = Helices.transform.GetChild(i);
            AtomGenerator.CombineAllMeshes(child.gameObject);
        }
    }

    void CreateCatmullRomCurveHelix()
    {
        GameObject parenthelix = new GameObject("Helix " + ID);
        parenthelix.AddComponent<MeshFilter>();
        parenthelix.AddComponent<MeshRenderer>();
        parenthelix.GetComponent<Renderer>().material.color = Color.green;
        parenthelix.transform.parent = Helices.transform;

        Vector3[] positions = new Vector3[numPoints];
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
                    CreateConnection(parenthelix, positions[j - 1], positions[j]); // Generate shapes along curve
                }
            }
        }
    }

        void CreateCatmullRomCurveAminos()
        {
            GameObject parenthelix = new GameObject("Sequence " + ID);
            parenthelix.AddComponent<MeshFilter>();
            parenthelix.AddComponent<MeshRenderer>();
            parenthelix.GetComponent<Renderer>().material.color = Color.green;
            parenthelix.transform.parent = Helices.transform;

            Vector3[] positions = new Vector3[numPoints];
            for (int i = 1; i < aminopoints.Count - 2; i++)
            {
                Vector3 p0 = aminopoints[i - 1];
                Vector3 p1 = aminopoints[i];
                Vector3 p2 = aminopoints[i + 1];
                Vector3 p3 = aminopoints[i + 2];

                for (int j = 0; j < numPoints; j++)
                {
                    float t = j / (float)(numPoints - 1);
                    positions[j] = CatmullRom(p0, p1, p2, p3, t);
                    if (j > 0)
                    {
                        CreateConnectionAmino(parenthelix, positions[j - 1], positions[j]); // Generate shapes along curve
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
        while (int.Parse(allatoms[i][3]) <= end) // Looks through the list to find all atoms in between the start point and end point and start chain and end chain
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
                Debug.LogError("couldnt find sequence start :" + start + " " + chainId);
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
