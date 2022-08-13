#if !ENABLE_INPUT_SYSTEM || !GOLD_PLAYER_NEW_INPUT
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR // If it's obsolete and not in the editor, remove it.
#define STRIP
#endif

#if !STRIP
using UnityEngine;
#if !OBSOLETE
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace Hertzole.GoldPlayer
{
#if OBSOLETE
    [System.Obsolete("You're not using the new Input System so this component will be useless.")]
    [AddComponentMenu("")]
#else
    [AddComponentMenu("Gold Player/Gold Player Input System", 1)]
    [DisallowMultipleComponent]
#endif
    public class GoldPlayerInputSystem : MonoBehaviour, IGoldInput
    {
#if !OBSOLETE
        [SerializeField] 
        [Tooltip("The player input to get the actions from.")]
        private PlayerInput playerInput = default;
        [SerializeField]
        [FormerlySerializedAs("input")]
        [Tooltip("The input asset to get all actions from.")]
        private InputActionAsset inputAsset = null;
        [SerializeField]
        [Tooltip("All the available actions.")]
        private InputSystemItem[] actions = null;
#endif
        [SerializeField]
        [Tooltip("If true, all actions will be enabled on enable.")]
        private bool autoEnableInput = true;
        [SerializeField]
        [Tooltip("If true, all actions will be disabled on disable.")]
        private bool autoDisableInput = true;

        private bool enabledInput = false;

#if !OBSOLETE
#if UNITY_EDITOR
        [System.Obsolete("Use 'InputAsset' instead. This will be removed on build.", true)]
        public InputActionAsset Input { get { return InputAsset; } set { InputAsset = value; } }
#endif
	    /// <summary> The player input to get the actions from. </summary>
	    public PlayerInput PlayerInput 
		{
		    get { return playerInput; }
		    set
		    {
			    if (playerInput != value)
			    {
					playerInput = value;
					UpdateActions();
			    }
		    } 
	    }
        /// <summary> The input asset to get all actions from. </summary>
        public InputActionAsset InputAsset { get { return inputAsset; } set { inputAsset = value; } }
        private readonly Dictionary<int, InputAction> actionsDictionary = new Dictionary<int, InputAction>();
        
        protected InputActionAsset ActionsAsset { get { return playerInput != null ? playerInput.actions : inputAsset; } }
#endif

        /// <summary> Has input been enabled? </summary>
        public bool EnabledInput { get { return enabledInput; } }
        /// <summary> If true, all actions will be enabled on enable. </summary>
        public bool AutoEnableInput { get { return autoEnableInput; } set { autoEnableInput = value; } }
        /// <summary> If true, all actions will be disabled on disable. </summary>
        public bool AutoDisableInput { get { return autoDisableInput; } set { autoDisableInput = value; } }

#if !OBSOLETE
        /// <summary> All the available actions. </summary>
        public InputSystemItem[] Actions { get { return actions; } set { actions = value; } }
#endif
	    
	    private void Start()
        {
#if OBSOLETE
            Debug.LogError(gameObject.name + " has GoldPlayerInputSystem added. It does not work on the legacy input manager.");
#else
            UpdateActions();
#endif
        }

        /// <summary>
        /// Enables all actions.
        /// </summary>
        public void EnableInput()
        {
#if !OBSOLETE
            if (actions != null)
            {
                for (int i = 0; i < actions.Length; i++)
                {
                    // Put in DEBUG or Unity editor because we don't want this in release builds in order to improve performance.
#if DEBUG || UNITY_EDITOR
                    // If the action doesn't exist, complain.
                    if (actions[i].action == null)
                    {
                        Debug.LogWarning("There's no action asset present on " + actions[i].actionName + ". It will not be enabled.", gameObject);
                        continue;
                    }
#endif
                    ActionsAsset[actions[i].actionName].Enable();
                }
                enabledInput = true;
            }
#endif
        }

        /// <summary>
        /// Disables all actions.
        /// </summary>
        public void DisableInput()
        {
#if !OBSOLETE
            if (actions != null)
            {
                for (int i = 0; i < actions.Length; i++)
                {
                    // Put in DEBUG or Unity editor because we don't want this in release builds in order to improve performance.
#if DEBUG || UNITY_EDITOR
                    // If the action doesn't exist, complain.
                    if (actions[i].action == null)
                    {
                        Debug.LogWarning("There's no action asset present on " + actions[i].actionName + ". It will not be disabled.", gameObject);
                        continue;
                    }
#endif
                    ActionsAsset[actions[i].actionName].Disable();
                }
                enabledInput = false;
            }
#endif
        }

        /// <summary>
        /// Enables a specific action.
        /// </summary>
        /// <param name="actionHash">The action by name to enable.</param>
        public void EnableAction(int actionHash)
        {
#if !OBSOLETE
            // Put in DEBUG or Unity editor because we don't want this in release builds in order to improve performance.
#if DEBUG || UNITY_EDITOR
            // If the action doesn't exist, complain.
            if (!actionsDictionary.ContainsKey(actionHash))
            {
                throw new System.ArgumentException($"There's no action called '{actionHash}' on {gameObject.name}.");
            }

            // If there's no action, complain.
            if (actionsDictionary[actionHash] == null)
            {
                Debug.LogWarning($"There's no action asset present on {actionsDictionary[actionHash]}.", gameObject);
                return;
            }
#endif // DEBUG || UNITY_EDITOR
            actionsDictionary[actionHash].Enable();
#endif // !OBSOLETE
        }

        /// <summary>
        /// Enables a specific action.
        /// </summary>
        /// <param name="actionIndex">The action by index to enable.</param>
        public void EnableActionIndex(int actionIndex)
        {
#if !OBSOLETE
            // Put in DEBUG or Unity editor because we don't want this in release builds in order to improve performance.
#if DEBUG || UNITY_EDITOR
            // If the index is out of range, complain.
            if (actionIndex < 0 || actionIndex >= actions.Length)
            {
                throw new System.ArgumentOutOfRangeException(nameof(actionIndex));
            }

            // If there's no action, complain.
            if (actions[actionIndex].action == null)
            {
                Debug.LogWarning($"There's no action asset present on {actions[actionIndex].actionName}.", gameObject);
                return;
            }
#endif // DEBUG || UNITY_EDITOR
            actions[actionIndex].action.action.Enable();
#endif // !OBSOLETE
        }

        /// <summary>
        /// Disables a specific action.
        /// </summary>
        /// <param name="actionHash">The action by name to disable.</param>
        public void DisableAction(int actionHash)
        {
#if !OBSOLETE
            // Put in DEBUG or Unity editor because we don't want this in release builds in order to improve performance.
#if DEBUG || UNITY_EDITOR
            // If the action doesn't exist, complain.
            if (!actionsDictionary.ContainsKey(actionHash))
            {
                throw new System.ArgumentException($"There's no action called '{actionHash}' on {gameObject.name}.");
            }

            // If there's no action, complain.
            if (actionsDictionary[actionHash] == null)
            {
                Debug.LogWarning($"There's no action asset present on {actionsDictionary[actionHash]}.", gameObject);
                return;
            }
#endif // DEBUG || UNITY_EDITOR
            actionsDictionary[actionHash].Disable();
#endif // !OBSOLETE
        }

        /// <summary>
        /// Disables a specific action.
        /// </summary>
        /// <param name="actionIndex">The action by index to disable.</param>
        public void DisableActionIndex(int actionIndex)
        {
#if !OBSOLETE
            // Put in DEBUG or Unity editor because we don't want this in release builds in order to improve performance.
#if DEBUG || UNITY_EDITOR
            // If the index is out of range, complain.
            if (actionIndex < 0 || actionIndex >= actions.Length)
            {
                throw new System.ArgumentOutOfRangeException(nameof(actionIndex));
            }

            // If there's no action, complain.
            if (actions[actionIndex].action == null)
            {
                Debug.LogWarning($"There's no action asset present on {actions[actionIndex].actionName}.", gameObject);
                return;
            }
#endif // DEBUG || UNITY_EDITOR
            actions[actionIndex].action.action.Disable();
#endif // !OBSOLETE
        }

#if !OBSOLETE
        private void OnEnable()
        {
            // Enable all input if auto enable input is on.
            if (autoEnableInput)
            {
                EnableInput();
            }
        }

        private void OnDisable()
        {
            // Disable all input if auto disable input is on.
            if (autoDisableInput)
            {
                DisableInput();
            }
        }

        // Add all actions to the actions dictionary.
        public void UpdateActions()
        {
            if (actions == null || actions.Length == 0)
            {
	            actionsDictionary.Clear();
                return;
            }

            actionsDictionary.Clear();
            for (int i = 0; i < actions.Length; i++)
            {
                actionsDictionary.Add(GoldPlayerController.InputNameToHash(actions[i].actionName), ActionsAsset[actions[i].actionName]);
            }
        }
#endif // !OBSOLETE

        /// <summary>
        /// Returns true while an action is being held down.
        /// </summary>
        /// <param name="buttonName">The action to check.</param>
        public bool GetButton(int buttonName)
        {
#if !OBSOLETE
            // Make sure the action exists.
            if (!DoesActionExist(buttonName))
            {
                return false;
            }

            return actionsDictionary[buttonName].activeControl is ButtonControl button && button.isPressed;
#else
            return false;
#endif
        }

        /// <summary>
        /// Returns true if the action was pressed this frame.
        /// </summary>
        /// <param name="buttonName">The action to check.</param>
        public bool GetButtonDown(int buttonName)
        {
#if !OBSOLETE
            // Make sure the action exists.
            if (!DoesActionExist(buttonName))
            {
                return false;
            }

            return actionsDictionary[buttonName].activeControl is ButtonControl button && button.wasPressedThisFrame;
#else
            return false;
#endif
        }

        /// <summary>
        /// Returns true if the action was released this frame.
        /// </summary>
        /// <param name="buttonName">The action to check.</param>
        public bool GetButtonUp(int buttonName)
        {
#if !OBSOLETE
            // Make sure the action exists.
            if (!DoesActionExist(buttonName))
            {
                return false;
            }

            return actionsDictionary[buttonName].activeControl is ButtonControl button && button.wasReleasedThisFrame;
#else
            return false;
#endif
        }

        /// <summary>
        /// Returns the value of an action axis.
        /// </summary>
        /// <param name="axisName">The action to check.</param>
        public float GetAxis(int axisName)
        {
#if !OBSOLETE
            // Make sure the action exists.
            if (!DoesActionExist(axisName, true))
            {
                return 0;
            }

            return actionsDictionary[axisName].ReadValue<float>();
#else
            return 0;
#endif
        }

        /// <summary>
        /// Does the same as GetAxis when using the input system.
        /// </summary>
        /// <param name="axisName">The action to check.</param>
        public float GetAxisRaw(int axisName)
        {
            return GetAxis(axisName);
        }

        /// <summary>
        /// Returns the Vector2 value from an action.
        /// </summary>
        /// <param name="action">The action to check.</param>
        public Vector2 GetVector2(int action)
        {
#if !OBSOLETE
            // Make sure the action exists.
            if (!DoesActionExist(action))
            {
                return Vector2.zero;
            }
            
            return actionsDictionary[action].ReadValue<Vector2>();
#else
            return Vector2.zero;
#endif
        }

#if !OBSOLETE
        private bool DoesActionExist(int action, bool axis = false)
        {
            // Put in DEBUG or Unity editor because we don't want this in release builds in order to improve performance.
#if DEBUG || UNITY_EDITOR
            if (inputAsset == null)
            {
                Debug.LogWarning($"There is no input asset on {gameObject.name}.", gameObject);
                return false;
            }
#endif

            if (actionsDictionary.Count != actions.Length)
            {
                UpdateActions();
            }

            // Put in DEBUG or Unity editor because we don't want this in release builds in order to improve performance.
#if DEBUG || UNITY_EDITOR
            // If there's no action, complain.
            if (!actionsDictionary.ContainsKey(action))
            {
                Debug.LogError($"Can't find action '{action}' in {inputAsset.name}.");
                return false;
            }

            // Check if there's an action asset assigned.
            if (actionsDictionary[action] == null)
            {
                Debug.LogError($"There's no action assigned on '{action}'.", gameObject);
                return false;
            }
#endif

            if (actionsDictionary[action].activeControl == null)
            {
                return false;
            }

#if DEBUG || UNITY_EDITOR
            // If it's an axis action, make sure the type is an axis.
            if (axis)
            {
                if (!(actionsDictionary[action].activeControl is AxisControl))
                {
                    Debug.LogError($"{action} is not an axis type.");
                    return false;
                }
            }
#endif
            return true;
        }
#endif // !OBSOLETE

#if UNITY_EDITOR && !OBSOLETE
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
#if !GOLD_PLAYER_DISABLE_INTERACTION
            GoldPlayerInteraction gi = GetComponent<GoldPlayerInteraction>();
#endif

            // If the player controller exists, add the input action name from there, otherwise just make it generic.
            actions = new InputSystemItem[]
            {
                new InputSystemItem(gp != null ? gp.Camera.LookInput : "Look", null),
                new InputSystemItem(gp != null ? gp.Movement.MoveInput : "Move", null),
                new InputSystemItem(gp != null ? gp.Camera.ZoomInput : "Zoom", null),
                new InputSystemItem(gp != null ? gp.Movement.JumpInput : "Jump", null),
                new InputSystemItem(gp != null ? gp.Movement.RunInput : "Run", null),
                new InputSystemItem(gp != null ? gp.Movement.CrouchInput : "Crouch", null),
#if !GOLD_PLAYER_DISABLE_INTERACTION
                new InputSystemItem(gi != null ? gi.InteractInput : "Interact", null)
#endif
            };
        }
#endif
    }
}
#endif
