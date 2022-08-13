#if GOLD_PLAYER_DISABLE_OBJECT_BOB
#define OBSOLETE
#endif

#if OBSOLETE && !UNITY_EDITOR
#define STRIP
#endif

#if !STRIP
using UnityEngine;

namespace Hertzole.GoldPlayer
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
#if !OBSOLETE
    [AddComponentMenu("Gold Player/Gold Player Object Bob", 10)]
#else
    [System.Obsolete("Gold Player Object Bob has been disabled. GoldPlayerObjectBob will be removed on build.")]
    [AddComponentMenu("")]
#endif
    public class GoldPlayerObjectBob : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The target objects to bob.")]
        private BobClass[] targets = null;

        [SerializeField]
        [HideInInspector]
        private CharacterController controller = null;
        [SerializeField]
        [HideInInspector]
        private GoldPlayerController playerController = null;

        public BobClass[] Targets { get { return targets; } set { targets = value; } }

        private void Awake()
        {
#if OBSOLETE
            Debug.LogError(gameObject.name + " has GoldPlayerObjectBob attached. It will be removed on build. Please remove this component if you don't intend to use it.", gameObject);
#else

            // Initialize all the argets.
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].Initialize();
            }
#endif
        }

        private void Update()
        {
            // Cache the velocity to avoid doing native calls for each object.
            // If there's a player controller present, use the velocity from there.
            Vector3 velocity = playerController != null ? playerController.Velocity : controller.velocity;
            // Cache the tilt. Clamp it at -1 to 1 to avoid it going further than expected.
            float tilt = Mathf.Clamp(transform.InverseTransformDirection(controller.velocity).x, -1f, 1f);
            // Cache the delta time to avoid native calls.
            float deltaTime = Time.deltaTime;

            // Go through each object and do the bob.
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].DoBob(velocity, deltaTime, tilt);
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
            // Gets the controller in the editor, usually when the script is added.
            // Avoids a GetComponent in Awake.
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
}
#endif
