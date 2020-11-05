using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static void AddMultipleDefines(params string[] defines)
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
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", newDefines.ToArray()));
            }
        }

        public static void AddAndRemoveDefines(List<string> add, List<string> remove)
        {
            Type enumType = typeof(BuildTargetGroup);

            foreach (BuildTargetGroup target in (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (target == BuildTargetGroup.Unknown)
                {
                    continue;
                }

                MemberInfo[] memberInfos = enumType.GetMember(target.ToString());
                MemberInfo enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                ObsoleteAttribute[] obsoleteAttributes = (ObsoleteAttribute[])enumValueMemberInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false);

                if (obsoleteAttributes.Length > 0)
                {
                    continue;
                }

                string[] scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Split(';');
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
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", newDefines.ToArray()));
                }
            }
        }
    }
}
