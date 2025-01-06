#if GOLD_PLAYER_DISABLE_AUDIO_EXTRAS
#define OBSOLETE
#endif

using UnityEditor;

namespace Hertzole.GoldPlayer.Editor
{
#pragma warning disable CS0618 // Type or member is obsolete
    [CustomEditor(typeof(GoldPlayerAudioAnimator))]
    public class GoldPlayerAudioAnimatorEditor : UnityEditor.Editor
    {
        private SerializedProperty independentHandling;

        private void OnEnable()
        {
            independentHandling = serializedObject.FindProperty("independentAudioHandling");
        }

        public override void OnInspectorGUI()
        {
#if !OBSOLETE
            serializedObject.Update();

            EditorGUILayout.PropertyField(independentHandling);

            serializedObject.ApplyModifiedProperties();
#else
            if (UnityEngine.GUILayout.Button("Remove Component"))
            {
                Undo.DestroyObjectImmediate((GoldPlayerAudioAnimator)target);
            }
#endif
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
