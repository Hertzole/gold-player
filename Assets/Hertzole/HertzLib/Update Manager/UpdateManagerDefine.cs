#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Hertzole.HertzLib.Editor
{
    [InitializeOnLoad]
    internal static class UpdateManagerDefine
    {
        private const string DEFINE = "HERTZLIB_UPDATE_MANAGER";

        // When a script reload happens, add the required definition to the project.
        static UpdateManagerDefine()
        {
            string scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!scriptDefines.Contains(DEFINE))
            {
                string toAdd = scriptDefines;
                if (!scriptDefines.EndsWith(";"))
                    toAdd += ";";
                toAdd += DEFINE;

                Debug.Log("Added '" + DEFINE + "' define");

                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, toAdd);
            }
        }
    }
}
#endif
