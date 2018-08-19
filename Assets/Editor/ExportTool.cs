using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ExportTool
{
    [MenuItem("Tools/Export")]
    private static void Export()
    {
        string[] allPaths = AssetDatabase.GetAllAssetPaths();
        List<string> validPaths = new List<string>();

        for (int i = 0; i < allPaths.Length; i++)
        {
            if (allPaths[i].StartsWith("Assets/"))
            {
                if (allPaths[i].StartsWith("Assets/Editor"))
                    continue;
                if (allPaths[i].ToLower().Contains("probuilder"))
                    continue;
                if (allPaths[i].ToLower().Contains("Gold Player Tests"))
                    continue;

                validPaths.Add(allPaths[i]);
            }
        }

        string exportPath = EditorUtility.SaveFilePanel("Export package", Application.dataPath + "/../Exports", "Gold Player", "unitypackage");

        if (!string.IsNullOrEmpty(exportPath))
            AssetDatabase.ExportPackage(validPaths.ToArray(), exportPath, ExportPackageOptions.Interactive);
    }
}
