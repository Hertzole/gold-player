#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT // Mark this as obsolete if the new input system is enabled.
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR // If it's obsolete and not in the editor, remove it.
#define STRIP
#endif

#if !STRIP
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer
{
#if !OBSOLETE
    [AddComponentMenu("Gold Player/Gold Player Input", 1)]
    [DisallowMultipleComponent]
#else
    [System.Obsolete("You're using the new Input System so this component will be useless.")]
    [AddComponentMenu("")]
#endif
    public class GoldPlayerInput : MonoBehaviour, IGoldInput
    {
        [SerializeField]
        [Tooltip("Determines if the input should be based around KeyCodes. If false, Input Manager will be used.")]
        [FormerlySerializedAs("m_UseKeyCodes")]
        private bool useKeyCodes = true;

        [Space]

        [SerializeField]
        [Tooltip("All the available inputs.")]
        [FormerlySerializedAs("m_Inputs")]
        private InputItem[] inputs = new InputItem[]
        {
            new InputItem("Horizontal", "Horizontal", KeyCode.None),
            new InputItem("Vertical", "Vertical", KeyCode.None),
            new InputItem("Mouse X", "Mouse X", KeyCode.None),
            new InputItem("Mouse Y" , "Mouse Y" , KeyCode.None),
            new InputItem("Jump", "Jump", KeyCode.Space),
            new InputItem("Crouch", "Crouch", KeyCode.C),
            new InputItem("Run", "Run", KeyCode.LeftShift),
#if !GOLD_PLAYER_DISABLE_INTERACTION
            new InputItem("Interact", "Interact", KeyCode.E),
#endif
        };

        private Dictionary<string, InputItem> inputsDic;

        /// <summary> Determines if the input should be based around KeyCodes. If false, Input Manager will be used. </summary>
        public bool UseKeyCodes { get { return useKeyCodes; } set { useKeyCodes = value; } }
        /// <summary> All the available inputs. </summary>
        public InputItem[] Inputs { get { return inputs; } set { inputs = value; UpdateInputs(); } }

        private void Start()
        {
#if OBSOLETE
            Debug.LogError(gameObject.name + " has GoldPlayerInput attached. It does not work when the new input system is enabled.");
#else
            UpdateInputs();
#endif
        }

        public void UpdateInputs()
        {
            inputsDic = new Dictionary<string, InputItem>();
            inputsDic.Clear();

            for (int i = 0; i < inputs.Length; i++)
            {
                inputsDic.Add(inputs[i].ButtonName, inputs[i]);
            }
        }

        /// <summary>
        /// Returns true while a button is being held down.
        /// </summary>
        /// <param name="buttonName">The button to check.</param>
        public bool GetButton(string buttonName)
        {
#if !OBSOLETE
            if (inputsDic == null)
            {
                UpdateInputs();
            }

            return useKeyCodes ? Input.GetKey(inputsDic[buttonName].Key) : Input.GetButton(inputsDic[buttonName].InputName);
#else
            return false;
#endif
        }

        /// <summary>
        /// Returns true if the button was pressed this frame.
        /// </summary>
        /// <param name="buttonName">The button to check.</param>
        public bool GetButtonDown(string buttonName)
        {
#if !OBSOLETE
            if (inputsDic == null)
            {
                UpdateInputs();
            }

            return useKeyCodes ? Input.GetKeyDown(inputsDic[buttonName].Key) : Input.GetButtonDown(inputsDic[buttonName].InputName);
#else
            return false;
#endif
        }

        /// <summary>
        /// Returns true if the button was released this frame.
        /// </summary>
        /// <param name="buttonName">The button to check.</param>
        public bool GetButtonUp(string buttonName)
        {
#if !OBSOLETE
            if (inputsDic == null)
            {
                UpdateInputs();
            }

            return useKeyCodes ? Input.GetKeyUp(inputsDic[buttonName].Key) : Input.GetButtonUp(inputsDic[buttonName].InputName);
#else
            return false;
#endif
        }

        /// <summary>
        /// Returns the value of a axis.
        /// </summary>
        /// <param name="axisName">The axis to check.</param>
        public float GetAxis(string axisName)
        {
#if !OBSOLETE
            if (inputsDic == null)
            {
                UpdateInputs();
            }

            return Input.GetAxis(inputsDic[axisName].InputName);
#else
            return 0;
#endif
        }

        /// <summary>
        /// Returns the value of a axis with no processing applied.
        /// </summary>
        /// <param name="axisName">The axis to check.</param>
        public float GetAxisRaw(string axisName)
        {
#if !OBSOLETE
            if (inputsDic == null)
            {
                UpdateInputs();
            }

            return Input.GetAxisRaw(inputsDic[axisName].InputName);
#else
            return 0;
#endif
        }

        /// <summary>
        /// Not used on GoldPlayerInput.
        /// </summary>
        [System.Obsolete("GetVector2 will do nothing when using the legacy input manager.")]
        public Vector2 GetVector2(string action)
        {
            Debug.LogWarning("GetVector2 will do nothing when using the legacy input manager.");
            return Vector2.zero;
        }

        /// <summary>
        /// Returns the Input Item that matches the buttonName in the given InputItem array.
        /// </summary>
        /// <param name="buttonName">The name of the item to try and find.</param>
        /// <param name="inputsArray">The array to search in to find the item.</param>
        protected virtual InputItem GetItem(string buttonName, InputItem[] inputsArray)
        {
            for (int i = 0; i < inputsArray.Length; i++)
            {
                if (inputsArray[i].ButtonName == buttonName)
                {
                    return inputsArray[i];
                }
            }

            Debug.LogError("No input with the name '" + buttonName + "' assigned on '" + gameObject.name + "'!");
            return new InputItem();
        }
    }
}
#endif
