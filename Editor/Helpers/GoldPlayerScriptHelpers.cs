using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
#if UNITY_2023_1_OR_NEWER
using UnityEditor.Build;
#endif

namespace Hertzole.GoldPlayer.Editor
{
    public static class GoldPlayerScriptHelpers
    {
        public static void AddAndRemoveDefines(List<string> add, List<string> remove)
        {
            var buildTargetGroups = GetBuildTargetGroups();

            List<string> newDefines = new List<string>();
            
            foreach (BuildTargetGroup group in buildTargetGroups)
            {
                newDefines.Clear();
                string[] scriptDefines = GetScriptDefineSymbols(group);
                newDefines.AddRange(scriptDefines);

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
                    SetScriptDefineSymbols(group, newDefines.ToArray());
                }
            }
        }
        
        private static BuildTargetGroup[] GetBuildTargetGroups()
        {
            Type enumType = typeof(BuildTargetGroup);

            List<BuildTargetGroup> groups = new List<BuildTargetGroup>();
            foreach (BuildTargetGroup target in (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (target == BuildTargetGroup.Unknown)
                {
                    continue;
                }

                MemberInfo[] memberInfos = enumType.GetMember(target.ToString());
                MemberInfo enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                if(enumValueMemberInfo == null)
                    continue;
                
                ObsoleteAttribute[] obsoleteAttributes = (ObsoleteAttribute[])enumValueMemberInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false);

                if (obsoleteAttributes.Length > 0)
                {
                    continue;
                }

                groups.Add(target);
            }

            return groups.ToArray();
        }

        private static string[] GetScriptDefineSymbols(BuildTargetGroup group)
        {
#if UNITY_2023_1_OR_NEWER
            PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(group), out string[] defines);
            return defines;
#else
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';');
#endif
        }

        private static void SetScriptDefineSymbols(BuildTargetGroup group, string[] defines)
        {
            #if UNITY_2023_1_OR_NEWER
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(group), defines);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
#endif
        }
    }
}
