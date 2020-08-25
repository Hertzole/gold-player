#if !GOLD_PLAYER_DISABLE_ANIMATOR
using UnityEditor;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace Hertzole.GoldPlayer.Editor
{
    [CustomEditor(typeof(GoldPlayerAnimator))]
    public class GoldPlayerAnimatorEditor : UnityEditor.Editor
    {
#if UNITY_2019_1_OR_NEWER
        private List<string> parameters = new List<string>();
        private List<int> parametersIndex = new List<int>();
#else
        private bool gotParameters = false;
        private Object oldAnimator;
        private GUIContent[] parameters;
        private int[] optionValues;
#endif

        private SerializedProperty animator;
        private SerializedProperty maxSpeed;
        private SerializedProperty valueSmooth;
        private SerializedProperty moveX;
        private SerializedProperty moveY;

        private void OnEnable()
        {
            animator = serializedObject.FindProperty("animator");
            maxSpeed = serializedObject.FindProperty("maxSpeed");
            valueSmooth = serializedObject.FindProperty("valueSmooth");
            moveX = serializedObject.FindProperty("moveX");
            moveY = serializedObject.FindProperty("moveY");

#if !UNITY_2019_1_OR_NEWER
            oldAnimator = animator.objectReferenceValue;
#endif

            GetAnimatorParameters();
        }

        private void GetAnimatorParameters()
        {
#if UNITY_2019_1_OR_NEWER
            parameters.Clear();
            parametersIndex.Clear();

            if (animator.objectReferenceValue != null)
            {
                Animator a = (Animator)animator.objectReferenceValue;
                for (int i = 0; i < a.parameterCount; i++)
                {
                    parameters.Add(a.GetParameter(i).name);
                    parametersIndex.Add(i);
                }
            }
            else
            {
                parameters.Add("None");
                parametersIndex.Add(0);
            }
#else
            if (animator.objectReferenceValue != null)
            {
                Animator a = (Animator)animator.objectReferenceValue;

                parameters = new GUIContent[a.parameterCount];
                optionValues = new int[a.parameterCount];
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameters[i] = new GUIContent(a.GetParameter(i).name);
                    optionValues[i] = i;
                }
            }
            else
            {
                parameters = new GUIContent[1] { new GUIContent("None") };
                optionValues = new int[1] { 0 };
                moveX.intValue = 0;
                moveY.intValue = 0;
            }

            gotParameters = true;
#endif
        }


#if UNITY_2019_1_OR_NEWER
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement()
            {
                name = "Root"
            };
            root.Add(new PropertyField(animator));
            root.Add(new PropertyField(maxSpeed));
            root.Add(new PropertyField(valueSmooth));

            root.Add(GoldPlayerUIHelper.GetSpace());

            root.Add(GoldPlayerUIHelper.GetHeaderLabel("Parameters"));

            root.Add(GetParameterField(moveX));
            root.Add(GetParameterField(moveY));

            return root;
        }

        private VisualElement GetParameterField(SerializedProperty property)
        {
            PopupField<int> popupField = new PopupField<int>(property.displayName, parametersIndex, property.intValue, FormatItem, FormatList);
            popupField.Bind(serializedObject);
            popupField.bindingPath = property.propertyPath;

            //Label fieldLabel = popupField.Q<Label>(className: "unity-base-field__label");
            //if (fieldLabel != null)
            //{
            //    fieldLabel.userData = property.Copy();
            //    fieldLabel.RegisterCallback<MouseUpEvent>(RightClickMenuEvent);
            //}

            return popupField;
        }

        //private void RightClickMenuEvent(MouseUpEvent evt)
        //{
        //    if(evt.button != (int)MouseButton.RightMouse)
        //        return;

        //    var label = evt.target as Label;
        //    if (label == null)
        //        return;

        //    var property = label.userData as SerializedProperty;
        //    if (property == null)
        //        return;

        //    var menu = EditorGUI.FillPropertyContextMenu(property);
        //    var menuPosition = new Vector2(label.layout.xMin, label.layout.height);
        //    menuPosition = label.LocalToWorld(menuPosition);
        //    var menuRect = new Rect(menuPosition, Vector2.zero);
        //    menu.DropDown(menuRect);

        //    evt.PreventDefault();
        //    evt.StopPropagation();
        //}

        private string FormatItem(int index)
        {
            return parameters[index];
        }

        private string FormatList(int index)
        {
            return parameters[index];
        }
#else
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (oldAnimator != animator.objectReferenceValue)
            {
                oldAnimator = animator.objectReferenceValue;
                gotParameters = false;
            }

            if (!gotParameters)
            {
                GetAnimatorParameters();
            }

            EditorGUILayout.PropertyField(animator);

            EditorGUILayout.PropertyField(maxSpeed);
            EditorGUILayout.PropertyField(valueSmooth);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);

            bool oEnabled = GUI.enabled;

            GUI.enabled = animator.objectReferenceValue != null;
            EditorGUILayout.IntPopup(moveX, parameters, optionValues);
            EditorGUILayout.IntPopup(moveY, parameters, optionValues);

            GUI.enabled = oEnabled;

            serializedObject.ApplyModifiedProperties();
        }
#endif
    }
}
#endif
