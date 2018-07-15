using UnityEditor;

namespace Hertzole.GoldPlayer.Interaction.Editor
{
    [InitializeOnLoad]
    public static class GoldPlayerInteractionDefine
    {
        private const string DEFINE = "GOLD_PLAYER_INTERACTION";

        // When a script reload happens, add the required definition to the project.
        static GoldPlayerInteractionDefine()
        {
            string scriptDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!scriptDefines.Contains(DEFINE))
            {
                string toAdd = string.Empty;
                if (!scriptDefines.EndsWith(";"))
                    toAdd = ";";
                toAdd += DEFINE;

                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, toAdd);
            }
        }
    }
}
