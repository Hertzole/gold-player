#if !UNITY_2019_2_OR_NEWER || (UNITY_2019_2_OR_NEWER && USE_UGUI)
#define USE_GUI
#endif

using System.Reflection;
using UnityEngine;
#if USE_GUI
using UnityEngine.Serialization;
using UnityEngine.UI;
#endif

namespace Hertzole.GoldPlayer.Example
{
    [AddComponentMenu("Gold Player/Examples/Gold Player Tweaker Field", 22)]
    public class GoldPlayerTweakField : MonoBehaviour
    {
#if USE_GUI
        [SerializeField]
        [FormerlySerializedAs("m_Label")]
        private Text label;
        public Text Label { get { return label; } set { label = value; } }
        [SerializeField]
        [FormerlySerializedAs("m_TextField")]
        private InputField textField;
        public InputField TextField { get { return textField; } set { textField = value; } }
        [SerializeField]
        [FormerlySerializedAs("m_ToggleField")]
        private Toggle toggleField;
        public Toggle ToggleField { get { return toggleField; } set { toggleField = value; } }
        [SerializeField]
        [FormerlySerializedAs("m_SliderField")]
        private Slider sliderField;
        public Slider SliderField { get { return sliderField; } set { sliderField = value; } }
#endif

        public void SetupField(string label, PropertyInfo info, object caller, bool slider = false, float minSliderNum = 0, float maxSliderNum = 1)
        {
#if USE_GUI
            textField.gameObject.SetActive(false);
            toggleField.gameObject.SetActive(false);
            sliderField.gameObject.SetActive(false);
            this.label.text = label;
            gameObject.SetActive(true);

            if (info.PropertyType == typeof(bool))
            {
                toggleField.gameObject.SetActive(true);
                toggleField.isOn = (bool)info.GetValue(caller, null);

                toggleField.onValueChanged.AddListener(delegate
                { info.SetValue(caller, toggleField.isOn, null); });
            }

            if (info.PropertyType == typeof(float) || info.PropertyType == typeof(int))
            {
                bool isInt = info.PropertyType == typeof(int);

                float floatValue = 0;
                int intValue = 0;

                if (isInt)
                    intValue = (int)info.GetValue(caller, null);
                else
                    floatValue = (float)info.GetValue(caller, null);

                if (slider)
                {
                    textField.gameObject.SetActive(false);
                    sliderField.gameObject.SetActive(true);

                    sliderField.minValue = minSliderNum;
                    sliderField.maxValue = maxSliderNum;
                    sliderField.value = isInt ? intValue : floatValue;
                    sliderField.wholeNumbers = isInt;
                    this.label.text = label + ": " + sliderField.value.ToString("F3");
                    sliderField.onValueChanged.AddListener(delegate
                    {
                        info.SetValue(caller, isInt ? Mathf.RoundToInt(sliderField.value) : sliderField.value, null);
                        this.label.text = label + ": " + sliderField.value.ToString("F3");
                    });
                }
                else
                {
                    textField.gameObject.SetActive(true);
                    sliderField.gameObject.SetActive(false);

                    textField.contentType = isInt ? InputField.ContentType.IntegerNumber : InputField.ContentType.DecimalNumber;
                    textField.text = (isInt ? intValue : floatValue).ToString();

                    textField.onValueChanged.AddListener(delegate
                    {
                        if (isInt)
                            info.SetValue(caller, int.Parse(textField.text), null);
                        else
                            info.SetValue(caller, float.Parse(textField.text), null);
                    });
                }
            }
#endif
        }
    }
}
