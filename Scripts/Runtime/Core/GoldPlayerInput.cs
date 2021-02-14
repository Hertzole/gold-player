#if ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT && !ENABLE_LEGACY_INPUT_MANAGER  // Mark this as obsolete if the new input system is enabled.
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR // If it's obsolete and not in the editor, remove it.
#define STRIP
#endif

#if DEBUG || UNITY_EDITOR || !GOLD_PLAYER_STRIP_SAFETY
#define USE_SAFETY
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
        [SerializeField]
        [Tooltip("If true, all actions will be enabled in OnEnable.")]
        private bool autoEnableInput = true;
        [SerializeField]
        [Tooltip("If true, all actions will be disabled in OnDisable.")]
        private bool autoDisableInput = true;

        [Space]

        [SerializeField]
        [Tooltip("All the available inputs.")]
        [FormerlySerializedAs("m_Inputs")]
        private InputItem[] inputs = new InputItem[]
        {
            new InputItem("Move", "Horizontal", "Vertical"),
            new InputItem("Look", "Mouse X", "Mouse Y"),
            new InputItem("Jump", "Jump", KeyCode.Space),
            new InputItem("Crouch", "Crouch", KeyCode.C),
            new InputItem("Run", "Run", KeyCode.LeftShift),
#if !GOLD_PLAYER_DISABLE_INTERACTION
            new InputItem("Interact", "Interact", KeyCode.E),
#endif
        };

        private Dictionary<string, InputItem> inputsDic;
        private Dictionary<string, bool> enabledInputs;

        /// <summary> Determines if the input should be based around KeyCodes. If false, Input Manager will be used. </summary>
        public bool UseKeyCodes { get { return useKeyCodes; } set { useKeyCodes = value; } }
        /// <summary> If true, all actions will be enabled on start. </summary>
        public bool AutoEnableInput { get { return autoEnableInput; } set { autoEnableInput = value; } }
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

#if !OBSOLETE
        private void OnEnable()
        {
            if (autoEnableInput)
            {
                EnableInput();
            }
        }

        private void OnDisable()
        {
            if (autoDisableInput)
            {
                DisableInput();
            }
        }
#endif

        public void UpdateInputs()
        {
            inputsDic = new Dictionary<string, InputItem>();
            inputsDic.Clear();
            enabledInputs = new Dictionary<string, bool>();

            for (int i = 0; i < inputs.Length; i++)
            {
                inputsDic.Add(inputs[i].ButtonName, inputs[i]);
                enabledInputs.Add(inputs[i].ButtonName, false);
            }
        }

        public void EnableInput()
        {
#if !OBSOLETE
            if (inputsDic == null)
            {
                UpdateInputs();
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                EnableAction(inputs[i].ButtonName);
            }
#else
            Debug.LogError("GoldPlayerInput is obsolete. DisableInput will do nothing.");
#endif
        }

        public void DisableInput()
        {
#if !OBSOLETE
            if (inputsDic == null)
            {
                return;
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                DisableAction(inputs[i].ButtonName);
            }
#else
            Debug.LogError("GoldPlayerInput is obsolete. DisableInput will do nothing.");
#endif
        }

        public void EnableAction(string action)
        {
#if !OBSOLETE
            if (!enabledInputs.ContainsKey(action))
            {
                Debug.LogError("There's no action called '" + action + "' on " + gameObject.name + ".");
                return;
            }

            enabledInputs[action] = true;
#else
            Debug.LogError("GoldPlayerInput is obsolete. DisableInput will do nothing.");
#endif
        }

        public void DisableAction(string action)
        {
#if !OBSOLETE
            if (!enabledInputs.ContainsKey(action))
            {
                Debug.LogError("There's no action called '" + action + "' on " + gameObject.name + ".");
                return;
            }

            enabledInputs[action] = false;
#else
            Debug.LogError("GoldPlayerInput is obsolete. DisableInput will do nothing.");
#endif
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

#if USE_SAFETY
            if (!IsValidInput(buttonName, InputItem.InputType.Button))
            {
                return false;
            }
#endif

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

#if USE_SAFETY
            if (!IsValidInput(buttonName, InputItem.InputType.Button))
            {
                return false;
            }
#endif

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

#if USE_SAFETY
            if (!IsValidInput(buttonName, InputItem.InputType.Button))
            {
                return false;
            }
#endif

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

#if USE_SAFETY
            if (!IsValidInput(axisName, InputItem.InputType.Axis))
            {
                return 0;
            }
#endif

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

#if USE_SAFETY
            if (!IsValidInput(axisName, InputItem.InputType.Axis))
            {
                return 0;
            }
#endif

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
#if !OBSOLETE
            if (inputsDic == null)
            {
                UpdateInputs();
            }

#if USE_SAFETY
            if (!IsValidInput(action, InputItem.InputType.Vector2))
            {
                return Vector2.zero;
            }
#endif

            return new Vector2(Input.GetAxisRaw(inputsDic[action].InputName), Input.GetAxisRaw(inputsDic[action].InputNameSecondary));
#else
            return Vector2.zero;
#endif
        }

#if USE_SAFETY
        private bool IsValidInput(string action, InputItem.InputType type)
        {
            if (!inputsDic.ContainsKey(action))
            {
                Debug.LogError("There's no input action called '" + action + "' on '" + gameObject.name + "'.", gameObject);
                return false;
            }

            if (inputsDic[action].Type != type)
            {
                Debug.LogError("Input action '" + action + "' is a " + inputsDic[action].Type + " but should be a " + type + ".");
                return false;
            }

            return true;
        }
#endif

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
