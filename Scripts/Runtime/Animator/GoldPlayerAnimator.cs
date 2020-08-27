#if !GOLD_PLAYER_DISABLE_ANIMATOR
using System;
using UnityEngine;

namespace Hertzole.GoldPlayer
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Gold Player/Gold Player Animator", 10)]
    public class GoldPlayerAnimator : MonoBehaviour
    {
        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private float maxSpeed = 6f;
        [SerializeField]
        private float valueSmooth = 0.15f;

        [Header("Parameters")]
        [SerializeField]
        private GoldPlayerAnimatorParameterInfo moveX = new GoldPlayerAnimatorParameterInfo(0, true);
        [SerializeField]
        private GoldPlayerAnimatorParameterInfo moveY = new GoldPlayerAnimatorParameterInfo(0, true);
        [SerializeField]
        private GoldPlayerAnimatorParameterInfo crouching = new GoldPlayerAnimatorParameterInfo(0, true);
        [SerializeField]
        private GoldPlayerAnimatorParameterInfo lookAngle = new GoldPlayerAnimatorParameterInfo(0, true);

        [SerializeField]
        [HideInInspector]
        private CharacterController controller = null;
        [SerializeField]
        [HideInInspector]
        private GoldPlayerController playerController = null;

        private int moveXHash;
        private int moveZHash;
        private int crouchingHash;
        private int lookAngleHash;

        private Vector3 targetVelocity;
        private Vector3 targetValue;

        private void Awake()
        {
            if (animator != null)
            {
                moveXHash = animator.GetParameter(moveX.index).nameHash;
                moveZHash = animator.GetParameter(moveY.index).nameHash;
                crouchingHash = animator.GetParameter(crouching.index).nameHash;
                lookAngleHash = animator.GetParameter(lookAngle.index).nameHash;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (animator != null)
            {
                CalculateVelocity();
                CalculateLookAngle();

                if (crouching.enabled)
                {
                    animator.SetBool(crouchingHash, playerController.Movement.IsCrouching);
                }
            }
        }

        private void CalculateVelocity()
        {
            if (!moveX.enabled && !moveY.enabled)
            {
                return;
            }

            Vector3 velocity = transform.InverseTransformDirection(controller.velocity);
            velocity /= maxSpeed;

            targetValue = Vector3.SmoothDamp(targetValue, velocity, ref targetVelocity, valueSmooth);

            if (moveX.enabled)
            {
                animator.SetFloat(moveXHash, targetValue.x);
            }

            if (moveY.enabled)
            {
                animator.SetFloat(moveZHash, targetValue.z);
            }
        }

        private void CalculateLookAngle()
        {
            if (!this.lookAngle.enabled)
            {
                return;
            }

            float lookAngle;
            if (playerController.Camera.CameraHead.eulerAngles.x <= 90f)
            {
                lookAngle = -playerController.Camera.CameraHead.eulerAngles.x;
            }
            else
            {
                lookAngle = 360 - playerController.Camera.CameraHead.eulerAngles.x;
            }

            animator.SetFloat(lookAngleHash, lookAngle);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            GetStandardComponents();
        }

        private void Reset()
        {
            GetStandardComponents();
        }

        private void GetStandardComponents()
        {
            if (controller == null)
            {
                controller = GetComponent<CharacterController>();
            }

            if (playerController == null)
            {
                playerController = GetComponent<GoldPlayerController>();
            }
        }
#endif
    }

    [Serializable]
    public struct GoldPlayerAnimatorParameterInfo : IEquatable<GoldPlayerAnimatorParameterInfo>
    {
        public int index;
        public bool enabled;

        public GoldPlayerAnimatorParameterInfo(int index, bool enabled)
        {
            this.index = index;
            this.enabled = enabled;
        }

        public override bool Equals(object obj)
        {
            return obj is GoldPlayerAnimatorParameterInfo info && Equals(info);
        }

        public bool Equals(GoldPlayerAnimatorParameterInfo other)
        {
            return index == other.index && enabled == other.enabled;
        }

        public override int GetHashCode()
        {
            int hashCode = -1933203711;
            hashCode = hashCode * -1521134295 + index.GetHashCode();
            hashCode = hashCode * -1521134295 + enabled.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(GoldPlayerAnimatorParameterInfo left, GoldPlayerAnimatorParameterInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GoldPlayerAnimatorParameterInfo left, GoldPlayerAnimatorParameterInfo right)
        {
            return !(left == right);
        }
    }
}
#endif
