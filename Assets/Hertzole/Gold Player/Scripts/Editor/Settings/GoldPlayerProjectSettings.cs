#if UNITY_2018_3_OR_NEWER
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    [System.Serializable]
    public class GoldPlayerProjectSettings
    {
        internal const string SETTINGS_LOCATION = "ProjectSettings/Packages/se.hertzole.goldplayer/GoldPlayerSettings.json";

        public bool disableInteraction = false;
        public bool disableUI = false;
        public bool disableGraphics = false;
        public bool disableAnimator = false;
        public bool disableAudioExtras = false;
        public bool disableObjectBob = false;

        internal static GoldPlayerProjectSettings GetOrCreate()
        {
            GoldPlayerProjectSettings settings = new GoldPlayerProjectSettings();

            if (File.Exists(SETTINGS_LOCATION))
            {
                settings = JsonUtility.FromJson<GoldPlayerProjectSettings>(File.ReadAllText(SETTINGS_LOCATION));
            }
            else
            {
                settings.disableInteraction = false;
                settings.disableUI = false;
                settings.disableGraphics = false;
                settings.disableAnimator = false;
                settings.disableAudioExtras = false;
                settings.disableObjectBob = false;

                Save(settings);
            }

            return settings;
        }

        internal static void Save(GoldPlayerProjectSettings settings)
        {
            if (!Directory.Exists("ProjectSettings/Packages/se.hertzole.goldplayer"))
            {
                Directory.CreateDirectory("ProjectSettings/Packages/se.hertzole.goldplayer");
            }

            File.WriteAllText(SETTINGS_LOCATION, JsonUtility.ToJson(settings, true));
        }
    }

    internal class GoldPlayerSettingsProvider : SettingsProvider
    {
        private GoldPlayerProjectSettings settings;

        private GoldPlayerSettingsProvider(string path, SettingsScope scopes) : base(path, scopes)
        {
            label = "Gold Player";
            settings = GoldPlayerProjectSettings.GetOrCreate();
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new GoldPlayerSettingsProvider("Project/Gold Player", SettingsScope.Project);
        }

        public override void OnGUI(string searchContext)
        {
            settings.disableInteraction = EditorGUILayout.Toggle("Disable Interaction", settings.disableInteraction);
            settings.disableUI = EditorGUILayout.Toggle("Disable uGUI", settings.disableUI);
            settings.disableGraphics = EditorGUILayout.Toggle("Disable Graphics", settings.disableGraphics);
            settings.disableAnimator = EditorGUILayout.Toggle("Disable Animator", settings.disableAnimator);
            settings.disableAudioExtras = EditorGUILayout.Toggle("Disable Audio Extras", settings.disableAudioExtras);
            settings.disableObjectBob = EditorGUILayout.Toggle("Disable Object Bob", settings.disableObjectBob);

            EditorGUILayout.Space();

            if (GUILayout.Button("Apply", GUILayout.Width(EditorGUIUtility.fieldWidth)))
            {
                if (EditorUtility.DisplayDialog("Notice", "This will add new script defines and trigger a script reload. Are you sure you want to do this?", "Yes", "No"))
                {
                    List<string> remove = new List<string>();
                    List<string> add = new List<string>();
                    if (settings.disableInteraction)
                    {
                        add.Add("GOLD_PLAYER_DISABLE_INTERACTION");
                    }
                    else
                    {
                        remove.Add("GOLD_PLAYER_DISABLE_INTERACTION");
                    }

                    if (settings.disableUI)
                    {
                        add.Add("GOLD_PLAYER_DISABLE_UI");
                    }
                    else
                    {
                        remove.Add("GOLD_PLAYER_DISABLE_UI");
                    }

                    if (settings.disableGraphics)
                    {
                        add.Add("GOLD_PLAYER_DISABLE_GRAPHICS");
                    }
                    else
                    {
                        remove.Add("GOLD_PLAYER_DISABLE_GRAPHICS");
                    }

                    if (settings.disableAnimator)
                    {
                        add.Add("GOLD_PLAYER_DISABLE_ANIMATOR");
                    }
                    else
                    {
                        remove.Add("GOLD_PLAYER_DISABLE_ANIMATOR");
                    }

                    if (settings.disableAudioExtras)
                    {
                        add.Add("GOLD_PLAYER_DISABLE_AUDIO_EXTRAS");
                    }
                    else
                    {
                        remove.Add("GOLD_PLAYER_DISABLE_AUDIO_EXTRAS");
                    }

                    if (settings.disableObjectBob)
                    {
                        add.Add("GOLD_PLAYER_DISABLE_OBJECT_BOB");
                    }
                    else
                    {
                        remove.Add("GOLD_PLAYER_DISABLE_OBJECT_BOB");
                    }

                    GoldPlayerScriptHelpers.AddAndRemove(add, remove);
                    GoldPlayerProjectSettings.Save(settings);
                }
            }
        }
    }
}
#endif
