#if GOLD_PLAYER_DISABLE_ANIMATOR
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR
#define STRIP
#endif

#if !STRIP
using System;
using UnityEngine;

namespace Hertzole.GoldPlayer
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
#if !OBSOLETE
    [AddComponentMenu("Gold Player/Gold Player Animator", 10)]
#else
    [System.Obsolete("Gold Player Animator has been disabled. GoldPlayerAnimator will be removed on build.")]
    [AddComponentMenu("")]
#endif
    public class GoldPlayerAnimator : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The target animator.")]
        private Animator animator = null;
        [SerializeField]
        [Tooltip("The max speed of your player to divide with.")]
        private float maxSpeed = 6f;
        [SerializeField]
        [Tooltip("Smooths out the value to make transitions between Move X/Y values.")]
        private float valueSmoothTime = 1f;
        [SerializeField]
        [Tooltip("If true, the smoothing will use unscaled delta time.")]
        private bool unscaledSmooth = false;

#if UNITY_EDITOR
        [Header("References")]
#endif
        [SerializeField]
        [Tooltip("The transform to use for the look angle. If empty it will use Camera Head from Gold Player Controller.")]
        private Transform lookAngleHead = null;

#if UNITY_EDITOR
        [Header("Parameters")]
#endif
        [SerializeField]
        [Tooltip("The Move X parameter on your animator.")]
        private GoldPlayerAnimatorParameterInfo moveX = new GoldPlayerAnimatorParameterInfo(0, true);
        [SerializeField]
        [Tooltip("The Move X parameter on your animator.")]
        private GoldPlayerAnimatorParameterInfo moveY = new GoldPlayerAnimatorParameterInfo(0, true);
        [SerializeField]
        [Tooltip("The Move X parameter on your animator.")]
        private GoldPlayerAnimatorParameterInfo crouching = new GoldPlayerAnimatorParameterInfo(0, true);
        [SerializeField]
        [Tooltip("The Move X parameter on your animator.")]
        private GoldPlayerAnimatorParameterInfo lookAngle = new GoldPlayerAnimatorParameterInfo(0, true);

        [SerializeField]
        [HideInInspector]
        private CharacterController controller = null;
        [SerializeField]
        [HideInInspector]
        private GoldPlayerController playerController = null;

        private bool hasGoldPlayer;
        
        // The hashes of the parameters.
        private int moveXHash;
        private int moveYHash;
        private int crouchingHash;
        private int lookAngleHash;

        private Vector3 targetValue;

        // The transform to use for look angle.
        private Transform targetLookAngle;

        /// <summary> The target animator. </summary>
        public Animator Animator
        {
            get { return animator; }
            set
            {
                if (animator != value)
                {
                    animator = value;
#if DEBUG || UNITY_EDITOR
                    ValidateParameters();
#endif
                    GetAnimatorHashes();
                }
            }
        }
        /// <summary> The max speed of your player to divide with. </summary>
        public float MaxSpeed { get { return maxSpeed; } set { maxSpeed = value; } }
        /// <summary> Smooths out the value to make transitions between Move X/Y values. </summary>
        public float ValueSmoothTime { get { return valueSmoothTime; } set { valueSmoothTime = value; } }
        /// <summary> If true, the smoothing will use unscaled delta time. </summary>
        public bool UnscaledSmooth { get { return unscaledSmooth; } set { unscaledSmooth = value; } }

        /// <summary> The Move X parameter. </summary>
        public string MoveX { get { return GetAnimatorParameter(moveX.index); } set { moveXHash = Animator.StringToHash(value); } }
        /// <summary> The Move Y parameter. </summary>
        public string MoveY { get { return GetAnimatorParameter(moveY.index); } set { moveYHash = Animator.StringToHash(value); } }
        /// <summary> The Crouching parameter. </summary>
        public string Crouching { get { return GetAnimatorParameter(crouching.index); } set { crouchingHash = Animator.StringToHash(value); } }
        /// <summary> The Look Angle parameter. </summary>
        public string LookAngle { get { return GetAnimatorParameter(lookAngle.index); } set { lookAngleHash = Animator.StringToHash(value); } }

        /// <summary> Determines if the Move X parameter should be active. </summary>
        public bool EnableMoveX { get { return moveX.enabled; } set { moveX.enabled = value; } }
        /// <summary> Determines if the Move Y parameter should be active. </summary>
        public bool EnableMoveY { get { return moveY.enabled; } set { moveY.enabled = value; } }
        /// <summary> Determines if the Crouching parameter should be active. </summary>
        public bool EnableCrouching { get { return crouching.enabled; } set { crouching.enabled = value; } }
        /// <summary> Determines if the Look Angle parameter should be active. </summary>
        public bool EnableLookAngle { get { return lookAngle.enabled; } set { lookAngle.enabled = value; } }

        /// <summary> The transform to use for the look angle. If empty it will use Camera Head from Gold Player Controller. </summary>
        public Transform LookAngleHead { get { return lookAngleHead; } set { if (lookAngleHead != value) { lookAngleHead = value; UpdateTargetLookAngle(); } } }

        /// <summary>
        /// Used to easily return a name of a animation parameter depending on index.
        /// </summary>
        /// <param name="index">The index of the animation parameter.</param>
        /// <returns>The name of the parameter or nothing if there's no animator.</returns>
        private string GetAnimatorParameter(int index)
        {
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                return string.Empty;
            }
            else
            {
                return animator.GetParameter(index).name;
            }
        }

        private void Awake()
        {
#if OBSOLETE
            Debug.LogError(gameObject.name + " has GoldPlayerAnimator attached. It will be removed on build. Please remove this component if you don't intend to use it.", gameObject);
#else
            if (animator != null)
            {
#if DEBUG || UNITY_EDITOR
                ValidateParameters();
#endif
                GetAnimatorHashes();
            }

            UpdateTargetLookAngle();

            hasGoldPlayer = playerController != null;
#endif
        }

        // Put this function in debug mode because it should only be present
        // during development builds or in the editor. We discard this check
        // in release builds to save performance.
