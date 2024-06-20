using UnityEngine;

public class AtomCombiner : MonoBehaviour
{
    public void CombineAtoms(GameObject elementContainer)
    {
        // Get all MeshFilters in the container
        MeshFilter[] meshFilters = elementContainer.GetComponentsInChildren<MeshFilter>();

        // Prepare CombineInstance array
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false); // Disable original objects
        }

        // Create combined mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances, true);

        // Create GameObject for the combined mesh
        GameObject combinedObject = new GameObject(elementContainer.name + "_Combined");
        combinedObject.transform.parent = elementContainer.transform.parent; // Parent to same level as containers
        combinedObject.transform.localPosition = Vector3.zero;
        combinedObject.transform.localRotation = Quaternion.identity;

        // Add MeshFilter and MeshRenderer
        MeshFilter meshFilter = combinedObject.AddComponent<MeshFilter>();
        meshFilter.mesh = combinedMesh;
        MeshRenderer meshRenderer = combinedObject.AddComponent<MeshRenderer>();
        meshRenderer.material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial; // Use material of the first mesh

        // Optional: Add collider or other components to combinedObject
    }
}
