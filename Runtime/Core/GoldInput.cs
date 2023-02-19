#if UNITY_EDITOR
using UnityEngine;

namespace Hertzole.GoldPlayer
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    [System.Obsolete("Inherit from 'IGoldInput' instead. This will be removed from build.", true)]
    [UnityEngine.TestTools.ExcludeFromCoverage]
    public abstract class GoldInput : MonoBehaviour
    {
        /// <summary>
        /// Returns true while the virtual button identified by buttonName is held down.
        /// </summary>
        [UnityEngine.TestTools.ExcludeFromCoverage]
        public virtual bool GetButton(string buttonName)
        {
            return false;
        }

        /// <summary>
        /// Returns true during the frame the user pressed down the virtual button identified by buttonName.
        /// </summary>
        [UnityEngine.TestTools.ExcludeFromCoverage]
        public virtual bool GetButtonDown(string buttonName)
        {
            return false;
        }

        /// <summary>
        /// Returns true the first frame the user releases the virtual button identified by buttonName.
        /// </summary>
        [UnityEngine.TestTools.ExcludeFromCoverage]
        public virtual bool GetButtonUp(string buttonName)
        {
            return false;
        }

        /// <summary>
        /// Returns the value of the virtual axis identified by axisName.
        /// </summary>
        [UnityEngine.TestTools.ExcludeFromCoverage]
        public virtual float GetAxis(string axisName)
        {
            return 0;
        }

        /// <summary>
        /// Returns the value of the virtual axis identified by axisName with no smoothing filtering applied.
        /// </summary>
        [UnityEngine.TestTools.ExcludeFromCoverage]
        public virtual float GetAxisRaw(string axisName)
        {
            return 0;
        }

#if !ENABLE_INPUT_SYSTEM && GOLD_PLAYER_NEW_INPUT
        [System.Obsolete("Only used in the new Input System. Will do nothing with Input Manager.")]
#endif
        [UnityEngine.TestTools.ExcludeFromCoverage]
        public virtual Vector2 GetVector2(string actionName)
        {
            return Vector2.zero;
        }
    }
}
#endif
