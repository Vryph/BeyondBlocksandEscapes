using UnityEditor;
using UnityEngine;
using System.IO;

public class LineCounter : EditorWindow
{
    [MenuItem("Tools/Count Lines of Code")]
    public static void CountLines()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        int lineCount = 0;

        foreach (string file in files)
        {
            lineCount += File.ReadAllLines(file).Length;
        }

        Debug.Log("Total lines of code: " + lineCount);
    }
}