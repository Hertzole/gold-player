using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    public class GoldPlayerInput : GoldInput
    {
        [SerializeField]
        private bool m_UseKeyCodes;
        public bool UseKeyCodes { get { return m_UseKeyCodes; } set { m_UseKeyCodes = value; } }

        [Space]

        [SerializeField]
        private InputItem[] m_Inputs = new InputItem[0];
        public InputItem[] Inputs { get { return m_Inputs; } set { m_Inputs = value; } }

        public override bool GetButton(string buttonName)
        {
            InputItem item = GetItem(buttonName, m_Inputs);
            if (item == null)
                return false;

            if (UseKeyCodes)
                return Input.GetKey(item.Key);
            else
                return Input.GetButton(item.InputName);
        }

        public override bool GetButtonDown(string buttonName)
        {
            InputItem item = GetItem(buttonName, m_Inputs);
            if (item == null)
                return false;

            if (UseKeyCodes)
                return Input.GetKeyDown(item.Key);
            else
                return Input.GetButtonDown(item.InputName);
        }

        public override bool GetButtonUp(string buttonName)
        {
            InputItem item = GetItem(buttonName, m_Inputs);
            if (item == null)
                return false;

            if (UseKeyCodes)
                return Input.GetKeyUp(item.Key);
            else
                return Input.GetButtonUp(item.InputName);
        }

        public override float GetAxis(string axisName)
        {
            InputItem item = GetItem(axisName, m_Inputs);
            if (item == null)
                return 0;

            return Input.GetAxis(item.InputName);
        }

        public override float GetAxisRaw(string axisName)
        {
            InputItem item = GetItem(axisName, m_Inputs);
            if (item == null)
                return 0;

            return Input.GetAxisRaw(item.InputName);
        }
    }
}
