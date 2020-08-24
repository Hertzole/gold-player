using System.Collections.Generic;
using UnityEditor;

namespace Hertzole.GoldPlayer.Editor
{
    public static class GoldPlayerScriptHelpers
    {
        public static void AddDefine(string define)
        {
            string scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!scriptDefines.Contains(define))
            {
                string toAdd = scriptDefines;
                if (!scriptDefines.EndsWith(";"))
                {
                    toAdd += ";";
                }

                toAdd += define;

                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, toAdd);
            }
        }

        public static void RemoveDefine(string define)
        {
            string scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (scriptDefines.Contains(define))
            {
                string toSet = string.Empty;

                string[] defines = scriptDefines.Split(';');
                for (int i = 0; i < defines.Length; i++)
                {
                    if (defines[i] != define)
                    {
                        toSet += defines[i];
                    }
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, toSet);
            }
        }

        public static void AddMultiple(params string[] defines)
        {
            string[] scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
            List<string> newDefines = new List<string>(scriptDefines);
            bool dirty = false;
            for (int i = 0; i < defines.Length; i++)
            {
                if (!newDefines.Contains(defines[i]))
                {
                    newDefines.Add(defines[i]);
                    dirty = true;
                }
            }

            if (dirty)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(",", newDefines));
            }
        }

        public static void AddAndRemove(List<string> add, List<string> remove)
        {
            string[] scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
            List<string> newDefines = new List<string>(scriptDefines);

            bool dirty = false;
            for (int i = 0; i < remove.Count; i++)
            {
                if (newDefines.Remove(remove[i]))
                {
                    dirty = true;
                }
            }

            for (int i = 0; i < add.Count; i++)
            {
                if (!newDefines.Contains(add[i]))
                {
                    newDefines.Add(add[i]);
                    dirty = true;
                }
            }

            if (dirty)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(",", newDefines));
            }
        }
    }
}
