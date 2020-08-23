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
        private string moveX = "MoveX";
        [SerializeField]
        private string moveZ = "MoveZ";

        [SerializeField]
        [HideInInspector]
        private CharacterController controller = null;

        private int moveXHash;
        private int moveZHash;

        private void Awake()
        {
            moveXHash = Animator.StringToHash(moveX);
            moveZHash = Animator.StringToHash(moveZ);
        }

        // Update is called once per frame
        private void Update()
        {
            Vector3 velocity = transform.InverseTransformDirection(controller.velocity);
            velocity /= maxSpeed;

            animator.SetFloat(moveXHash, velocity.x);
            animator.SetFloat(moveZHash, velocity.z);
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
