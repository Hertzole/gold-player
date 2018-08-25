using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public abstract class GoldInput : MonoBehaviour
    {
        /// <summary>
        /// Returns true while the virtual button identified by buttonName is held down.
        /// </summary>
        public virtual bool GetButton(string buttonName)
        {
            return false;
        }

        /// <summary>
        /// Returns true during the frame the user pressed down the virtual button identified by buttonName.
        /// </summary>
        public virtual bool GetButtonDown(string buttonName)
        {
            return false;
        }

        /// <summary>
        /// Returns true the first frame the user releases the virtual button identified by buttonName.
        /// </summary>
        public virtual bool GetButtonUp(string buttonName)
        {
            return false;
        }

        /// <summary>
        /// Returns the value of the virtual axis identified by axisName.
        /// </summary>
        public virtual float GetAxis(string axisName)
        {
            return 0;
        }

        /// <summary>
        /// Returns the value of the virtual axis identified by axisName with no smoothing filtering applied.
        /// </summary>
        public virtual float GetAxisRaw(string axisName)
        {
            return 0;
        }

        /// <summary>
        /// Returns the Input Item that matches the buttonName in the given InputItem array.
        /// </summary>
        /// <param name="buttonName">The name of the item to try and find.</param>
        /// <param name="inputsArray">The array to search in to find the item.</param>
        protected virtual InputItem GetItem(string buttonName, InputItem[] inputsArray)
        {
            for (int i = 0; i < inputsArray.Length; i++)
            {
                if (inputsArray[i].ButtonName == buttonName)
                    return inputsArray[i];
            }

            Debug.LogError("No input with the name '" + buttonName + "' assigned on '" + gameObject.name + "'!");
            return new InputItem();
        }
    }
}
