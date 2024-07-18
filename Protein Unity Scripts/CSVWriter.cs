using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Unity.Collections;
using UnityEngine;

public class CSVWriter : MonoBehaviour, ReferenceFileWriter
{
    string pdbFile, csvFile, csvPath;
    StreamWriter writer;
    bool firstline = false;

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
        firstline = false;
        string line;
        using (StreamReader reader = new StreamReader(pdbFile))
        {
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("ATOM") || line.StartsWith("HETATM"))
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
                else if (line.StartsWith("TER"))
                {
                    writer.WriteLine("TER");
                }
                else if (line.StartsWith("LINK"))
                {
                    WriteLinkLine(line);
                }
                else if (line.StartsWith("TITLE"))
                {
                    WriteTitleLine(line);
                }
            }
        }

        writer.Close();
    }

    void WriteAtomLine(string line)
    {
        string type = line.Substring(0, 6).Trim();
        string number = line.Substring(6, 5).Trim();
        string aminoAcid = line.Substring(17, 3).Trim();
        string chainnumber = line.Substring(21, 1).Trim();
        string substring = line.Substring(22, 7).Trim();

        // Regular expression to match one or more digits
        Regex regex = new Regex(@"\d+");
        Match match = regex.Match(substring);
        float residuenumber = float.Parse(match.Value);
        float x = float.Parse(line.Substring(30, 8).Trim());
        float y = float.Parse(line.Substring(38, 8).Trim());
        float z = float.Parse(line.Substring(46, 8).Trim());
        float displacement = float.Parse(line.Substring(60, 6).Trim());
        string atom = line.Substring(76, 2).Trim();
        string element = line.Substring(12, 4).Trim();
        string realresiduenumber = null;
        writer.WriteLine($"{type}, {aminoAcid}, {chainnumber}, {residuenumber},{x},{y},{z},{displacement},{atom},{element},{number},{realresiduenumber}");
        atomlist.Add($"{type},{aminoAcid},{chainnumber},{residuenumber},{x},{y},{z},{displacement},{atom},{element},{number},{realresiduenumber}".Split(','));
    }

    void WriteTitleLine(string line)
    {
        string type;
        string title;
        if (!firstline)
        {
            type = "TITLE";
            title = line.Substring(6);
            firstline = true;
            writer.WriteLine($"{type},{title}");
        }
     
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
        //Number of connections varies on each line, ensures no empty connections are added to ruin conncetions
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

        writer.WriteLine($"{type},{string.Join(",", connectedatoms)}");

    }

    void WriteLinkLine(string line)
    {
        string type = line.Substring (0, 6).Trim();
        string name1 = line.Substring(12, 4).Trim();
        string chainID1 = line.Substring(21, 1).Trim();
        string sequence1 = line.Substring(22, 4).Trim();
        string name2 = line.Substring(42, 4).Trim();
        string chainID2 = line.Substring(51, 1).Trim();
        string sequence2 = line.Substring(52, 4).Trim();
        writer.WriteLine($"{type},{name1},{chainID1},{sequence1},{name2},{chainID2},{sequence2}");
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
