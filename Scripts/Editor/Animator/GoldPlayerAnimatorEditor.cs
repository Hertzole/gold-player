#if GOLD_PLAYER_DISABLE_ANIMATOR
#define OBSOLETE
#endif
#if UNITY_2019_1_OR_NEWER
// Currently disabled because IMGUI offers more functionality with dealing with prefabs.
//#define USE_UI_ELEMENTS 
#endif
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
#if USE_UI_ELEMENTS
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace Hertzole.GoldPlayer.Editor
{
#pragma warning disable CS0618 // Type or member is obsolete
    [CustomEditor(typeof(GoldPlayerAnimator))]
    public class GoldPlayerAnimatorEditor : UnityEditor.Editor
    {
#if USE_UI_ELEMENTS
        private List<string> parameters = new List<string>();
        private List<int> parametersIndex = new List<int>();
#else
#if !OBSOLETE
        private bool gotParameters = false;
#endif
        private Object oldAnimator;
        private GUIContent[] parameters;
        private int[] optionValues;
#endif

        private SerializedProperty animator;
        private SerializedProperty maxSpeed;
        private SerializedProperty valueSmoothTime;
        private SerializedProperty lookAngleHead;
        private SerializedProperty moveX;
        private SerializedProperty moveY;
        private SerializedProperty crouching;
        private SerializedProperty lookAngle;
        
        private static readonly GUIContent[] noParameterContent = new GUIContent[] { new GUIContent("None") };
        private static readonly int[] noParameterValues = new int[] { 0 };

        private void OnEnable()
        {
            animator = serializedObject.FindProperty("animator");
            maxSpeed = serializedObject.FindProperty("maxSpeed");
            valueSmoothTime = serializedObject.FindProperty("valueSmoothTime");
            lookAngleHead = serializedObject.FindProperty("lookAngleHead");
            moveX = serializedObject.FindProperty("moveX");
            moveY = serializedObject.FindProperty("moveY");
            crouching = serializedObject.FindProperty("crouching");
            lookAngle = serializedObject.FindProperty("lookAngle");

#if !USE_UI_ELEMENTS
            oldAnimator = animator.objectReferenceValue;
#endif

            GetAnimatorParameters(animator.objectReferenceValue);
        }

        private void GetAnimatorParameters(Object target)
        {
            Animator anim = target as Animator;
            AnimatorController controller = anim == null ? null : anim.runtimeAnimatorController as AnimatorController;
#if USE_UI_ELEMENTS
            parameters.Clear();
            parametersIndex.Clear();
#endif

            if (controller != null)
            {
#if !USE_UI_ELEMENTS
                parameters = new GUIContent[controller.parameters.Length];
                optionValues = new int[controller.parameters.Length];
#endif
                for (int i = 0; i < controller.parameters.Length; i++)
                {
#if USE_UI_ELEMENTS
                    parameters.Add(anim.GetParameter(i).name);
                    parametersIndex.Add(i);
#else
                    parameters[i] = new GUIContent(controller.parameters[i].name);
                    optionValues[i] = i;
#endif
                }
            }
            else
            {
#if USE_UI_ELEMENTS
                parameters.Add("None");
                parametersIndex.Add(0);
#else
                parameters = noParameterContent;
                optionValues = noParameterValues;
#endif

                moveX.FindPropertyRelative("index").intValue = 0;
                moveY.FindPropertyRelative("index").intValue = 0;
                crouching.FindPropertyRelative("index").intValue = 0;
                lookAngle.FindPropertyRelative("index").intValue = 0;
                serializedObject.ApplyModifiedProperties();
            }

#if !USE_UI_ELEMENTS
#if !OBSOLETE
            gotParameters = true;
#endif
#endif
        }

#if USE_UI_ELEMENTS
        private VisualElement moveXField;
        private VisualElement moveYField;
        private VisualElement crouchingField;
        private VisualElement lookAngleField;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement()
            {
                name = "Root"
            };

            //PropertyField animatorField = new PropertyField(animator);
            ObjectField animatorField = new ObjectField(animator.displayName)
            {
                objectType = typeof(Animator)
            };
            animatorField.BindProperty(animator);
            animatorField.RegisterValueChangedCallback(x =>
            {
                ToggleParameters(x.newValue != null);
                GetAnimatorParameters(x.newValue);
            });

            moveXField = GetParameterField(moveX);
            moveYField = GetParameterField(moveY);
            crouchingField = GetParameterField(crouching);
            lookAngleField = GetParameterField(lookAngle);

            root.Add(animatorField);
            root.Add(new PropertyField(maxSpeed));
            root.Add(new PropertyField(valueSmoothTime));
            root.Add(new PropertyField(lookAngleHead));

            root.Add(GoldPlayerUIHelper.GetSpace());

            root.Add(GoldPlayerUIHelper.GetHeaderLabel("Parameters"));

            root.Add(moveXField);
            root.Add(moveYField);
            root.Add(crouchingField);
            root.Add(lookAngleField);

            ToggleParameters(animator.objectReferenceValue != null);

            return root;
        }

        private void ToggleParameters(bool toggle)
        {
            moveXField.SetEnabled(toggle);
            moveYField.SetEnabled(toggle);
            crouchingField.SetEnabled(toggle);
            lookAngleField.SetEnabled(toggle);
        }

        private VisualElement GetParameterField(SerializedProperty property)
        {
            SerializedProperty index = property.FindPropertyRelative("index");

            if (index.intValue >= parametersIndex.Count)
            {
                index.intValue = parametersIndex.Count - 1;
                serializedObject.ApplyModifiedProperties();
            }

            PropertyField propField = new PropertyField(property);

            propField.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                propField.Q<Foldout>().style.display = DisplayStyle.None;
            });

            Label label = new Label(property.displayName);
            propField.Add(label);

            //PopupField<int> popupField = new PopupField<int>(property.displayName, parametersIndex, index.intValue, FormatItem, FormatItem);
            //popupField.BindProperty(index);
            //popupField.Q<VisualElement>(className: "unity-popup-field__input").SetEnabled(property.FindPropertyRelative("enabled").boolValue);

            //Toggle enableToggle = new Toggle(string.Empty);
            //popupField.Insert(1, enableToggle);
            //enableToggle.BindProperty(property.FindPropertyRelative("enabled"));
            //enableToggle.RegisterValueChangedCallback(x =>
            //{
            //    popupField.Q<VisualElement>(className: "unity-popup-field__input").SetEnabled(x.newValue);
            //});

            return propField;
        }

        private string FormatItem(int index)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return "None";
            }
            else if (parameters.Count == 1)
            {
                return parameters[0];
            }
            else
            {
                return parameters[index];
            }
        }
