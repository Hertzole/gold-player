using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Hertzole.GoldPlayer.Example
{
    public class GoldPlayerTweakField : MonoBehaviour
    {
        [SerializeField]
        private Text m_Label;
        public Text Label { get { return m_Label; } set { m_Label = value; } }
        [SerializeField]
        private InputField m_TextField;
        public InputField TextField { get { return m_TextField; } set { m_TextField = value; } }
        [SerializeField]
        private Toggle m_ToggleField;
        public Toggle ToggleField { get { return m_ToggleField; } set { m_ToggleField = value; } }
        [SerializeField]
        private Slider m_SliderField;
        public Slider SliderField { get { return m_SliderField; } set { m_SliderField = value; } }

        public void SetupField(string label, PropertyInfo info, object caller, bool slider = false, float minSliderNum = 0, float maxSliderNum = 1)
        {
            //m_Label.text = ;
            m_TextField.gameObject.SetActive(false);
            m_ToggleField.gameObject.SetActive(false);
            m_SliderField.gameObject.SetActive(false);
            m_Label.text = label;
            gameObject.SetActive(true);

            if (info.PropertyType == typeof(bool))
            {
                m_ToggleField.gameObject.SetActive(true);
                m_ToggleField.isOn = (bool)info.GetValue(caller, null);

                m_ToggleField.onValueChanged.AddListener(delegate { info.SetValue(caller, m_ToggleField.isOn, null); });
            }

            if (info.PropertyType == typeof(float))
            {
                float value = (float)info.GetValue(caller, null);
                if (slider)
                {
                    m_TextField.gameObject.SetActive(false);
                    m_SliderField.gameObject.SetActive(true);

                    m_SliderField.minValue = minSliderNum;
                    m_SliderField.maxValue = maxSliderNum;
                    m_SliderField.value = value;
                    m_Label.text = label + ": " + m_SliderField.value.ToString("F3");
                    m_SliderField.onValueChanged.AddListener(delegate { info.SetValue(caller, m_SliderField.value, null); m_Label.text = label + ": " + m_SliderField.value.ToString("F3"); });
                }
                else
                {
                    m_TextField.gameObject.SetActive(true);
                    m_SliderField.gameObject.SetActive(false);

                    m_TextField.contentType = InputField.ContentType.DecimalNumber;
                    m_TextField.text = value.ToString();

                    m_TextField.onValueChanged.AddListener(delegate { info.SetValue(caller, float.Parse(m_TextField.text), null); });
                }
            }
        }
    }
}
