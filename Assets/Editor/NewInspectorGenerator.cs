#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Hertzole.HertzLib.Editor
{
    /// <summary>
    /// Based on https://gist.github.com/mandarinx/26355b1811314cc9812b42701496cbe0
    /// and https://bitbucket.org/igjuricic/unity-custom-editor-generator
    /// </summary>

    public class NewInspectorGenerator
    {
        [MenuItem("Assets/Create/C# Inspector Editor", false, 50)]
        public static void GenerateCustomEditorFromSelectedScript()
        {
            if (!GenerateCustomEditorFromSelectedScript_Validator())
            {
                return;
            }

            MonoScript scriptAsset = Selection.objects[0] as MonoScript;
            Type scriptClass = scriptAsset.GetClass();
            string editorClass = scriptClass.Name + "Editor";

            if (Type.GetType(editorClass) != null)
            {
                if (!EditorUtility.DisplayDialog("Create Script Editor?",
                    "Editor for class \"" + scriptClass.Name + "\" already exists. Are you sure you want to overwrite it?", "Create", "Cancel"))
                {
                    return;
                }
            }

            string scriptDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(scriptAsset));
            string editorDir = scriptDir + Path.DirectorySeparatorChar + "Editor";
            string editorPath = editorDir + Path.DirectorySeparatorChar + scriptClass.Name + "Editor.cs";

            if (!AssetDatabase.IsValidFolder(editorDir))
            {
                AssetDatabase.CreateFolder(scriptDir, "Editor");
            }

            string editorContents = GenerateScriptEditor(scriptClass);

            System.IO.File.WriteAllText(editorPath, editorContents);

            AssetImporter.GetAtPath(editorPath);
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Create/C# Inspector Editor", true, 50)]
        public static bool GenerateCustomEditorFromSelectedScript_Validator()
        {
            if (Selection.objects != null &&
                Selection.objects.Length == 1 &&
                Selection.objects[0] is MonoScript &&
                (Selection.objects[0] as MonoScript).GetClass() != null &&
                !(Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(UnityEditor.Editor)) &&
                (
                    (Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(ScriptableObject)) ||
                    (Selection.objects[0] as MonoScript).GetClass().IsSubclassOf(typeof(MonoBehaviour))
                )
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        static string GenerateScriptEditor(Type type)
        {
            StringBuilder builder = new StringBuilder();

            string editorClass = type.Name + "Editor";

            FieldInfo[] fields = GetSerializableFields(type);

            string classNamespace = string.Empty;
            string indent = string.Empty;
            if (!string.IsNullOrEmpty(type.Namespace))
            {
                classNamespace = type.Namespace;
                indent = "\t";
            }

            builder.AppendLine("#if UNITY_EDITOR");
            builder.AppendLine("using UnityEditor;");
            builder.AppendLine();

            if (!string.IsNullOrEmpty(classNamespace))
            {
                builder.AppendLine(String.Format("namespace {0}.Editor", type.Namespace));
                builder.AppendLine("{");
            }

            builder.AppendLine(String.Format("{1}[CustomEditor(typeof({0}))]", type.Name, indent));
            builder.AppendLine(String.Format("{1}public class {0} : UnityEditor.Editor", editorClass, indent));
            builder.AppendLine(indent + "{");

            foreach (FieldInfo field in fields)
            {
                builder.AppendLine(String.Format("\t{1}private SerializedProperty {0};", field.Name, indent));
            }

            // FUNCTION: OnEnable()
            builder.AppendLine();

            builder.AppendLine(indent + "\t// Get all the serialized properties from the target script.");
            builder.AppendLine(indent + "\tprivate void OnEnable()");
            builder.AppendLine(indent + "\t{");

            foreach (FieldInfo field in fields)
            {
                builder.AppendLine(String.Format("\t\t{1}{0} = serializedObject.FindProperty(\"{0}\");", field.Name, indent));
            }

            builder.AppendLine(indent + "\t}");
            // END FUNCTION

            // FUNCTION: OnInspectorGUI()
            builder.AppendLine();

            builder.AppendLine(indent + "\t// Draw all the GUI in the inspector.");
            builder.AppendLine(indent + "\tpublic override void OnInspectorGUI()");
            builder.AppendLine(indent + "\t{");
            builder.AppendLine(indent + "\t\tserializedObject.Update();");
            builder.AppendLine();

            foreach (FieldInfo field in fields)
            {
                builder.AppendLine(string.Format("{1}\t\tEditorGUILayout.PropertyField({0}, true);", field.Name, indent));
            }

            builder.AppendLine();
            builder.AppendLine(indent + "\t\tserializedObject.ApplyModifiedProperties();");

            builder.AppendLine(indent + "\t}");
            // END FUNCTION: OnInspectorGUI

            builder.AppendLine(indent + "}");
            // END CLASS

            if (!string.IsNullOrEmpty(classNamespace))
                builder.AppendLine("}");

            builder.AppendLine("#endif");

            return builder.ToString();
        }

        private static FieldInfo[] GetSerializableFields(Type type)
        {
            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(f => IsSerializable(f) && !f.IsInitOnly).ToArray();
        }

        private static bool IsSerializable(FieldInfo field)
        {
            return field.IsPublic || field.IsDefined(typeof(SerializeField), false);
        }
    }
}
#endif
