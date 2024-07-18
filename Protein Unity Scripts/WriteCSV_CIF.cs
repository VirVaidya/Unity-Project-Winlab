using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class WriteCSV_CIF : ReferenceFileWriter
{
    string cifFile, csvFile, csvPath;
    StreamWriter writer;

    List<string[]> atomlist = new List<string[]>();

    public WriteCSV_CIF(string ciffile, string csvfile, string csvpath)
    {
        this.cifFile = ciffile;
        this.csvFile = csvfile;
        this.csvPath = csvpath;
        writer = new StreamWriter(Path.Combine(csvpath, csvfile));
    }

    void Start()
    {

    }

    public void WriteCSV()
    {
        using (StreamReader reader = new StreamReader(cifFile))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("_atom_site."))
                {
                    WriteAtomLine(reader);
                }
                else if (line.StartsWith("loop_"))
                {
                    line = reader.ReadLine();
                    if (line.StartsWith("_struct_conf."))
                    {
                        WriteHelixLines(reader);
                    }
                    else if (line.StartsWith("_struct_conn."))
                    {
                        WriteConnections(reader);
                    }
                    else if (line.StartsWith("_struct_sheet_range."))
                    {
                        WriteSheetLines(reader);
                    }
                }
                else if (line.StartsWith("_struct.title"))
                {
                    WriteTitleLine(reader);
                }
            }
        }

        writer.Close();
    }

    public void WriteTitleLine(StreamReader reader)
    {
        string line;
  
        while ((line = reader.ReadLine()) != null && !line.StartsWith("#") && !line.StartsWith("loop_"))
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                Debug.Log(line);
                string type = "TITLE";
                string title = line.Trim();
                writer.WriteLine($"{type}, {title}");
                break;
            }
        }
    }

    public void WriteAtomLine(StreamReader reader)
    {
        List<string> atomLines = new List<string>();
        string line;

        string[] headers = reader.ReadLine().Split();

        while ((line = reader.ReadLine()) != null && !line.StartsWith("#") && !line.StartsWith("loop_"))
        {
            atomLines.Add(line.Trim());
        }

        foreach (var atomLine in atomLines)
        {
            string[] data = atomLine.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (data.Length >= 11)
            {
                string type = data[0].Trim();
                string aminoAcid = data[17].Trim();
                string chainnumber = data[18].Trim();
                float residuenumber = float.Parse(data[16].Trim());
                float x = float.Parse(data[10].Trim());
                float y = float.Parse(data[11].Trim());
                float z = float.Parse(data[12].Trim());
                float displacement = float.Parse(data[14].Trim());
                string atom = data[2].Trim();
                string element = data[3].Trim();
                string number = data[1].Trim();
                string realresiduenumber = data[8].Trim();

                writer.WriteLine($"{type}, {aminoAcid}, {chainnumber}, {residuenumber},{x},{y},{z},{displacement},{atom},{element},{number},{realresiduenumber}");
                atomlist.Add($"{type},{aminoAcid},{chainnumber},{residuenumber},{x},{y},{z},{displacement},{atom},{element},{number},{realresiduenumber}".Split(','));
            }
        }
    }


    public List<string[]> GetAtomInfo()
    {
        return atomlist;
    }

    void WriteHelixLines(StreamReader reader)
    {
        List<string> helixLines = new List<string>();
        string line;

        while ((line = reader.ReadLine()) != null && !line.StartsWith("#") && !line.StartsWith("loop_"))
        {
            helixLines.Add(line.Trim());
        }

        foreach (var helixLine in helixLines)
        {
            string[] data = helixLine.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (data.Length >= 20)
            {
                string type = "HELIX";
                string startCompID = data[13].Trim(); //StartResidue
                string startAsymID = data[12].Trim(); //ChainID
                string endCompID = data[16].Trim();

                writer.WriteLine($"{type},{startCompID},{endCompID},{startAsymID}");
            }
        }
    }

    void WriteConnections(StreamReader reader)
    {
        List<string> connectionLines = new List<string>();
        string line;

        // Read connection lines
        while ((line = reader.ReadLine()) != null && !line.StartsWith("#") && !line.StartsWith("loop_"))
        {
            connectionLines.Add(line.Trim());
        }

        foreach (var connectionLine in connectionLines)
        {
            string[] data = connectionLine.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (data.Length >= 16)
            {
                string type = "CONECT";
                string chainid1 = !string.IsNullOrWhiteSpace(data[4].Trim()) && data[4].Trim() != "." ? data[4].Trim() : data[4].Trim();
                string sequenceid1 = !string.IsNullOrWhiteSpace(data[6].Trim()) && data[6].Trim() != "." ? data[6].Trim() : data[6].Trim();
                string atomId1 = !string.IsNullOrWhiteSpace(data[7].Trim()) && data[7].Trim() != "." ? data[7].Trim() : data[7].Trim();
                string chainid2 = !string.IsNullOrWhiteSpace(data[12].Trim()) && data[12].Trim() != "." ? data[12].Trim() : data[12].Trim();
                string sequenceid2 = !string.IsNullOrWhiteSpace(data[14].Trim()) && data[14].Trim() != "." ? data[14].Trim() : data[14].Trim();
                string atomid2 = !string.IsNullOrWhiteSpace(data[15].Trim()) && data[15].Trim() != "." ? data[15].Trim() : data[15].Trim();

                // Ensure there are no "?" characters in the IDs
                chainid1 = chainid1 != "?" ? chainid1 : null;
                sequenceid1 = sequenceid1 != "?" ? sequenceid1 : null;
                atomId1 = atomId1 != "?" ? atomId1 : null;
                chainid2 = chainid2 != "?" ? chainid2 : null;
                sequenceid2 = sequenceid2 != "?" ? sequenceid2 : null;
                atomid2 = atomid2 != "?" ? atomid2 : null;

                var builder = new StringBuilder(type);

                if (chainid1 != null) builder.Append($",{chainid1}");
                if (sequenceid1 != null) builder.Append($",{sequenceid1}");
                if (atomId1 != null) builder.Append($",{atomId1}");
                if (chainid2 != null) builder.Append($",{chainid2}");
                if (sequenceid2 != null) builder.Append($",{sequenceid2}");
                if (atomid2 != null) builder.Append($",{atomid2}");

                writer.WriteLine(builder.ToString());
            }
        }

    }




    void WriteSheetLines(StreamReader reader)
    {
        List<string> sheetLines = new List<string>();
        string line;

        while ((line = reader.ReadLine()) != null && !line.StartsWith("#") && !line.StartsWith("loop_"))
        {
            sheetLines.Add(line.Trim());
        }

        foreach (var sheetLine in sheetLines)
        {
            string[] data = sheetLine.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (data.Length >= 5)
            {
                string type = "SHEET";
                string chainID1 = data[11].Trim();
                string chainID2 = data[14].Trim();
                int startResidue = int.Parse(data[12].Trim());
                int endResidue = int.Parse(data[15].Trim());
                writer.WriteLine($"{type}, {chainID1}, {startResidue}, {chainID2}, {endResidue}");
            }
        }
    }
}
