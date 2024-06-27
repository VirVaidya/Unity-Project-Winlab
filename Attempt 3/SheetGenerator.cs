using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SheetGenerator : MonoBehaviour
{
    GameObject protein;
    int ID = 1;

    private float scale = 1;

    private static int numPoints = 20;
    private Vector3[] positions = new Vector3[numPoints];
    GameObject sheets = new GameObject();


    public SheetGenerator(GameObject protein)
    {
        this.protein = protein;
        sheets.name = "Sheets";
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
        for (int i = 0; i < range.Count() - 2; i++)
        {
            float a1x = float.Parse(range[i][4]);
            float a1y = float.Parse(range[i][5]);
            float a1z = float.Parse(range[i][6]);
            Vector3 v1 = new Vector3(a1x, a1y, a1z);

            float a2x = float.Parse(range[i + 1][4]);
            float a2y = float.Parse(range[i + 1][5]);
            float a2z = float.Parse(range[i + 1][6]);
            Vector3 v2 = new Vector3(a2x, a2y, a2z);

            float a3x = float.Parse(range[i + 2][4]) * scale;
            float a3y = float.Parse(range[i + 2][5]) * scale;
            float a3z = float.Parse(range[i + 2][6]) * scale;
            Vector3 v3 = new Vector3(a3x, a3y, a3z);

            CreateCurve(v1, v2, v3);
        }

    }
    public void makemeshes()
    {
        int childCount = sheets.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = sheets.transform.GetChild(i);
            CombineAllMeshes(child.gameObject);
        }
    }
    public void CombineAllMeshes(GameObject sheetparent)
    {
        MeshFilter[] meshfilter = sheetparent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshfilter.Length];
        Matrix4x4 parentTransform = sheetparent.transform.localToWorldMatrix;
        Matrix4x4 parentInverse = parentTransform.inverse;
        int i = 0;
        Vector3 originalPosition = sheetparent.transform.localPosition;
        Vector3 originalScale = sheetparent.transform.localScale;


        while (i < meshfilter.Length)
        {
            combine[i].mesh = meshfilter[i].sharedMesh;
            //combine[i].transform = meshfilter[i].transform.localToWorldMatrix;
            combine[i].transform = parentInverse * meshfilter[i].transform.localToWorldMatrix;
            meshfilter[i].gameObject.SetActive(false);


            i++;
        }
        MeshFilter mf = sheetparent.GetComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mf.mesh.CombineMeshes(combine);


        Renderer mr = sheetparent.GetComponent<Renderer>();
        mr.material.color = meshfilter[0].gameObject.GetComponent<Renderer>().material.color;

        sheetparent.transform.localPosition = originalPosition;
        sheetparent.transform.localScale = originalScale;
        sheetparent.SetActive(true);
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

    void CreateCurve(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        GameObject parentsheet = new GameObject("sheet " + ID);
        parentsheet.AddComponent<MeshFilter>();
        parentsheet.AddComponent<MeshRenderer>();
        parentsheet.GetComponent<Renderer>().material.color = Color.gray;
        parentsheet.transform.parent = sheets.transform;
        positions[0] = p0;
        for (int i = 1; i < numPoints; i++)
        {
            float t = i / (float)numPoints;
            positions[i] = CalculateCurve(p0, p1, p2, t);
            CreateConnection(parentsheet, positions[i - 1], positions[i]);
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

    List<string[]> GetBetaCarbons(int start, int end, string chainID1, string chainID2, AtomGenerator atoms)
    {
        List<string[]> toReturn = new List<string[]>();
        List<string[]> atomsInRange = GetAtomsInRange(start, end, chainID1, chainID2, atoms);
        foreach (string[] atom in atomsInRange)
        {
            if (atom[9].Equals("CA"))
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
        while (int.Parse(allatoms[i][3]) <= end || allatoms[i][2] != chainId2)
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
