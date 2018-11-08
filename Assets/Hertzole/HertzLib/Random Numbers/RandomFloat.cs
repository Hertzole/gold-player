using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hertzole.HertzLib
{
    [System.Serializable]
    public struct RandomFloat
    {
        [SerializeField]
        private float m_Min;
        public float Min { get { return m_Min; } set { m_Min = value; } }
        [SerializeField]
        private float m_Max;
        public float Max { get { return m_Max; } set { m_Max = value; } }

        public float Value { get { return Random.Range(Min, Max); } }

        public RandomFloat(float min, float max)
        {
            m_Min = min;
            m_Max = max;
        }

        public static bool operator ==(int x, RandomFloat y)
        {
            return x == y.Value;
        }

        public static bool operator ==(RandomFloat x, int y)
        {
            return x.Value == y;
        }

        public static bool operator ==(RandomFloat x, RandomFloat y)
        {
            return x.Value == y.Value;
        }

        public static bool operator !=(int x, RandomFloat y)
        {
            return x != y.Value;
        }

        public static bool operator !=(RandomFloat x, int y)
        {
            return x.Value != y;
        }

        public static bool operator !=(RandomFloat x, RandomFloat y)
        {
            return x.Value != y.Value;
        }

        public static float operator +(float x, RandomFloat y)
        {
            return x + y.Value;
        }

        public static float operator +(RandomFloat x, float y)
        {
            return x.Value + y;
        }

        public static float operator +(RandomFloat x, RandomFloat y)
        {
            return x.Value + y.Value;
        }

        public static float operator -(float x, RandomFloat y)
        {
            return x - y.Value;
        }

        public static float operator -(RandomFloat x, float y)
        {
            return x.Value - y;
        }

        public static float operator -(RandomFloat x, RandomFloat y)
        {
            return x.Value - y.Value;
        }

        public static float operator /(float x, RandomFloat y)
        {
            return x / y.Value;
        }

        public static float operator /(RandomFloat x, float y)
        {
            return x.Value / y;
        }

        public static float operator /(RandomFloat x, RandomFloat y)
        {
            return x.Value / y.Value;
        }

        public static float operator *(float x, RandomFloat y)
        {
            return x * y.Value;
        }

        public static float operator *(RandomFloat x, float y)
        {
            return x.Value * y;
        }

        public static float operator *(RandomFloat x, RandomFloat y)
        {
            return x.Value * y.Value;
        }

        public static float operator %(float x, RandomFloat y)
        {
            return x % y.Value;
        }

        public static float operator %(RandomFloat x, float y)
        {
            return x.Value % y;
        }

        public static float operator %(RandomFloat x, RandomFloat y)
        {
            return x.Value % y.Value;
        }

        public static float operator *(Vector3 v, RandomFloat y)
        {
            return v * y;
        }

        public static float operator *(RandomFloat x, Vector3 v)
        {
            return x * v;
        }

        public override bool Equals(object obj)
        {
            try
            {
                return Value == ((RandomFloat)obj).Value;
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToString(string format)
        {
            return Value.ToString(format);
        }

        public string ToString(System.IFormatProvider provider)
        {
            return Value.ToString(provider);
        }

        public string ToString(string format, System.IFormatProvider provider)
        {
            return Value.ToString(format, provider);
        }
    }
}

#if UNITY_EDITOR
namespace Hertzole.HertzLib.Editor
{
    [CustomPropertyDrawer(typeof(RandomFloat))]
    public class RandomFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height), label);
            EditorGUI.PrefixLabel(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, 25, position.height), new GUIContent("Min"));
            EditorGUI.PropertyField(new Rect(position.x + EditorGUIUtility.labelWidth + 25, position.y, ((position.width - EditorGUIUtility.labelWidth) / 2) - 27, position.height), property.FindPropertyRelative("m_Min"), GUIContent.none);
            EditorGUI.PrefixLabel(new Rect(position.x + EditorGUIUtility.labelWidth + ((position.width - EditorGUIUtility.labelWidth) / 2) + 2, position.y, 27, position.height), new GUIContent("Max"));
            EditorGUI.PropertyField(new Rect(position.x + EditorGUIUtility.labelWidth + ((position.width - EditorGUIUtility.labelWidth) / 2) + 31, position.y, ((position.width - EditorGUIUtility.labelWidth) / 2) - 31, position.height), property.FindPropertyRelative("m_Max"), GUIContent.none);
        }
    }
}
#endif
