﻿using UnityEngine;

namespace Hertzole.GoldPlayer.Tests
{
    internal class GoldPlayerTestInput : MonoBehaviour, IGoldInput
    {
        public Vector2 moveDirection;
        public Vector2 mouseInput;
        public bool isRunning;
        public bool isJumping;
        public bool isCrouching;
        public bool isInteracting;

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
                case "Run":
                    return isRunning;
                case "Crouch":
                    return isCrouching;
                default:
                    return false;
            }
        }

        public bool GetButtonDown(string buttonName)
        {
            switch (buttonName)
            {
                case "Jump":
                    bool r = isJumping;
                    isJumping = false;
                    return r;
                case "Interact":
                    r = isInteracting;
                    isInteracting = false;
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
                case "Move":
                    return moveDirection;
                case "Look":
                    return mouseInput;
                default:
                    return Vector2.zero;
            }

        }
    }
}
