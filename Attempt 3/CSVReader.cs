using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Globalization;

public class CSVReader : MonoBehaviour
{
    [SerializeField] private string pdbFilename, csvFilename, csvDirectory;
    
    void Start()
    {
        WriteCSV();
        ReadCSV(csvFilename);
    }

    /*---------Write---------*/
    public void WriteCSV()
    {
        using (StreamReader pdbFile = new StreamReader(pdbFilename))
        {
            string csvPath = Path.Combine(csvDirectory, csvFilename);
            using (StreamWriter csvFile = new StreamWriter(csvPath))
            {
                WriteAtoms(pdbFile, csvFile);
            }
        }
    }

    public void WriteAtoms(StreamReader pdbFile, StreamWriter csvFile)
    {
        csvFile.WriteLine("Type, Amino Acid, Chain Number, Residue Number, X, Y, Z, Displacement, Atom");
        string line;
        while ((line = pdbFile.ReadLine()) != null)
        {
            if (line.StartsWith("ATOM") || line.StartsWith("HETATM"))
            {
                string type = line.Substring(0, line.IndexOf(" "));
                string aminoAcid = line.Substring(17, 3).Trim();
                string chainnumber = line.Substring(21, 2).Trim();
                float residuenumber = float.Parse(line.Substring(24, 2).Trim());
                float x = float.Parse(line.Substring(30, 8).Trim());
                float y = float.Parse(line.Substring(38, 8).Trim());
                float z = float.Parse(line.Substring(46, 8).Trim());
                float displacement = float.Parse(line.Substring(60, 6).Trim());
                string atom = line.Substring(76, 2).Trim();
                csvFile.WriteLine($"{type}, {aminoAcid}, {chainnumber}, {residuenumber},{x},{y},{z},{displacement},{atom}");
            }
            else if (line.StartsWith("HELIX"))
                {
                char chainID = line[19];
                int startResidue = int.Parse(line.Substring(21, 4).Trim());
                int endResidue = int.Parse(line.Substring(33, 4).Trim());

                Vector3 startPosition = GetResiduePosition(chainID, startResidue);
                Vector3 endPosition = GetResiduePosition(chainID, endResidue);
                //csvFile.WriteLine($"{chainID}, {startResidue}, {endResidue}");

               
            } 

        }
    }

    /*void WriteHelix(StreamReader pdbFilePath)
    {
        using (pdbFilePath)
        {
            string line;
            while ((line = pdbFilePath.ReadLine()) != null)
            {
                if (line.StartsWith("HELIX"))
                {
                    int startResidue = int.Parse(line.Substring(21, 4).Trim());
                    int endResidue = int.Parse(line.Substring(33, 4).Trim());
                    char chainID = line[19];

                    // For simplicity, let's create a helix object and position it
                    Vector3 startPosition = GetResiduePosition(chainID, startResidue);
                    Vector3 endPosition = GetResiduePosition(chainID, endResidue);

                    CreateHelix(startPosition, endPosition);
                }
                // Add similar parsing for SHEET and TURN if needed
            }
        }
    }*/

    void CreateHelix(Vector3 startPosition, Vector3 endPosition, char chainID, GameObject protein)
    {
        GameObject helix = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        helix.transform.position = (startPosition + endPosition) / 2;
        helix.transform.localScale = new Vector3(1, 1, Vector3.Distance(startPosition, endPosition));
        helix.transform.LookAt(endPosition);
        helix.transform.parent = protein.transform;

    }

    Vector3 GetResiduePosition(char chainID, int residueNumber)
    {
        // This method should return the position of the residue in 3D space
        // You'll need to parse the ATOM/HETATM records to get actual coordinates
        return new Vector3(); // Placeholder
    }


    /*----------READ-----------*/
    public void ReadCSV(string fileName)
    {
        GameObject protein = new GameObject();
        protein.name = "Protein";
        
        string csvPath = Path.Combine(csvDirectory, csvFilename);
        using (StreamReader csvFile = new StreamReader(csvPath))
        {
            ReadAtoms(csvFile, protein);
        }
    }



    

    public void ReadAtoms(StreamReader csvFile, GameObject protein)
    {
        // skips headers
        csvFile.ReadLine();

        string line;
        int ID = 1;
        while ((line = csvFile.ReadLine()) != null)
        {
            string[] attributes = line.Split(',');
            //Debug.Log(attributes[2] + ", " + attributes[3] + ", " + attributes[4]);
            float atomX = float.Parse(attributes[4]);
            float atomY = float.Parse(attributes[5]);
            float atomZ = float.Parse(attributes[6]);

            GameObject atom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            atom.name = "atom-" + ID.ToString();
            atom.transform.parent = protein.transform;
            atom.transform.localPosition = new Vector3(atomX, atomY, atomZ);
            atom.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            AssignColor(atom, attributes[8]);

            ID++;
        }
    }

    public void AssignColor(GameObject atom, string element)
    {
        Renderer renderer = atom.GetComponent<Renderer>();
        
        switch (element)
        {
            case ("C"):
                renderer.material.color = Color.black;
                break;
            case ("N"):
                renderer.material.color = Color.blue;
                break;
            case ("H"):
                renderer.material.color = Color.white;
                break;
            case ("O"):
                renderer.material.color = Color.red;
                break;
            case ("S"):
                renderer.material.color = Color.yellow;
                break;
            default:
                renderer.material.color = Color.magenta;
                break;
        }
    }
}
