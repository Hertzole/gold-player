using UnityEngine;

namespace Hertzole.GoldPlayer.Tests
{
    internal class GoldPlayerTestInput : MonoBehaviour, IGoldInput
    {
        public Vector2 moveDirection;
        public Vector2 mouseInput;
        public bool isRunning;
        public bool isRunningToggle;
        public bool isJumping;
        public bool isCrouching;
        public bool isCrouchingToggle;
        public bool isInteracting;

        public const string MOVE = "Moving";
        public const string LOOK = "Looking";
        public const string RUN = "Running";
        public const string CROUCH = "Crouching";
        public const string JUMP = "Jumping";
        public const string INTERACT = "Interacting";

        public void DisableAction(string action)
        {
            throw new System.NotImplementedException();
        }

        public void DisableInput()
        {
            throw new System.NotImplementedException();
        }

        public void EnableAction(string action)
        {
            throw new System.NotImplementedException();
        }

        public void EnableInput()
        {
            throw new System.NotImplementedException();
        }

        public float GetAxis(string axis)
        {
            switch (axis)
            {
                case "Horizontal":
                    return moveDirection.x;
                case "Vertical":
                    return moveDirection.y;
                case "Mouse X":
                    return mouseInput.x;
                case "Mouse Y":
                    return mouseInput.y;
                default:
                    return 0;
            }
        }

        public float GetAxisRaw(string axis)
        {
            switch (axis)
            {
                case "Horizontal":
                    return moveDirection.x;
                case "Vertical":
                    return moveDirection.y;
                case "Mouse X":
                    return mouseInput.x;
                case "Mouse Y":
                    return mouseInput.y;
                default:
                    return 0;
            }
        }

        public bool GetButton(string buttonName)
        {
            switch (buttonName)
            {
                case RUN:
                    return isRunning;
                case CROUCH:
                    return isCrouching;
                default:
                    return false;
            }
        }

        public bool GetButtonDown(string buttonName)
        {
            switch (buttonName)
            {
                case JUMP:
                    bool r = isJumping;
                    isJumping = false;
                    return r;
                case INTERACT:
                    r = isInteracting;
                    isInteracting = false;
                    return r;
                case RUN:
                    r = isRunningToggle;
                    isRunningToggle = false;
                    return r;
                case CROUCH:
                    r = isCrouchingToggle;
                    isCrouchingToggle = false;
                    return r;
                default:
                    return false;
            }
        }

        public bool GetButtonUp(string buttoName)
        {
            return false;
        }

        public Vector2 GetVector2(string actionName)
        {
            switch (actionName)
            {
                case MOVE:
                    return moveDirection;
                case LOOK:
                    return mouseInput;
                default:
                    return Vector2.zero;
            }

        }
    }
}