#if DEBUG || UNITY_EDITOR
        /// <summary>
        /// Used to validate all parameters and warn if they are not their expected type.
        /// </summary>
        private void ValidateParameters()
        {
            if (moveX.enabled && animator.GetParameter(moveX.index).type != AnimatorControllerParameterType.Float)
            {
                Debug.LogWarning("Move X parameter needs to be a Float value.", gameObject);
            }

            if (moveY.enabled && animator.GetParameter(moveY.index).type != AnimatorControllerParameterType.Float)
            {
                Debug.LogWarning("Move Y parameter needs to be a Float value.", gameObject);
            }

            if (crouching.enabled && animator.GetParameter(crouching.index).type != AnimatorControllerParameterType.Bool)
            {
                Debug.LogWarning("Crouching parameter needs to be a Boolean value.", gameObject);
            }

            if (lookAngle.enabled && animator.GetParameter(lookAngle.index).type != AnimatorControllerParameterType.Float)
            {
                Debug.LogWarning("Look Angle parameter needs to be a Float value.", gameObject);
            }
        }
#endif

        /// <summary>
        /// Assigns all the hashes depending on their indexes.
        /// </summary>
        protected virtual void GetAnimatorHashes()
        {
            moveXHash = animator.GetParameter(moveX.index).nameHash;
            moveYHash = animator.GetParameter(moveY.index).nameHash;
            crouchingHash = animator.GetParameter(crouching.index).nameHash;
            lookAngleHash = animator.GetParameter(lookAngle.index).nameHash;
        }

        /// <summary>
        /// Used to set the target for look angle. It will use the assigned camera head and if that isn't assigned it will use the one from the player controller.
        /// </summary>
        protected virtual void UpdateTargetLookAngle()
        {
            if (lookAngleHead != null)
            {
                targetLookAngle = lookAngleHead;
            }
            else if (playerController != null)
            {
                targetLookAngle = playerController.Camera.CameraHead;
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

        /// <summary>
        /// Figures out the velocity to set and assigns it to the animator.
        /// </summary>
        protected virtual void CalculateVelocity()
        {
            // If both move X and move Y is disabled, we can just stop here.
            if (!moveX.enabled && !moveY.enabled)
            {
                return;
            }

            // Get the direction relative transform from the character controller.
            Vector3 velocity = transform.InverseTransformDirection(hasGoldPlayer ? controller.velocity : playerController.Velocity);
            velocity /= maxSpeed;

            // It only needs to smooth the value if value smooth is above 0.
            // Else just set it to the velocity.
            if (valueSmoothTime > 0f)
            {
                // Thanks Unity for this piece of awesome code! <3
                // Framerate-independent interpolation.
                // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time.
                float valueLerpPercent = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / valueSmoothTime) * (unscaledSmooth ? Time.unscaledDeltaTime : Time.deltaTime));
                targetValue = Vector3.Lerp(targetValue, velocity, valueLerpPercent);
            }
            else
            {
                targetValue = velocity;
            }

            // If Move X is enabled, set the Move X parameter to the target value.
            if (moveX.enabled)
            {
                animator.SetFloat(moveXHash, targetValue.x);
            }

            // If Move Y is enabled, set the Move Y parameter to the target value.
            if (moveY.enabled)
            {
                animator.SetFloat(moveYHash, targetValue.z);
            }
        }

        protected virtual void CalculateLookAngle()
        {
            // If look angle is disabled, stop here.
            if (!lookAngle.enabled)
            {
                return;
            }

            // If there's no transform for look angle, say so and stop here.
            if (targetLookAngle == null)
            {
                Debug.LogWarning("There's no camera assigned to the Gold Player Animator, nor is there a Gold Player Controller to get one from. Look Angle will not work.");
                return;
            }

            float currentLookAngle;
            // If the look angle is below 90 degrees, we're looking down so make the value -X.
            // Else we're looking up and we just want to go from 0 to 90, so take away 360 from it.
            if (targetLookAngle.eulerAngles.x <= 90f)
            {
                currentLookAngle = -targetLookAngle.eulerAngles.x;
            }
            else
            {
                currentLookAngle = 360 - targetLookAngle.eulerAngles.x;
            }

            // Set the look angle parameter.
            animator.SetFloat(lookAngleHash, currentLookAngle);
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
            // Get the controller in the editor and cache it.
            if (controller == null)
            {
                controller = GetComponent<CharacterController>();
            }

            if (playerController == null)
            {
                playerController = GetComponent<GoldPlayerController>();
            }

            // Adapt to values if they are changed in play mode.
            if (Application.isPlaying)
            {
                ValidateParameters();
                GetAnimatorHashes();
                UpdateTargetLookAngle();
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
#if NET_4_6 || (UNITY_2018_3_OR_NEWER && !NET_LEGACY)
            return obj is GoldPlayerAnimatorParameterInfo info && Equals(info);
#else
            return obj is GoldPlayerAnimatorParameterInfo && Equals((GoldPlayerAnimatorParameterInfo)obj);
#endif

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
