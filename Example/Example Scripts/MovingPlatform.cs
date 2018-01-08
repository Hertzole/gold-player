using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;

namespace Hertzole.GoldPlayer.Example
{
    [AddComponentMenu("Gold Player/Examples/Moving Platform", 20)]
    public class MovingPlatform : MonoBehaviour
    {
        [System.Serializable]
        public struct Waypoint
        {
            [SerializeField]
            private Vector3 m_Position;
            [SerializeField]
            private float m_WaitTime;

            public Vector3 Position { get { return m_Position; } set { m_Position = value; } }
            public float WaitTime { get { return m_WaitTime; } set { m_WaitTime = value; } }
        }

        [SerializeField]
        private List<Waypoint> m_Waypoints = new List<Waypoint>();
        [SerializeField]
        private int m_StartingWaypoint = 0;
        [SerializeField]
        private float m_MoveSpeed = 5.0f;
#if UNITY_EDITOR
        [SerializeField]
        private Color m_GizmosColor = Color.red;
#endif
        private int m_CurrentWaypoint = 0;
        private float m_NextMoveTime;

        // Use this for initialization
        void Start()
        {
            m_CurrentWaypoint = m_StartingWaypoint;
            m_NextMoveTime = Time.time + m_Waypoints[m_CurrentWaypoint].WaitTime;
            transform.position = m_Waypoints[m_CurrentWaypoint].Position;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time >= m_NextMoveTime)
                transform.position = Vector3.MoveTowards(transform.position, m_Waypoints[m_CurrentWaypoint].Position, m_MoveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, m_Waypoints[m_CurrentWaypoint].Position) == 0)
            {
                NextWaypoint();
            }
        }

        void NextWaypoint()
        {
            m_NextMoveTime = Time.time + m_Waypoints[m_CurrentWaypoint].WaitTime;

            m_CurrentWaypoint++;

            if (m_CurrentWaypoint == m_Waypoints.Count)
                m_CurrentWaypoint = 0;
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = m_GizmosColor;

            if (m_Waypoints != null)
            {
                if (m_Waypoints.Count > 0)
                {
                    foreach (Waypoint pos in m_Waypoints)
                    {
                        Gizmos.DrawCube(pos.Position, new Vector3(0.5f, 0.5f, 0.5f));
                    }

                    for (int i = 0; i < m_Waypoints.Count; i++)
                    {
                        if (i == m_Waypoints.Count - 1)
                        {
                            Gizmos.DrawLine(m_Waypoints[i].Position, m_Waypoints[0].Position);
                        }
                        else
                        {
                            Gizmos.DrawLine(m_Waypoints[i].Position, m_Waypoints[i + 1].Position);
                        }
                    }
                }
            }
        }
#endif
    }
}

#if UNITY_EDITOR
namespace Hertzole.GoldPlayer.Example.Editor
{
    [CustomEditor(typeof(MovingPlatform))]
    public class MovingPlatformEditor : UnityEditor.Editor
    {
        private ReorderableList waypointsList;

        void OnEnable()
        {
            waypointsList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_Waypoints"), true, true, true, true);
            waypointsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = waypointsList.serializedProperty.GetArrayElementAtIndex(index);

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 70, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Position"), GUIContent.none);
                EditorGUI.LabelField(new Rect(rect.x + rect.width - 65, rect.y, rect.width - 65, EditorGUIUtility.singleLineHeight), "T");
                EditorGUI.PropertyField(new Rect(rect.x + rect.width - 50, rect.y, 50, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_WaitTime"), GUIContent.none);
            };

            waypointsList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Waypoints");
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_StartingWaypoint"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_MoveSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_GizmosColor"));
            EditorGUILayout.Space();
            waypointsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
