using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Splines;

public class HelixGenerator : MonoBehaviour
{
    GameObject protein;
    int ID = 1;

    private float scale = 1;

    private static int numPoints = 110;
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
        string chainID = attributes[3].Trim();

        List<string[]> range = GetAlphaCarbons(startresidue, endresidue, chainID, atoms);
        List<Vector3> controlPoints = new List<Vector3>();

        for (int i = 0; i < range.Count(); i++)
        {
            float ax = float.Parse(range[i][4]) * scale;
            float ay = float.Parse(range[i][5]) * scale;
            float az = float.Parse(range[i][6]) * scale;
            Vector3 v = new Vector3(ax, ay, az);
            controlPoints.Add(v);
        }

        // Ensure there are enough control points for at least one segment
        if (controlPoints.Count >= 4)
        {
            CreateBezierCurve(controlPoints);
        }
        else
        {
            Debug.LogWarning("Not enough control points to create a Bezier curve.");
        }
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

    public void makemeshes()
    {
        int childCount = Helices.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = Helices.transform.GetChild(i);
            CombineAllMeshes(child.gameObject);
        }
    }

    public void CombineAllMeshes(GameObject helixparent)
    {
        MeshFilter[] meshfilter = helixparent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshfilter.Length];
        Matrix4x4 parentTransform = helixparent.transform.localToWorldMatrix;
        Matrix4x4 parentInverse = parentTransform.inverse;
        int i = 0;
        Vector3 originalPosition = helixparent.transform.localPosition;
        Vector3 originalScale = helixparent.transform.localScale;

        while (i < meshfilter.Length)
        {
            combine[i].mesh = meshfilter[i].sharedMesh;
            //combine[i].transform = meshfilter[i].transform.localToWorldMatrix;
            combine[i].transform = parentInverse * meshfilter[i].transform.localToWorldMatrix;
            meshfilter[i].gameObject.SetActive(false);

            i++;
        }
        MeshFilter mf = helixparent.GetComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mf.mesh.CombineMeshes(combine);

        Renderer mr = helixparent.GetComponent<Renderer>();
        mr.material.color = meshfilter[0].gameObject.GetComponent<Renderer>().material.color;

        helixparent.transform.localPosition = originalPosition;
        helixparent.transform.localScale = originalScale;
        helixparent.SetActive(true);
    }

    void CreateBezierCurve(List<Vector3> controlPoints)
    {
        GameObject parenthelix = new GameObject("Helix " + ID);
        parenthelix.AddComponent<MeshFilter>();
        parenthelix.AddComponent<MeshRenderer>();
        parenthelix.GetComponent<Renderer>().material.color = Color.green;
        parenthelix.transform.parent = Helices.transform;

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
