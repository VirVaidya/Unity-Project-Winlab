using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ElementColor : MonoBehaviour
{
    static string FilePath = "Assets/Scripts/Attempt 3/Elements.txt";
    static Hashtable colors = new Hashtable();
    static Hashtable scales = new Hashtable();

    public void createmap()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string path = Path.Combine(currentDirectory, FilePath);
        //Debug.Log(path);
        
        string line;
        using(StreamReader reader = new StreamReader(path))
        {
            while ((line = reader.ReadLine()) != null) 
            { 
                string atom = line.Split(' ')[0];
               
                string hexvalue = line.Split(' ')[1];
                
                string scale = line.Split(" ")[2];

                colors.Add(atom, hexvalue);
                scales.Add(atom, scale);
            }

        }
    }

    public static Color32 AssignColor(string abbr)
    {   
        if (!colors.ContainsKey(abbr))
        {
            Debug.LogError($"Could not find Color {abbr}");
            return Color.magenta;
        }

        string hexcode = (string)colors[abbr];
        string r = hexcode.Substring(0, 2);
        string g = hexcode.Substring(2, 2);
        string b = hexcode.Substring(4, 2).Trim();

        byte red = byte.Parse(r, System.Globalization.NumberStyles.HexNumber);
        byte green = byte.Parse(g, System.Globalization.NumberStyles.HexNumber);
        byte blue = byte.Parse(b, System.Globalization.NumberStyles.HexNumber);

        return new Color32(red, green, blue, 0xFF);
    }

    public static void AssignScale(GameObject atom, string abbr)
    {
        Vector3 originalscale = atom.transform.localScale;
        string scale = (string)scales[abbr];
        originalscale.x *= float.Parse(scale);
        originalscale.y *= float.Parse(scale);
        originalscale.z *= float.Parse(scale);

        atom.transform.localScale = originalscale;

    }
}
