using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    

    public void CombineMeshes()
    {
        Dictionary<(Mesh, Material), List<CombineInstance>> meshGroups = new Dictionary<(Mesh, Material), List<CombineInstance>>();

        // Iterate through all children
        foreach (Transform child in transform)
        {
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            Renderer renderer = child.GetComponent<Renderer>();

            if (meshFilter != null && renderer != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Material material = renderer.sharedMaterial;

                if (!meshGroups.ContainsKey((mesh, material)))
                {
                    meshGroups[(mesh, material)] = new List<CombineInstance>();
                }

                CombineInstance combineInstance = new CombineInstance
                {
                    mesh = mesh,
                    transform = child.localToWorldMatrix
                };
                meshGroups[(mesh, material)].Add(combineInstance);
            }
        }

        // Combine meshes for each group
        foreach (var kvp in meshGroups)
        {
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(kvp.Value.ToArray(), true, true);

            GameObject combinedObject = new GameObject(kvp.Key.Item2.name + "_Combined");
            combinedObject.transform.parent = transform;
            combinedObject.transform.localPosition = Vector3.zero;
            combinedObject.transform.localRotation = Quaternion.identity;

            MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
            combinedMeshFilter.mesh = combinedMesh;

            Renderer combinedRenderer = combinedObject.AddComponent<MeshRenderer>();
            combinedRenderer.material = kvp.Key.Item2;

            // Optionally, destroy original objects after combining
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
