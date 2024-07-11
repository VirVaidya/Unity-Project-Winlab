using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

public static class FetchPDB
{
    private static readonly HttpClient httpClient = new HttpClient();

    // Fetches PDB or CIF file based on the provided pdb_id and saves it to the Assets/Input directory
    public static async Task FetchAndSavePDBAsync(string pdbId)
    {
        if (string.IsNullOrEmpty(pdbId))
        {
            Debug.LogError("PDB ID is null or empty.");
            return;
        }

        // Create the Assets/Input directory if it does not exist
        string inputDir = Path.Combine(Application.dataPath, "Input");
        if (!Directory.Exists(inputDir))
        {
            Directory.CreateDirectory(inputDir);
        }

        string pdbUrl = $"https://files.rcsb.org/download/{pdbId}.pdb";
        string cifUrl = $"https://files.rcsb.org/download/{pdbId}.cif";

        // Try to download the PDB file
        HttpResponseMessage response = await httpClient.GetAsync(pdbUrl);
        if (response.IsSuccessStatusCode)
        {
            string filePath = Path.Combine(inputDir, $"{pdbId}.pdb");
            await SaveFileAsync(response, filePath);
            Debug.Log($"PDB file saved at: {filePath}");
        }
        else
        {
            // Try to download the CIF file if PDB file was not found
            response = await httpClient.GetAsync(cifUrl);
            if (response.IsSuccessStatusCode)
            {
                string filePath = Path.Combine(inputDir, $"{pdbId}.cif");
                await SaveFileAsync(response, filePath);
                Debug.Log($"CIF file saved at: {filePath}");
            }
            else
            {
                Debug.LogError($"Neither PDB nor CIF file found for ID: {pdbId}");
            }
        }
    }

    // Helper method to save the file from the response stream
    private static async Task SaveFileAsync(HttpResponseMessage response, string filePath)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await response.Content.CopyToAsync(fileStream);
        }
    }
}
