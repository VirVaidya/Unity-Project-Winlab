using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;

public class CSVWriter : MonoBehaviour
{
    string pdbFile, csvFile, csvPath;
    StreamWriter writer;

    List<string[]> atomlist = new List<string[]>();

    public CSVWriter(string pdbfile, string csvfile, string csvpath)
    {
        this.pdbFile = pdbfile;
        this.csvFile = csvfile;
        this.csvPath = csvpath;
        writer = new StreamWriter(Path.Combine(csvpath, csvfile));
    }
    
    // Start is called before the first frame update
    void Start()
    {

    }

    public void WriteCSV()
    {
        string line;
        using (StreamReader reader = new StreamReader(pdbFile))
        {
            while ((line = reader.ReadLine()) != null)
            {
                if(line.StartsWith("ATOM") || line.StartsWith("HETATM"))
                {
                    WriteAtomLine(line);
                }
                else if (line.StartsWith("HELIX"))
                {
                    WriteHelixLine(line);
                }
                else if (line.StartsWith("CONECT"))
                {
                    WriteConecctLine(line);
                }
                else if (line.StartsWith("SHEET"))
                {
                    WriteSheetLine(line);
                }
            }
        }

        writer.Close();
    }

    void WriteAtomLine(string line)
    {
        string type = line.Substring(0, 6).Trim();
        string aminoAcid = line.Substring(17, 3).Trim();
        string chainnumber = line.Substring(21, 1).Trim();
        float residuenumber = float.Parse(line.Substring(22, 5).Trim());
        float x = float.Parse(line.Substring(30, 8).Trim());
        float y = float.Parse(line.Substring(38, 8).Trim());
        float z = float.Parse(line.Substring(46, 8).Trim());
        float displacement = float.Parse(line.Substring(60, 6).Trim());
        string atom = line.Substring(76, 2).Trim();
        string element = line.Substring(12, 4).Trim();
        writer.WriteLine($"{type}, {aminoAcid}, {chainnumber}, {residuenumber},{x},{y},{z},{displacement},{atom},{element}");
        atomlist.Add($"{type}, {aminoAcid}, {chainnumber}, {residuenumber},{x},{y},{z},{displacement},{atom},{element}".Split(','));
    }

    public List<string[]> GetAtomInfo()
    {
        return atomlist;
    }

    void WriteHelixLine(string line)
    {
        string type = line.Substring(0, line.IndexOf(" "));
        int startResidue = int.Parse(line.Substring(21, 4).Trim());
        int endResidue = int.Parse(line.Substring(33, 4).Trim());
        char chainID = line[19];
        writer.WriteLine($"{type}, {startResidue}, {endResidue}, {chainID}");
    }

    void WriteConecctLine (string line)
    {
        string type = line.Substring(0, 6);
        List<string> connectedatoms = new List<string>();
        int position = 6;
        int i = 0;
        string connection1 = line.Substring(6, 5).Trim();
        if (!string.IsNullOrEmpty(connection1))
        {
            connectedatoms.Add(connection1);
        }
        
        string connection2 = line.Substring(11, 5).Trim();
        if (!string.IsNullOrEmpty(connection2))
        {
            connectedatoms.Add(connection2);
        }
        string connection3 = line.Substring(16, 5).Trim();
        if (!string.IsNullOrEmpty(connection3))
        {
            connectedatoms.Add(connection3);
        }
        string connection4 = line.Substring(21, 5).Trim();
        if (!string.IsNullOrEmpty(connection4))
        {
            connectedatoms.Add(connection4);
        }
        


        /*while (position < line.Length)
        {
            int nextSpace = line.IndexOf(' ', position);
            if (nextSpace == -1) nextSpace = line.Length; 
            string connectedAtom = line.Substring(position, nextSpace - position).Trim();
            if (!string.IsNullOrEmpty(connectedAtom))
            {
                connectedatoms.Add(connectedAtom);
                i++;
            }
            position = nextSpace + 1;
        }*/

        writer.WriteLine($"{type},{string.Join(",", connectedatoms)}");

    }

    void WriteSheetLine (string line)
    {
        string type = line.Substring(0, line.IndexOf(" "));
        char chainID1 = line[21];
        int startResidue = int.Parse(line.Substring(23, 3).Trim());
        char chainID2 = line[32];
        int endResidue = int.Parse(line.Substring(34, 3).Trim());

        writer.WriteLine($"{type}, {chainID1}, {startResidue}, {chainID2}, {endResidue}");
    }
}