#else
        public override void OnInspectorGUI()
        {
#if !OBSOLETE
            serializedObject.Update();

            if (oldAnimator != animator.objectReferenceValue)
            {
                oldAnimator = animator.objectReferenceValue;
                gotParameters = false;
            }

            if (!gotParameters)
            {
                GetAnimatorParameters(animator.objectReferenceValue);
            }

            EditorGUILayout.PropertyField(animator);

            if (animator.objectReferenceValue != null && ((Animator)animator.objectReferenceValue).runtimeAnimatorController == null)
            {
                EditorGUILayout.HelpBox("Your animator does not have a controller assigned.", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(maxSpeed);
            EditorGUILayout.PropertyField(valueSmoothTime);

            EditorGUILayout.PropertyField(lookAngleHead);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);

            bool oEnabled = GUI.enabled;

            GUI.enabled = animator.objectReferenceValue != null && ((Animator)animator.objectReferenceValue).runtimeAnimatorController != null;

            PopupField(moveX);
            PopupField(moveY);
            PopupField(crouching);
            PopupField(lookAngle);

            GUI.enabled = oEnabled;

            serializedObject.ApplyModifiedProperties();
#else
            if (GUILayout.Button("Remove Component"))
            {
                Undo.DestroyObjectImmediate((GoldPlayerAnimator)target);
            }
#endif
        }

        private void PopupField(SerializedProperty property)
        {
            Rect rect = EditorGUILayout.GetControlRect();

            rect.width = EditorGUIUtility.labelWidth;

            EditorGUI.PrefixLabel(rect, new GUIContent(property.displayName), property.prefabOverride ? EditorStyles.boldLabel : GUI.skin.label);

            rect.width = EditorGUIUtility.singleLineHeight;
            rect.x = EditorGUIUtility.labelWidth + EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("enabled"), GUIContent.none);

            rect.width = EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - (EditorGUIUtility.singleLineHeight * 2) - 5;
            rect.x = EditorGUIUtility.labelWidth + (EditorGUIUtility.singleLineHeight * 2);

            bool oEnabled = GUI.enabled;

            if (GoldPlayerUIHelper.ShouldDisableElements(property.FindPropertyRelative("enabled")))
            {
                GUI.enabled = false;
            }

            EditorGUI.IntPopup(rect, property.FindPropertyRelative("index"), parameters, optionValues, GUIContent.none);
            GUI.enabled = oEnabled;
        }
#pragma warning restore CS0618 // Type or member is obsolete
#endif
    }
}
