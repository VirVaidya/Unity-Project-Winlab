using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface ReferenceFileWriter
{
    public void WriteCSV();
    public List<string[]> GetAtomInfo();
}
