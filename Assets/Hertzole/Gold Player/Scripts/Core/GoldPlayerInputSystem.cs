using Hertzole.GoldPlayer.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hertzole.GoldPlayer
{
    public class GoldPlayerInputSystem : GoldInput
    {
#if ENABLE_INPUT_SYSTEM
        [SerializeField]
        private InputActionAsset input = null;

        private Dictionary<string, InputAction> actions;
        private Dictionary<string, bool> actionDown;
        private Dictionary<string, bool> actionUp;
#endif

        private void Start()
        {
#if ENABLE_INPUT_SYSTEM
            UpdateActions();
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private void OnEnable()
        {
            input.Enable();
        }

        private void OnDisable()
        {
            input.Disable();
        }

        private void UpdateActions()
        {
            actions = new Dictionary<string, InputAction>();
            actionDown = new Dictionary<string, bool>();
            actionUp = new Dictionary<string, bool>();

            foreach (InputActionMap item in input.actionMaps)
            {
                foreach (InputAction action in item.actions)
                {
                    actions.Add(action.name, action);
                    actionDown.Add(action.name, false);
                    actionUp.Add(action.name, false);
                }
            }
        }

        public override bool GetButton(string buttonName)
        {
            if (actions == null)
            {
                UpdateActions();
            }

            if (actions.TryGetValue(buttonName, out InputAction inputAction))
            {
                return inputAction.ReadValue<float>() == 1;
            }
            else
            {
                Debug.LogError("Can't find action '" + buttonName + "' in " + input.name + "!");
                return false;
            }
        }

        public override bool GetButtonDown(string buttonName)
        {
            if (actions == null)
            {
                UpdateActions();
            }

            if (actions.TryGetValue(buttonName, out InputAction inputAction))
            {
                bool value = inputAction.ReadValue<float>() == 1;
                if (value && !actionDown[buttonName])
                {
                    actionDown[buttonName] = value;
                    return true;
                }
                else if (!value)
                {
                    actionDown[buttonName] = value;
                    return false;
                }

                return false;
            }
            else
            {
                Debug.LogError("Can't find action '" + buttonName + "' in " + input.name + "!");
                return false;
            }
        }

        public override bool GetButtonUp(string buttonName)
        {
            if (actions == null)
            {
                UpdateActions();
            }

            if (actions.TryGetValue(buttonName, out InputAction inputAction))
            {
                bool value = inputAction.ReadValue<float>() == 1;
                if (value && !actionUp[buttonName])
                {
                    actionUp[buttonName] = value;
                    return false;
                }
                else if (!value && actionUp[buttonName])
                {
                    actionUp[buttonName] = value;
                    return true;
                }

                return false;
            }
            else
            {
                Debug.LogError("Can't find action '" + buttonName + "' in " + input.name + "!");
                return false;
            }
        }

        public override Vector2 GetVector2(string action)
        {
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
