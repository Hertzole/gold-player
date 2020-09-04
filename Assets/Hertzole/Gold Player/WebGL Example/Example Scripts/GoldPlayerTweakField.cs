#if !UNITY_2019_2_OR_NEWER || (UNITY_2019_2_OR_NEWER && GOLD_PLAYER_UGUI)
#define USE_GUI
#endif

using System;
using TMPro;
using UnityEngine;
#if USE_GUI
using UnityEngine.Serialization;
using UnityEngine.UI;
#endif

namespace Hertzole.GoldPlayer.Example
{
    [AddComponentMenu("Gold Player/Examples/Gold Player Tweaker Field", 100)]
    public class GoldPlayerTweakField : MonoBehaviour
    {
#if USE_GUI
        [SerializeField]
        [FormerlySerializedAs("m_Label")]
        private TextMeshProUGUI label;
        public TextMeshProUGUI Label { get { return label; } set { label = value; } }
        [SerializeField]
        [FormerlySerializedAs("m_TextField")]
        private TMP_InputField textField;
        public TMP_InputField TextField { get { return textField; } set { textField = value; } }
        [SerializeField]
        [FormerlySerializedAs("m_ToggleField")]
        private Toggle toggleField;
        public Toggle ToggleField { get { return toggleField; } set { toggleField = value; } }
        [SerializeField]
        [FormerlySerializedAs("m_SliderField")]
        private Slider sliderField;
        public Slider SliderField { get { return sliderField; } set { sliderField = value; } }
#endif

        public void SetInteractable(bool interactable)
        {
            textField.interactable = interactable;
            toggleField.interactable = interactable;
            sliderField.interactable = interactable;
        }

        public void SetupField(string label, Action<bool> valueChanged, bool defaultValue)
        {
#if USE_GUI
            textField.gameObject.SetActive(false);
            sliderField.gameObject.SetActive(false);
            toggleField.gameObject.SetActive(true);
            this.label.text = label;

            toggleField.isOn = defaultValue;
            toggleField.onValueChanged.AddListener(x =>
            {
                if (valueChanged != null)
                {
                    valueChanged.Invoke(x);
                }
            });

            gameObject.SetActive(true);
#endif
        }

        public void SetupField(string label, Action<float> valueChanged, float defaultValue, bool slider = false, float minSlider = 0, float maxSlider = 1, float labelDivide = 1f)
        {
#if USE_GUI
            textField.gameObject.SetActive(!slider);
            sliderField.gameObject.SetActive(slider);
            toggleField.gameObject.SetActive(false);
            this.label.text = label;

            if (slider)
            {
                sliderField.minValue = minSlider;
                sliderField.maxValue = maxSlider;
                sliderField.value = defaultValue;
                sliderField.wholeNumbers = false;
                this.label.text = label + ": " + (sliderField.value / labelDivide).ToString("F3");
                sliderField.onValueChanged.AddListener(x =>
                {
                    this.label.text = label + ": " + (sliderField.value / labelDivide).ToString("F3");
                    if (valueChanged != null)
                    {
                        valueChanged.Invoke(x);
                    }
                });
            }
            else
            {
                textField.text = defaultValue.ToString();
                textField.contentType = TMP_InputField.ContentType.DecimalNumber;
                textField.onValueChanged.AddListener(x =>
                {
                    if (valueChanged != null)
                    {
                        if (string.IsNullOrWhiteSpace(x))
                        {
                            x = "0";
                        }

                        if (float.TryParse(x, out float result))
                        {
                            valueChanged.Invoke(result);
                        }
                    }
                });
            }

            gameObject.SetActive(true);
#endif
        }

        public void SetupField(string label, Action<int> valueChanged, int defaultValue, bool slider = false, int minSlider = 0, int maxSlider = 1, float labelDivide = 1)
        {
#if USE_GUI
            textField.gameObject.SetActive(!slider);
            sliderField.gameObject.SetActive(slider);
            toggleField.gameObject.SetActive(false);
            this.label.text = label;

            if (slider)
            {
                sliderField.minValue = minSlider;
                sliderField.maxValue = maxSlider;
                sliderField.value = defaultValue;
                sliderField.wholeNumbers = true;
                this.label.text = label + ": " + sliderField.value / labelDivide;
                sliderField.onValueChanged.AddListener(x =>
                {
                    this.label.text = label + ": " + sliderField.value / labelDivide;
                    if (valueChanged != null)
                    {
                        valueChanged.Invoke(Mathf.RoundToInt(x));
                    }
                });
            }
            else
            {
                textField.text = defaultValue.ToString();
                textField.contentType = TMP_InputField.ContentType.IntegerNumber;
                textField.onValueChanged.AddListener(x =>
                {
                    if (valueChanged != null)
                    {
                        valueChanged.Invoke(int.Parse(x));
                    }
                });
            }

            gameObject.SetActive(true);
#endif
        }
    }
}
