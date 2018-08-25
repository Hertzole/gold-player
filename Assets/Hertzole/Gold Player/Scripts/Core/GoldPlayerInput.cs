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
        private InputItem[] m_Inputs = new InputItem[]
        {
            new InputItem("Horizontal", "Horizontal", KeyCode.None),
            new InputItem("Vertical", "Vertical", KeyCode.None),
            new InputItem("Mouse X", "Mouse X", KeyCode.None),
            new InputItem("Mouse Y" , "Mouse Y" , KeyCode.None),
            new InputItem("Jump", "Jump", KeyCode.Space),
            new InputItem("Crouch", "Crouch", KeyCode.C),
            new InputItem("Run", "Run", KeyCode.LeftShift),
#if GOLD_PLAYER_INTERACTION
            new InputItem("Interact", "Interact", KeyCode.E),
#endif
#if GOLD_PLAYER_WEAPONS
            new InputItem("Primary Attack", "Primary Attack", KeyCode.Mouse0),
            new InputItem("Secondary Attack", "Secondary Attack", KeyCode.Mouse1),
            new InputItem("Change Weapon Scroll", "Mouse ScrollWheel", KeyCode.None),
            new InputItem("Change Weapon 1", "Change Weapon 1", KeyCode.Alpha1),
            new InputItem("Change Weapon 2", "Change Weapon 2", KeyCode.Alpha2),
            new InputItem("Change Weapon 3", "Change Weapon 3", KeyCode.Alpha3),
            new InputItem("Change Weapon 4", "Change Weapon 4", KeyCode.Alpha4),
            new InputItem("Change Weapon 5", "Change Weapon 5", KeyCode.Alpha5),
            new InputItem("Change Weapon 6", "Change Weapon 6", KeyCode.Alpha6),
            new InputItem("Change Weapon 7", "Change Weapon 7", KeyCode.Alpha7),
            new InputItem("Change Weapon 8", "Change Weapon 8", KeyCode.Alpha8),
            new InputItem("Change Weapon 9", "Change Weapon 9", KeyCode.Alpha9),
#endif
        };

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
