using System;
using UnityEngine;

namespace Hertzole.GoldPlayer.Core
{
    [Serializable]
    public class InputItem
    {
        [SerializeField]
        private string m_ButtonName;
        public string ButtonName { get { return m_ButtonName; } set { m_ButtonName = value; } }
        [SerializeField]
        private string m_InputName;
        public string InputName { get { return m_InputName; } set { m_InputName = value; } }
        [SerializeField]
        private KeyCode m_Key;
        public KeyCode Key { get { return m_Key; } set { m_Key = value; } }
    }
}
