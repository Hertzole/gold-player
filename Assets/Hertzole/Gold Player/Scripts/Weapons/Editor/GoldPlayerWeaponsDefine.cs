#if UNITY_EDITOR
using UnityEditor;

namespace Hertzole.GoldPlayer.Weapons.Editor
{
    [InitializeOnLoad]
    public static class GoldPlayerInteractionDefine
    {
        private const string DEFINE = "GOLD_PLAYER_WEAPONS";

        // When a script reload happens, add the required definition to the project.
        static GoldPlayerInteractionDefine()
        {
            string scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!scriptDefines.Contains(DEFINE))
            {
                string toAdd = scriptDefines;
                if (!scriptDefines.EndsWith(";"))
                    toAdd += ";";
                toAdd += DEFINE;

                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, toAdd);
            }
        }
    }
}
#endif
