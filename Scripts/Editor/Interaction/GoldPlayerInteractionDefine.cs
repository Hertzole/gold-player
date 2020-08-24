#if UNITY_EDITOR
using UnityEditor;

namespace Hertzole.GoldPlayer.Editor
{
    [InitializeOnLoad]
    public static class GoldPlayerInteractionDefine
    {
        private const string DEFINE = "GOLD_PLAYER_INTERACTION";

        // When a script reload happens, add the required definition to the project.
        static GoldPlayerInteractionDefine()
        {
            GoldPlayerScriptHelpers.AddDefine(DEFINE);
        }
    }
}
#endif
