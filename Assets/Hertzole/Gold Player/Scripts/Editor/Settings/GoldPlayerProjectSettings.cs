#if UNITY_2018_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Hertzole.GoldPlayer.Editor
{
    public enum EditorGUIAdaption { AlwaysShow, HideUnused, DisableUnused };

    [Serializable]
    public class GoldPlayerProjectSettings : ScriptableObject
    {

        private class OldJsonSettings
        {
            public bool disableInteraction = false;
            public bool disableUI = false;
            public bool disableGraphics = false;
            public bool disableAnimator = false;
            public bool disableAudioExtras = false;
            public bool disableObjectBob = false;
        }

        private const string DIRECTORY = "ProjectSettings/Packages/se.hertzole.goldplayer";
        private const string PATH = DIRECTORY + "/GoldPlayerSettings.asset";

        [SerializeField]
        private EditorGUIAdaption guiAdapation = EditorGUIAdaption.HideUnused;
        [SerializeField]
        private bool showGroundCheckGizmos = true;
        [SerializeField]
        private bool disableInteraction = false;
        [SerializeField]
        private bool disableUI = false;
        [SerializeField]
        private bool disableGraphics = false;
        [SerializeField]
        private bool disableAnimator = false;
        [SerializeField]
        private bool disableAudioExtras = false;
        [SerializeField]
        private bool disableObjectBob = false;

        public EditorGUIAdaption GUIAdapation { get { return guiAdapation; } set { guiAdapation = value; Save(); } }

        public bool ShowGroundCheckGizmos { get { return showGroundCheckGizmos; } set { showGroundCheckGizmos = value; Save(); } }

        public bool DisableInteraction { get { return disableInteraction; } set { disableInteraction = value; } }
        public bool DisableUI { get { return disableUI; } set { disableUI = value; } }
        public bool DisableGraphics { get { return disableGraphics; } set { disableGraphics = value; } }
        public bool DisableAnimator { get { return disableAnimator; } set { disableAnimator = value; } }
        public bool DisableAudioExtras { get { return disableAudioExtras; } set { disableAudioExtras = value; } }
        public bool DisableObjectBob { get { return disableObjectBob; } set { disableObjectBob = value; } }

        private static GoldPlayerProjectSettings instance;
        public static GoldPlayerProjectSettings Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = GetOrCreate();
                return instance;
            }
        }

        [InitializeOnLoadMethod]
        internal static void OnLoad()
        {
            // Adds back defines if they were manually removed.
            ApplyDefines(GetOrCreate());
        }

        public static GoldPlayerProjectSettings GetOrCreate()
        {
            GoldPlayerProjectSettings settings;

            string oldLocation = DIRECTORY + "/GoldPlayerSettings.json";

            // Backwards compatibility.
            if (File.Exists(oldLocation))
            {
                OldJsonSettings oldSettings = JsonUtility.FromJson<OldJsonSettings>(File.ReadAllText(oldLocation));
                settings = CreateNewSettings();
                settings.disableInteraction = oldSettings.disableInteraction;
                settings.disableUI = oldSettings.disableUI;
                settings.disableGraphics = oldSettings.disableGraphics;
                settings.disableAnimator = oldSettings.disableAnimator;
                settings.disableAudioExtras = oldSettings.disableAudioExtras;
                settings.disableObjectBob = oldSettings.disableObjectBob;
                RemoveFile(oldLocation);
                SaveInstance(settings);
                Debug.Log("Gold Player :: Found old settings. Upgrading your settings.");
                return settings;
            }

            if (!File.Exists(PATH))
            {
                settings = CreateNewSettings();
            }
            else
            {
                settings = LoadSettings();

                if (settings == null)
                {
                    RemoveFile(PATH);
                    settings = CreateNewSettings();
                }
            }

            settings.hideFlags = HideFlags.HideAndDontSave;

            return settings;
        }

        private static GoldPlayerProjectSettings CreateNewSettings()
        {
            GoldPlayerProjectSettings settings = CreateInstance<GoldPlayerProjectSettings>();
            SaveInstance(settings);
            return settings;
        }

        private static GoldPlayerProjectSettings LoadSettings()
        {
            GoldPlayerProjectSettings settings;

            try
            {
                settings = (GoldPlayerProjectSettings)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(PATH)[0];
            }
            catch (Exception)
            {
                Debug.Log("Could not load Gold Player settings. Settings will be reset.");
                settings = null;
            }

            return settings;
        }

        public static void Save()
        {
            SaveInstance(Instance);
        }

        private static void SaveInstance(GoldPlayerProjectSettings settings)
        {
            if (!Directory.Exists(DIRECTORY))
            {
                Directory.CreateDirectory(DIRECTORY);
            }

            try
            {
                UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new UnityEngine.Object[] { settings }, PATH, true);
            }
            catch (Exception ex)
            {
                Debug.LogError("Can't save Gold Player settings!\n" + ex);
            }
        }

        public static void ApplyDefines(GoldPlayerProjectSettings settings)
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

            GoldPlayerScriptHelpers.AddAndRemoveDefines(add, remove);
            Save();
        }

        private static void RemoveFile(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            FileAttributes attributes = File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.ReadOnly))
            {
                File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
            }

            File.Delete(path);
        }
    }
}
#endif
