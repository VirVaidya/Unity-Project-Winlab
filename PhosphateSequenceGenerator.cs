using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhosphateSequenceGenerator : MonoBehaviour
{
    GameObject protein;
    int ID = 1;
    private float scale = 1;
    private static int numPoints = 20;
    private Vector3[] positions = new Vector3[numPoints];
    GameObject RNAHelices = new GameObject();

    private List<string> sequences = new List<string>();
    private List<string[]> atoms;

    public PhosphateSequenceGenerator(GameObject protein, AtomGenerator atom)
    {
        this.protein = protein;
        RNAHelices.name = "RNA Helices";
        RNAHelices.transform.parent = this.protein.transform;
        atoms = atom.getatominfo();
    }

    void findSequences()
    {
        foreach (string[] atom in atoms)
        {
            if (!sequences.Contains(atom[2]))
            {
                sequences.Add(atom[2]);
            }
        }
    }

    public void checkatoms(AtomGenerator atom)
    {
        for (int i = 0; i < atoms.Count; i++)
        {
            atom.displayatominfo(i);
            Debug.Log("i = " + i);
        }
    }

    public void createHelix()
    {
        findSequences();
        foreach (string chainID in sequences)
        {
            List<string[]> PS = findPhosphateSequences(chainID);
            if (PS.Count > 3)
            {
                List<Vector3> controlPoints = new List<Vector3>();
                for (int i = 0; i < PS.Count; i++)
                {
                    float ax = float.Parse(PS[i][4]) * scale;
                    float ay = float.Parse(PS[i][5]) * scale;
                    float az = float.Parse(PS[i][6]) * scale;
                    Vector3 v = new Vector3(ax, ay, az);
                    controlPoints.Add(v);
                }

                CreateBezierCurve(controlPoints);
            }
        }
    }

    void CreateBezierCurve(List<Vector3> controlPoints)
    {
        GameObject parenthelix = new GameObject("RNA Helix " + ID);
        parenthelix.AddComponent<MeshFilter>();
        parenthelix.AddComponent<MeshRenderer>();
        parenthelix.GetComponent<Renderer>().material.color = new Color32(0xFF, 0xA5, 0x00, 0xFF);
        parenthelix.transform.parent = RNAHelices.transform;

        for (int i = 0; i < controlPoints.Count - 3; i += 3)
        {
            Vector3 p0 = controlPoints[i];
            Vector3 p1 = controlPoints[i + 1];
            Vector3 p2 = controlPoints[i + 2];
            Vector3 p3 = controlPoints[i + 3];

            for (int j = 1; j <= numPoints; j++)
            {
                float t = j / (float)numPoints;
                Vector3 pointOnCurve = CalculateCubicCurve(p0, p1, p2, p3, t);
                positions[j - 1] = pointOnCurve;
                if (j > 1)
                {
                    CreateConnection(parenthelix, positions[j - 2], pointOnCurve);
                }
            }
        }

        ID++;
    }

    Vector3 CalculateCubicCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float ttt = tt * t;
        float uuu = uu * u;

        Vector3 p = uuu * p0; // uuu * P0
        p += 3 * uu * t * p1; // 3 * uu * t * P1
        p += 3 * u * tt * p2; // 3 * u * tt * P2
        p += ttt * p3; // ttt * P3

        return p;
    }

    void CreateConnection(GameObject parent, Vector3 v1, Vector3 v2)
    {
        GameObject connect = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        connect.transform.parent = parent.transform;
        Renderer renderer = connect.GetComponent<Renderer>();
        connect.transform.localScale = new Vector3(0.6f, Vector3.Distance(v1, v2), 0.6f);
        connect.transform.localPosition = new Vector3((v1.x + v2.x) / 2, (v1.y + v2.y) / 2, (v1.z + v2.z) / 2);
        connect.transform.LookAt(v2);
        connect.transform.Rotate(90.0f, 0f, 0f, Space.Self);
        renderer.material.color = new Color32(0xFF, 0xA5, 0x00, 0xFF);
    }

    public void makemeshes()
    {
        int childCount = RNAHelices.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = RNAHelices.transform.GetChild(i);
            AtomGenerator.CombineAllMeshes(child.gameObject);
        }
    }

    

    public List<string[]> findPhosphateSequences(string chainID) //Locates for the atoms in the sequence to create the curves through
    {
        List<string[]> toReturn = new List<string[]>();

        int i = 0;
        while (i < atoms.Count())
        {
            if (atoms[i][2] == chainID)
            {
                if (atoms[i][9] == "P")
                {
                    if (isPhosphateSequence(atoms[i]) && !toReturn.Contains(atoms[i]))
                    {
                        toReturn.Add(atoms[i]);
                    }
                }
                else if (!toReturn.Contains(atoms[i]) && (atoms[i][9] == "O5'" || atoms[i][9] == "C5'" || atoms[i][9] == "C4'" || atoms[i][9] == "C3'" || atoms[i][9] == "O3'"))
                {
                    toReturn.Add(atoms[i]);
                }
            }
            i++;
        }
        return toReturn;
    }

    public bool isPhosphateSequence(string[] startAtom) //Checks to make sure it is a phosphate for an actual RNA sequence and not just a random P atom
    {
        int i = atoms.IndexOf(startAtom);
        if (startAtom[2] != atoms[i + 21][2])
        {
            return false;
        }
        i += 3;
        return i < atoms.Count() && atoms[i][9].Contains("'");
    }
}
