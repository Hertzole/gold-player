using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hertzole.HertzLib
{
    [System.Serializable]
    public struct RandomInt
    {
        [SerializeField]
        private int m_Min;
        public int Min { get { return m_Min; } set { m_Min = value; } }
        [SerializeField]
        private int m_Max;
        public int Max { get { return m_Max; } set { m_Max = value; } }

        public int Value { get { return Random.Range(Min, Max); } }

        public RandomInt(int min, int max)
        {
            m_Min = min;
            m_Max = max;
        }

        public static bool operator ==(int x, RandomInt y)
        {
            return x == y.Value;
        }

        public static bool operator ==(RandomInt x, int y)
        {
            return x.Value == y;
        }

        public static bool operator ==(RandomInt x, RandomInt y)
        {
            return x.Value == y.Value;
        }

        public static bool operator !=(int x, RandomInt y)
        {
            return x != y.Value;
        }

        public static bool operator !=(RandomInt x, int y)
        {
            return x.Value != y;
        }

        public static bool operator !=(RandomInt x, RandomInt y)
        {
            return x.Value != y.Value;
        }

        public static int operator +(int x, RandomInt y)
        {
            return x + y.Value;
        }

        public static int operator +(RandomInt x, int y)
        {
            return x.Value + y;
        }

        public static int operator +(RandomInt x, RandomInt y)
        {
            return x.Value + y.Value;
        }

        public static int operator -(int x, RandomInt y)
        {
            return x - y.Value;
        }

        public static int operator -(RandomInt x, int y)
        {
            return x.Value - y;
        }

        public static int operator -(RandomInt x, RandomInt y)
        {
            return x.Value - y.Value;
        }

        public static int operator /(int x, RandomInt y)
        {
            return x / y.Value;
        }

        public static int operator /(RandomInt x, int y)
        {
            return x.Value / y;
        }

        public static int operator /(RandomInt x, RandomInt y)
        {
            return x.Value / y.Value;
        }

        public static int operator *(int x, RandomInt y)
        {
            return x * y.Value;
        }

        public static int operator *(RandomInt x, int y)
        {
            return x.Value * y;
        }

        public static int operator *(RandomInt x, RandomInt y)
        {
            return x.Value * y.Value;
        }

        public static int operator %(int x, RandomInt y)
        {
            return x % y.Value;
        }

        public static int operator %(RandomInt x, int y)
        {
            return x.Value % y;
        }

        public static int operator %(RandomInt x, RandomInt y)
        {
            return x.Value % y.Value;
        }

        public override bool Equals(object obj)
        {
            try
            {
                return Value == ((RandomInt)obj).Value;
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Value;
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
    [CustomPropertyDrawer(typeof(RandomInt))]
    public class RandomIntDrawer : PropertyDrawer
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
