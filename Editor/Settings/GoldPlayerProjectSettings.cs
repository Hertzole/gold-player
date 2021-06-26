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

        private const string DISABLE_INTERACTION = "GOLD_PLAYER_DISABLE_INTERACTION";
        private const string DISABLE_UI = "GOLD_PLAYER_DISABLE_UI";
        private const string DISABLE_GRAPHICS = "GOLD_PLAYER_DISABLE_GRAPHICS";
        private const string DISABLE_ANIMATOR = "GOLD_PLAYER_DISABLE_ANIMATOR";
        private const string DISABLE_AUDIO_EXTRAS = "GOLD_PLAYER_DISABLE_AUDIO_EXTRAS";
        private const string DISABLE_OBJECT_BOB = "GOLD_PLAYER_DISABLE_OBJECT_BOB";
        private const string DISABLE_OPTIMIZATIONS = "GOLD_PLAYER_DISABLE_OPTIMIZATIONS";

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
        [SerializeField]
        private bool disableOptimizations = false;

        public EditorGUIAdaption GUIAdapation { get { return guiAdapation; } set { guiAdapation = value; Save(); } }

        public bool ShowGroundCheckGizmos { get { return showGroundCheckGizmos; } set { showGroundCheckGizmos = value; Save(); } }

        public bool DisableInteraction { get { return disableInteraction; } set { disableInteraction = value; } }
        public bool DisableUI { get { return disableUI; } set { disableUI = value; } }
        public bool DisableGraphics { get { return disableGraphics; } set { disableGraphics = value; } }
        public bool DisableAnimator { get { return disableAnimator; } set { disableAnimator = value; } }
        public bool DisableAudioExtras { get { return disableAudioExtras; } set { disableAudioExtras = value; } }
        public bool DisableObjectBob { get { return disableObjectBob; } set { disableObjectBob = value; } }
        public bool DisableOptimizations { get { return disableOptimizations; } set { disableOptimizations = value; } }

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
                add.Add(DISABLE_INTERACTION);
            }
            else
            {
                remove.Add(DISABLE_INTERACTION);
            }

            if (settings.disableUI)
            {
                add.Add(DISABLE_UI);
            }
            else
            {
                remove.Add(DISABLE_UI);
            }

            if (settings.disableGraphics)
            {
                add.Add(DISABLE_GRAPHICS);
            }
            else
            {
                remove.Add(DISABLE_GRAPHICS);
            }

            if (settings.disableAnimator)
            {
                add.Add(DISABLE_ANIMATOR);
            }
            else
            {
                remove.Add(DISABLE_ANIMATOR);
            }

            if (settings.disableAudioExtras)
            {
                add.Add(DISABLE_AUDIO_EXTRAS);
            }
            else
            {
                remove.Add(DISABLE_AUDIO_EXTRAS);
            }

            if (settings.disableObjectBob)
            {
                add.Add(DISABLE_OBJECT_BOB);
            }
            else
            {
                remove.Add(DISABLE_OBJECT_BOB);
            }

            if (settings.disableOptimizations)
            {
                add.Add(DISABLE_OPTIMIZATIONS);
            }
            else
            {
                remove.Add(DISABLE_OPTIMIZATIONS);
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
