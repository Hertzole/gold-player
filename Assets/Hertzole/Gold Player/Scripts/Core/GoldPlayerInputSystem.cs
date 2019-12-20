using Hertzole.GoldPlayer.Core;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace Hertzole.GoldPlayer
{
#if !ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
    [System.Obsolete("You're not using the new Input System so this component will be useless.")]
#else
    [AddComponentMenu("Gold Player/Gold Player Input System", 02)]
    [DisallowMultipleComponent]
#endif
    public class GoldPlayerInputSystem : MonoBehaviour, IGoldInput
    {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
        [SerializeField]
        private InputActionAsset input = null;
#endif
        [SerializeField]
        private bool autoEnableInput = true;
        [SerializeField]
        private bool autoDisableInput = true;

#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
        public InputActionAsset Input { get { return input; } set { input = value; } }
#endif

        public bool AutoEnableInput { get { return autoEnableInput; } set { autoEnableInput = value; } }
        public bool AutoDisableInput { get { return autoDisableInput; } set { autoDisableInput = value; } }

        private Dictionary<string, InputAction> actions;

        private void Start()
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            UpdateActions();
#endif
        }

        public void EnableInput()
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            if (input != null)
            {
                input.Enable();
            }
            else
            {
                Debug.LogWarning("There's no input asset on " + gameObject.name + " to enable.", gameObject);
            }
#endif
        }

        public void DisableInput()
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            if (input != null)
            {
                input.Disable();
            }
            else
            {
                Debug.LogWarning("There's no input asset on " + gameObject.name + " to disable.", gameObject);
            }
#endif
        }

#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
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

        private void UpdateActions()
        {
            if (input == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return;
            }

            actions = new Dictionary<string, InputAction>();

            foreach (InputActionMap item in input.actionMaps)
            {
                foreach (InputAction action in item.actions)
                {
                    actions.Add(item.name + "/" + action.name, action);
                }
            }
        }

        public bool GetButton(string buttonName)
        {
            if (input == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return false;
            }

            if (actions == null)
            {
                UpdateActions();
            }

            if (actions.TryGetValue(buttonName, out InputAction inputAction))
            {
                return inputAction.activeControl is ButtonControl button && button.isPressed;
            }
            else
            {
                Debug.LogError("Can't find action '" + buttonName + "' in " + input.name + "!");
                return false;
            }
        }

        public bool GetButtonDown(string buttonName)
        {
            if (input == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return false;
            }

            if (actions == null)
            {
                UpdateActions();
            }

            if (actions.TryGetValue(buttonName, out InputAction inputAction))
            {
                return inputAction.activeControl is ButtonControl button && button.wasPressedThisFrame;
            }
            else
            {
                Debug.LogError("Can't find action '" + buttonName + "' in " + input.name + "!");
                return false;
            }
        }

        public bool GetButtonUp(string buttonName)
        {
            if (input == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return false;
            }

            if (actions == null)
            {
                UpdateActions();
            }

            if (actions.TryGetValue(buttonName, out InputAction inputAction))
            {
                return inputAction.activeControl is ButtonControl button && button.wasReleasedThisFrame;
            }
            else
            {
                Debug.LogError("Can't find action '" + buttonName + "' in " + input.name + "!");
                return false;
            }
        }

        public float GetAxis(string axisName)
        {
            if (input == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return 0;
            }

            if (actions.TryGetValue(axisName, out InputAction inputAction))
            {
                if (inputAction.activeControl is AxisControl axis)
                {
                    return axis.ReadValue();
                }
                else
                {
                    Debug.LogError(axisName + " is not an axis type.");
                    return 0;
                }
            }
            else
            {
                Debug.LogError("Can't find action '" + axisName + "' in " + input.name + "!");
                return 0;
            }
        }

        public float GetAxisRaw(string axisName)
        {
            if (input == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return 0;
            }

            if (actions.TryGetValue(axisName, out InputAction inputAction))
            {
                if (inputAction.activeControl is AxisControl axis)
                {
                    return axis.ReadUnprocessedValue();
                }
                else
                {
                    Debug.LogError(axisName + " is not an axis type.");
                    return 0;
                }
            }
            else
            {
                Debug.LogError("Can't find action '" + axisName + "' in " + input.name + "!");
                return 0;
            }
        }

        public Vector2 GetVector2(string action)
        {
            if (input == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return Vector2.zero;
            }

            if (actions == null)
            {
                UpdateActions();
            }

            if (actions.TryGetValue(action, out InputAction inputAction))
            {
                return inputAction.ReadValue<Vector2>();
            }
            else
            {
                Debug.LogError("Can't find action '" + action + "' in " + input.name + "!");
                return Vector2.zero;
            }
        }
#endif
    }
}
