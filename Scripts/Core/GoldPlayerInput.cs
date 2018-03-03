using Hertzole.GoldPlayer.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Hertzole.GoldPlayer
{
    [AddComponentMenu("Gold Player/Gold Player Input", 02)]
    [DisallowMultipleComponent]
    public class GoldPlayerInput : GoldInput
    {
        [SerializeField]
        [Tooltip("Determines if the input should be based around KeyCodes. If false, Input Manager will be used.")]
        private bool m_UseKeyCodes;

        [Space]

        [SerializeField]
        [Tooltip("All the available inputs.")]
        private InputItem[] m_Inputs = new InputItem[0];

        private Dictionary<string, InputItem> m_InputsDic;

        /// <summary> Determines if the input should be based around KeyCodes. If false, Input Manager will be used. </summary>
        public bool UseKeyCodes { get { return m_UseKeyCodes; } set { m_UseKeyCodes = value; } }
        /// <summary> All the available inputs. </summary>
        public InputItem[] Inputs { get { return m_Inputs; } set { m_Inputs = value; UpdateInputs(); } }

        private void Start()
        {
            UpdateInputs();
        }

        public void UpdateInputs()
        {
            m_InputsDic = new Dictionary<string, InputItem>();
            m_InputsDic.Clear();

            for (int i = 0; i < m_Inputs.Length; i++)
            {
                m_InputsDic.Add(m_Inputs[i].ButtonName, m_Inputs[i]);
            }
        }

        public override bool GetButton(string buttonName)
        {
            if (m_UseKeyCodes)
                return Input.GetKey(m_InputsDic[buttonName].Key);
            else
                return Input.GetButton(m_InputsDic[buttonName].InputName);
        }

        public override bool GetButtonDown(string buttonName)
        {
            if (m_UseKeyCodes)
                return Input.GetKeyDown(m_InputsDic[buttonName].Key);
            else
                return Input.GetButtonDown(m_InputsDic[buttonName].InputName);
        }

        public override bool GetButtonUp(string buttonName)
        {
            if (m_UseKeyCodes)
                return Input.GetKeyUp(m_InputsDic[buttonName].Key);
            else
                return Input.GetButtonUp(m_InputsDic[buttonName].InputName);
        }

        public override float GetAxis(string axisName)
        {
            return Input.GetAxis(m_InputsDic[axisName].InputName);
        }

        public override float GetAxisRaw(string axisName)
        {
            return Input.GetAxisRaw(m_InputsDic[axisName].InputName);
        }
    }
}
