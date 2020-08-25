#if !GOLD_PLAYER_DISABLE_ANIMATOR
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
        private int moveX = 0;
        [SerializeField]
        private int moveY = 0;

        [SerializeField]
        [HideInInspector]
        private CharacterController controller = null;

        private int moveXHash;
        private int moveZHash;

        private Vector3 targetVelocity;
        private Vector3 targetValue;

        private void Awake()
        {
            if (animator != null)
            {
                moveXHash = animator.GetParameter(moveX).nameHash;
                moveZHash = animator.GetParameter(moveY).nameHash;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (animator != null)
            {
                Vector3 velocity = transform.InverseTransformDirection(controller.velocity);
                velocity /= maxSpeed;

                targetValue = Vector3.SmoothDamp(targetValue, velocity, ref targetVelocity, valueSmooth);

                animator.SetFloat(moveXHash, targetValue.x);
                animator.SetFloat(moveZHash, targetValue.z);
            }
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
        }
#endif
    }
}
#endif
