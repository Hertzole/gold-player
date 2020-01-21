using UnityEngine;
using UnityEngine.Serialization;
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace Hertzole.GoldPlayer
{
#if !ENABLE_INPUT_SYSTEM || !UNITY_2019_3_OR_NEWER
    [System.Obsolete("You're not using the new Input System so this component will be useless.")]
#else
    [AddComponentMenu("Gold Player/Gold Player Input System", 1)]
    [DisallowMultipleComponent]
#endif
    public class GoldPlayerInputSystem : MonoBehaviour, IGoldInput
    {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
        [System.Serializable]
        public struct InputItem
        {
            public string actionName;
            public InputActionReference action;

            public InputItem(string actionName, InputActionReference action)
            {
                this.actionName = actionName;
                this.action = action;
            }

            public override bool Equals(object obj)
            {
                return obj != null && obj is InputItem item ? item.actionName == actionName && item.action == action : false;
            }

            public override int GetHashCode()
            {
                return (action.action.name + "." + actionName).GetHashCode();
            }

            public static bool operator ==(InputItem left, InputItem right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(InputItem left, InputItem right)
            {
                return !(left == right);
            }
        }

        [SerializeField]
        [FormerlySerializedAs("input")]
        private InputActionAsset inputAsset = null;
        [SerializeField]
        private InputItem[] actions = null;
#endif
        [SerializeField]
        private bool autoEnableInput = true;
        [SerializeField]
        private bool autoDisableInput = true;

        private bool enabledInput = false;

#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
        [System.Obsolete("Use 'InputAsset' instead.")]
        public InputActionAsset Input { get { return InputAsset; } set { InputAsset = value; } }
        public InputActionAsset InputAsset { get { return inputAsset; } set { inputAsset = value; } }
        private Dictionary<string, InputAction> actionsDictionary;
#endif

        public bool EnabledInput { get { return enabledInput; } }
        public bool AutoEnableInput { get { return autoEnableInput; } set { autoEnableInput = value; } }
        public bool AutoDisableInput { get { return autoDisableInput; } set { autoDisableInput = value; } }

        private void Start()
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            UpdateActions();
#endif
        }

        public void EnableInput()
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i].action != null)
                {
                    actions[i].action.action.Enable();
                }
            }

#endif
            enabledInput = true;
        }

        public void DisableInput()
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i].action != null)
                {
                    actions[i].action.action.Disable();
                }
            }
#endif
            enabledInput = false;
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
            actionsDictionary = new Dictionary<string, InputAction>();
            for (int i = 0; i < actions.Length; i++)
            {
                actionsDictionary.Add(actions[i].actionName, actions[i].action);
            }
        }
#endif

        public bool GetButton(string buttonName)
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            if (inputAsset == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return false;
            }

            if (actionsDictionary == null)
            {
                UpdateActions();
            }

            if (actionsDictionary.TryGetValue(buttonName, out InputAction inputAction))
            {
                if (inputAction == null)
                {
                    return false;
                }

                return inputAction.activeControl is ButtonControl button && button.isPressed;
            }
            else
            {
                Debug.LogError("Can't find action '" + buttonName + "' in " + inputAsset.name + "!");
                return false;
            }
#else
            return false;
#endif
        }

        public bool GetButtonDown(string buttonName)
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            if (inputAsset == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return false;
            }

            if (actionsDictionary == null)
            {
                UpdateActions();
            }

            if (actionsDictionary.TryGetValue(buttonName, out InputAction inputAction))
            {
                if (inputAction == null)
                {
                    return false;
                }

                return inputAction.activeControl is ButtonControl button && button.wasPressedThisFrame;
            }
            else
            {
                Debug.LogError("Can't find action '" + buttonName + "' in " + inputAsset.name + "!");
                return false;
            }
#else
            return false;
#endif
        }

        public bool GetButtonUp(string buttonName)
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            if (inputAsset == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return false;
            }

            if (actionsDictionary == null)
            {
                UpdateActions();
            }

            if (actionsDictionary.TryGetValue(buttonName, out InputAction inputAction))
            {
                if (inputAction == null)
                {
                    return false;
                }

                return inputAction.activeControl is ButtonControl button && button.wasReleasedThisFrame;
            }
            else
            {
                Debug.LogError("Can't find action '" + buttonName + "' in " + inputAsset.name + "!");
                return false;
            }
#else
            return true;
#endif
        }

        public float GetAxis(string axisName)
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            if (inputAsset == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return 0;
            }

            if (actionsDictionary.TryGetValue(axisName, out InputAction inputAction))
            {
                if (inputAction == null)
                {
                    return 0;
                }

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
                Debug.LogError("Can't find action '" + axisName + "' in " + inputAsset.name + "!");
                return 0;
            }
#else
            return 0;
#endif
        }

        public float GetAxisRaw(string axisName)
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            if (inputAsset == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return 0;
            }

            if (actionsDictionary.TryGetValue(axisName, out InputAction inputAction))
            {
                if (inputAction == null)
                {
                    return 0;
                }

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
                Debug.LogError("Can't find action '" + axisName + "' in " + inputAsset.name + "!");
                return 0;
            }
#else
            return 0;
#endif
        }

        public Vector2 GetVector2(string action)
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2019_3_OR_NEWER
            if (inputAsset == null)
            {
                Debug.LogWarning("There is no input asset on " + gameObject.name + ".", gameObject);
                return Vector2.zero;
            }

            if (actionsDictionary == null)
            {
                UpdateActions();
            }

            if (actionsDictionary.TryGetValue(action, out InputAction inputAction))
            {
                if (inputAction == null)
                {
                    return Vector2.zero;
                }

                return inputAction.ReadValue<Vector2>();
            }
            else
            {
                Debug.LogError("Can't find action '" + action + "' in " + inputAsset.name + "!");
                return Vector2.zero;
            }
#else
            return Vector2.zero;
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateActions();

                if (enabledInput)
                {
                    EnableInput();
                }
            }
        }

        private void Reset()
        {
            GoldPlayerController gp = GetComponent<GoldPlayerController>();
#if GOLD_PLAYER_INTERACTION
            GoldPlayerInteraction gi = GetComponent<GoldPlayerInteraction>();
#endif

            actions = new InputItem[]
            {
                new InputItem(gp != null ? gp.Camera.LookInput : "Look", null),
                new InputItem(gp != null ? gp.Movement.MoveInput : "Move", null),
                new InputItem(gp != null ? gp.Movement.JumpInput : "Jump", null),
                new InputItem(gp != null ? gp.Movement.RunInput : "Run", null),
                new InputItem(gp != null ? gp.Movement.CrouchInput : "Crouch",null),
#if GOLD_PLAYER_INTERACTION
                new InputItem(gi != null ? gi.InteractInput : "Interact",null)
#endif
            };
        }
#endif
    }
}
