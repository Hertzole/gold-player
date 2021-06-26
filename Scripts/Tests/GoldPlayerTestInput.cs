using System;
using UnityEngine;

namespace Hertzole.GoldPlayer.Tests
{
    internal class GoldPlayerTestInput : MonoBehaviour, IGoldInput
    {
        public Vector2 moveDirection;
        public Vector2 mouseInput;
        public bool isRunning;
        public bool isRunningToggle;
        public bool isJumpingToggle;
        public bool isCrouching;
        public bool isCrouchingToggle;
        public bool isInteracting;

        public const string HORIZONTAL = "Horizontal";
        public const string VERTICAL = "Vertical";
        public const string MOUSE_X = "Mouse X";
        public const string MOUSE_Y = "Mouse Y";
        public const string MOVE = "Moving";
        public const string LOOK = "Looking";
        public const string RUN = "Running";
        public const string CROUCH = "Crouching";
        public const string JUMP = "Jumping";
        public const string INTERACT = "Interacting";

        private readonly int horizontalHash = GoldPlayerController.InputNameToHash(HORIZONTAL);
        private readonly int verticalHash = GoldPlayerController.InputNameToHash(VERTICAL);
        private readonly int mouseXHash = GoldPlayerController.InputNameToHash(MOUSE_X);
        private readonly int mouseYHash = GoldPlayerController.InputNameToHash(MOUSE_Y);
        
        private readonly int moveHash = GoldPlayerController.InputNameToHash(MOVE);
        private readonly int lookHash = GoldPlayerController.InputNameToHash(LOOK);
        private readonly int runHash = GoldPlayerController.InputNameToHash(RUN);
        private readonly int crouchHash = GoldPlayerController.InputNameToHash(CROUCH);
        private readonly int jumpHash = GoldPlayerController.InputNameToHash(JUMP);
        private readonly int interactHash = GoldPlayerController.InputNameToHash(INTERACT);

        public void DisableAction(int action)
        {
            throw new NotImplementedException();
        }

        public void DisableInput()
        {
            throw new NotImplementedException();
        }

        public void EnableAction(int action)
        {
            throw new NotImplementedException();
        }

        public void EnableInput()
        {
            throw new NotImplementedException();
        }
        
        public float GetAxis(int axis)
        {
            if (axis == horizontalHash)
            {
                return moveDirection.x;
            }

            if (axis == verticalHash)
            {
                return moveDirection.y;
            }

            if (axis == mouseXHash)
            {
                return mouseInput.x;
            }

            if (axis == mouseYHash)
            {
                return mouseInput.y;
            }

            return 0;
        }

        public float GetAxisRaw(int axis)
        {
            if (axis == horizontalHash)
            {
                return moveDirection.x;
            }

            if (axis == verticalHash)
            {
                return moveDirection.y;
            }

            if (axis == mouseXHash)
            {
                return mouseInput.x;
            }

            if (axis == mouseYHash)
            {
                return mouseInput.y;
            }

            return 0;
        }

        public bool GetButton(int buttonName)
        {
            if (buttonName == runHash)
            {
                return isRunning;
            }

            if (buttonName == crouchHash)
            {
                return isCrouching;
            }

            return false;
        }

        public bool GetButtonDown(string buttonName)
        {
            return GetButtonDown(GoldPlayerController.InputNameToHash(buttonName));
        }

        public bool GetButtonDown(int buttonName)
        {
            if (buttonName == jumpHash)
            {
                bool r = isJumpingToggle;
                isJumpingToggle = false;
                return r;
            }

            if (buttonName == interactHash)
            {
                bool r = isInteracting;
                isInteracting = false;
                return r;
            }

            if (buttonName == runHash)
            {
                bool r = isRunningToggle;
                isRunningToggle = false;
                return r;
            }

            if (buttonName == crouchHash)
            {
                bool r = isCrouchingToggle;
                isCrouchingToggle = false;
                return r;
            }

            return false;
        }

        public bool GetButtonUp(int buttonName)
        {
            return false;
        }

        public Vector2 GetVector2(int actionName)
        {
            if (actionName == moveHash)
            {
                return moveDirection;
            }

            return actionName == lookHash ? mouseInput : Vector2.zero;
        }
    }
}
