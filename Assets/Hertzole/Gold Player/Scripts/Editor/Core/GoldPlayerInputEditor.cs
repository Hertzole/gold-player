#pragma warning disable CS0618 // Type or member is obsolete
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    [CustomEditor(typeof(GoldPlayerInput))]
    public class GoldPlayerInputEditor : UnityEditor.Editor
    {
        private SerializedProperty useKeyCodes;
        private SerializedProperty inputs;

        private void OnEnable()
        {
            useKeyCodes = serializedObject.FindProperty("useKeyCodes");
            inputs = serializedObject.FindProperty("inputs");
        }

        public override void OnInspectorGUI()
        {
#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
            if (GUILayout.Button("Replace with Gold Player Input System"))
            {
                GameObject go = ((GoldPlayerInput)target).gameObject;

                Undo.DestroyObjectImmediate(go.GetComponent<GoldPlayerInput>());
                Undo.AddComponent<GoldPlayerInputSystem>(go);
            }
#else
            EditorGUILayout.PropertyField(useKeyCodes);
            EditorGUILayout.PropertyField(inputs);
#endif
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
