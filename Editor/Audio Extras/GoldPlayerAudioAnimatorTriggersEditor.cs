#if GOLD_PLAYER_DISABLE_AUDIO_EXTRAS
#define OBSOLETE
#endif

using UnityEditor;

namespace Hertzole.GoldPlayer.Editor
{
#pragma warning disable CS0618 // Type or member is obsolete
    [CustomEditor(typeof(GoldPlayerAudioAnimatorTriggers))]
    public class GoldPlayerAudioAnimatorTriggersEditor : UnityEditor.Editor
    {
        private SerializedProperty audioTarget;

        private void OnEnable()
        {
            audioTarget = serializedObject.FindProperty("audioTarget");
        }

        public override void OnInspectorGUI()
        {
#if !OBSOLETE
            serializedObject.Update();

            EditorGUILayout.PropertyField(audioTarget);

            serializedObject.ApplyModifiedProperties();
#else
            if (UnityEngine.GUILayout.Button("Remove Component"))
            {
                Undo.DestroyObjectImmediate((GoldPlayerAudioAnimatorTriggers)target);
            }
#endif
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
