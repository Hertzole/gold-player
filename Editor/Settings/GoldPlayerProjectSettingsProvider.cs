using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    public class GoldPlayerProjectSettingsProvider : SettingsProvider
    {
        private GoldPlayerProjectSettings settings;

        private readonly GUIContent uiAdapationContent = new GUIContent("GUI Adaption", "Determines how the editor GUI should adapt in the inspector.");
        private readonly GUIContent showGroundGizmosContent = new GUIContent("Show Ground Gizmos", "If true, the ground check gizmos will be visible when the player is selected.");
        private readonly GUIContent disableOptimizationsContent = new GUIContent("Disable Optimizations", "If true, some special optimizations will be disabled.");

        public GoldPlayerProjectSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        public override void OnGUI(string searchContext)
        {
            if (settings == null)
            {
                settings = GoldPlayerProjectSettings.Instance;
            }

            using (new SettingsGUIScope())
            {
                EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);

                EditorGUIAdaption uiAdapation = settings.GUIAdapation;
                EditorGUI.BeginChangeCheck();
                uiAdapation = (EditorGUIAdaption)EditorGUILayout.EnumPopup(uiAdapationContent, uiAdapation);
                if (EditorGUI.EndChangeCheck())
                {
                    settings.GUIAdapation = uiAdapation;
                }

                GUILayout.Space(16f);

                EditorGUILayout.LabelField("Scene View", EditorStyles.boldLabel);

                bool showGizmos = settings.ShowGroundCheckGizmos;
                EditorGUI.BeginChangeCheck();
                showGizmos = EditorGUILayout.Toggle(showGroundGizmosContent, showGizmos);
                if (EditorGUI.EndChangeCheck())
                {
                    settings.ShowGroundCheckGizmos = showGizmos;
                }

                GUILayout.Space(16f);

                EditorGUILayout.LabelField("Disable Components", EditorStyles.boldLabel);

                EditorGUILayout.HelpBox("Disabling components strips them out of your game. This is much more recommended than outright removing script files.", MessageType.Info);

                settings.DisableInteraction = EditorGUILayout.Toggle("Disable Interaction", settings.DisableInteraction);
                settings.DisableUI = EditorGUILayout.Toggle("Disable uGUI", settings.DisableUI);
                settings.DisableGraphics = EditorGUILayout.Toggle("Disable Graphics", settings.DisableGraphics);
                settings.DisableAnimator = EditorGUILayout.Toggle("Disable Animator", settings.DisableAnimator);
                settings.DisableAudioExtras = EditorGUILayout.Toggle("Disable Audio Extras", settings.DisableAudioExtras);
                settings.DisableObjectBob = EditorGUILayout.Toggle("Disable Object Bob", settings.DisableObjectBob);

                EditorGUILayout.Space();

                DrawApplyButton();

                GUILayout.Space(16f);

                EditorGUILayout.LabelField("Optimizations", EditorStyles.boldLabel);

                settings.DisableOptimizations = EditorGUILayout.Toggle(disableOptimizationsContent, settings.DisableOptimizations);

                EditorGUILayout.Space();

                DrawApplyButton();
            }
        }

        private void DrawApplyButton()
        {
            if (GUILayout.Button("Apply", GUILayout.Width(EditorGUIUtility.fieldWidth)))
            {
                if (EditorUtility.DisplayDialog("Notice", "This will add new script defines and trigger a script reload. Are you sure you want to do this?", "Yes", "No"))
                {
                    GoldPlayerProjectSettings.ApplyDefines(GoldPlayerProjectSettings.Instance);
                }
                else
                {
                    settings = GoldPlayerProjectSettings.GetOrCreate();
                }
            }
        }
        
        private class SettingsGUIScope : GUI.Scope
        {
            private readonly float labelWidth;

            public SettingsGUIScope()
            {
                GUILayout.BeginVertical();

                labelWidth = EditorGUIUtility.labelWidth;

                if (EditorGUILayout.GetControlRect(false, 0).width > 550)
                {
                    EditorGUIUtility.labelWidth = 250;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Space(7);
                GUILayout.BeginVertical();
                GUILayout.Space(4);
            }

            protected override void CloseScope()
            {
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                EditorGUIUtility.labelWidth = labelWidth;
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new GoldPlayerProjectSettingsProvider("Hertzole/Gold Player", SettingsScope.Project)
            {
                label = "Gold Player",
                keywords = new string[]
                {
                    "hertzole",
                    "gold",
                    "player",
                    "controller",
                    "disable",
                    "components",
                    "optimizations",
                    "graphics",
                    "ugui",
                    "interaction",
                    "animator",
                    "audio",
                    "extras",
                    "object",
                    "bob",
                    "ground",
                    "gizmos",
                    "gui",
                    "adaption",
                    "disable"
                }
            };
        }
    }
}
