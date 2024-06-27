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
        for(int i = 0; i < atoms.Count; i++)
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
            if(PS.Count > 1)
            {
                for (int i = 0; i < PS.Count() - 2; i += 2)
                {
                    //Debug.Log($"{i}: {PS[i][10]}, {PS[i + 1][10]}, {PS[i + 2][10]}");
                    
                    float a1x = float.Parse(PS[i][4]) * scale;
                    float a1y = float.Parse(PS[i][5]) * scale;
                    float a1z = float.Parse(PS[i][6]) * scale;
                    Vector3 v1 = new Vector3(a1x, a1y, a1z);

                    float a2x = float.Parse(PS[i + 1][4]) * scale;
                    float a2y = float.Parse(PS[i + 1][5]) * scale;
                    float a2z = float.Parse(PS[i + 1][6]) * scale;
                    Vector3 v2 = new Vector3(a2x, a2y, a2z);

                    float a3x = float.Parse(PS[i + 2][4]) * scale;
                    float a3y = float.Parse(PS[i + 2][5]) * scale;
                    float a3z = float.Parse(PS[i + 2][6]) * scale;
                    Vector3 v3 = new Vector3(a3x, a3y, a3z);

                    CreateCurve(v1, v2, v3);
                }
            }
        }
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
            CombineAllMeshes(child.gameObject);
        }
    }
    public void CombineAllMeshes(GameObject RNAhelixparent)
    {
        MeshFilter[] meshfilter = RNAhelixparent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshfilter.Length];
        Matrix4x4 parentTransform = RNAhelixparent.transform.localToWorldMatrix;
        Matrix4x4 parentInverse = parentTransform.inverse;
        int i = 0;
        Vector3 originalPosition = RNAhelixparent.transform.localPosition;
        Vector3 originalScale = RNAhelixparent.transform.localScale;


        while (i < meshfilter.Length)
        {
            combine[i].mesh = meshfilter[i].sharedMesh;
            combine[i].transform = parentInverse * meshfilter[i].transform.localToWorldMatrix;
            meshfilter[i].gameObject.SetActive(false);


            i++;
        }
        MeshFilter mf = RNAhelixparent.GetComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mf.mesh.CombineMeshes(combine);


        Renderer mr = RNAhelixparent.GetComponent<Renderer>();
        mr.material.color = meshfilter[0].gameObject.GetComponent<Renderer>().material.color;

        RNAhelixparent.transform.localPosition = originalPosition;
        RNAhelixparent.transform.localScale = originalScale;
        RNAhelixparent.SetActive(true);
    }
    void CreateCurve(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        GameObject parenthelix = new GameObject("RNA Helix " + ID);
        parenthelix.AddComponent<MeshFilter>();
        parenthelix.AddComponent<MeshRenderer>();
        parenthelix.GetComponent<Renderer>().material.color = new Color32(0xFF, 0xA5, 0x00, 0xFF);
        parenthelix.transform.parent = RNAHelices.transform;
        positions[0] = p0;
        for (int i = 1; i < numPoints; i++)
        {
            float t = i / (float)numPoints;
            positions[i] = CalculateCurve(p0, p1, p2, t);
            CreateConnection(parenthelix, positions[i - 1], positions[i]);
        }
        //parenthelix.transform.Rotate(0f, 180f, 0f, Space.Self);
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

    public List<string[]> findPhosphateSequences(string chainID)
    {
        List<string[]> toReturn = new List<string[]>();
        
        int i = 0;
        while(i < atoms.Count())
        {
            if (atoms[i][2] == chainID)
            {
                if (atoms[i][9] == "P")
                {
                    if (isPhosphateSequence(atoms[i]) && !toReturn.Contains(atoms[i]))
                    {
                        toReturn.Add(atoms[i]);
                        //Debug.Log($"Added atom #{atoms[i][10]}");
                    }
                }
                else if (!toReturn.Contains(atoms[i]) && (atoms[i][9] == "O5'" || atoms[i][9] == "C5'" || atoms[i][9] == "C4'" || atoms[i][9] == "C3'" || atoms[i][9] == "O3'"))
                {
                    toReturn.Add(atoms[i]);
                    //Debug.Log($"Added atom #{atoms[i][10]}");
                } 
            }
            i++;
        }
        //Debug.Log("--------------------------------");
        return toReturn;
    }

    public bool isPhosphateSequence(string[] startAtom)
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
