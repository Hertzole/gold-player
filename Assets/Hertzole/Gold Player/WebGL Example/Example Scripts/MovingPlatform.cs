using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Hertzole.GoldPlayer.Example
{
    [AddComponentMenu("Gold Player/Examples/Moving Platform", 100)]
    public class MovingPlatform : MonoBehaviour
    {
        [System.Serializable]
        public struct Waypoint : IEquatable<Waypoint>
        {
            [FormerlySerializedAs("m_Position")]
            public Vector3 position;
            public Vector3 rotation;
            [FormerlySerializedAs("m_WaitTime")]
            public float waitTime;

            public override bool Equals(object obj)
            {
                return obj is Waypoint waypoint && Equals(waypoint);
            }

            public bool Equals(Waypoint other)
            {
                return position.Equals(other.position) && rotation.Equals(other.rotation) && waitTime == other.waitTime;
            }

            public override int GetHashCode()
            {
                int hashCode = 2032512538;
                hashCode = hashCode * -1521134295 + position.GetHashCode();
                hashCode = hashCode * -1521134295 + rotation.GetHashCode();
                hashCode = hashCode * -1521134295 + waitTime.GetHashCode();
                return hashCode;
            }

            public static bool operator ==(Waypoint left, Waypoint right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Waypoint left, Waypoint right)
            {
                return !(left == right);
            }
        }

        [SerializeField]
        [FormerlySerializedAs("m_Waypoints")]
        private List<Waypoint> waypoints = new List<Waypoint>();
        [SerializeField]
        [FormerlySerializedAs("m_StartingWaypoint")]
        private int startingWaypoint = 0;
        [SerializeField]
        [FormerlySerializedAs("m_MoveSpeed")]
        private float moveSpeed = 5.0f;
        [SerializeField]
        private float rotateSpeed = 180f;
#if UNITY_EDITOR
        [SerializeField]
        [FormerlySerializedAs("m_GizmosColor")]
        private Color gizmosColor = Color.red;
#endif
        private int currentWaypoint = 0;
        private float nextMoveTime = 0;

        // Use this for initialization
        void Start()
        {
            currentWaypoint = startingWaypoint;
            if (waypoints.Count > 0)
            {
                nextMoveTime = Time.time + waypoints[currentWaypoint].waitTime;
                transform.position = waypoints[currentWaypoint].position;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (waypoints.Count > 0)
            {
                if (Time.time >= nextMoveTime)
                {
                    transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, moveSpeed * Time.deltaTime);
                    transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, waypoints[currentWaypoint].rotation, rotateSpeed * Time.deltaTime);
                }

                if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) == 0f)
                {
                    NextWaypoint();
                }
            }
        }

        void NextWaypoint()
        {
            nextMoveTime = Time.time + waypoints[currentWaypoint].waitTime;

            currentWaypoint++;

            if (currentWaypoint == waypoints.Count)
            {
                currentWaypoint = 0;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmosColor;

            if (waypoints != null)
            {
                if (waypoints.Count > 0)
                {
                    foreach (Waypoint pos in waypoints)
                    {
                        Gizmos.DrawCube(pos.position, new Vector3(0.5f, 0.5f, 0.5f));
                    }

                    for (int i = 0; i < waypoints.Count; i++)
                    {
                        if (i == waypoints.Count - 1)
                        {
                            Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                        }
                        else
                        {
                            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
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
            waypointsList = new ReorderableList(serializedObject, serializedObject.FindProperty("waypoints"), true, true, true, true)
            {
                elementHeight = (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * 2
            };
            waypointsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = waypointsList.serializedProperty.GetArrayElementAtIndex(index);

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 70, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("position"), GUIContent.none);
                EditorGUI.LabelField(new Rect(rect.x + rect.width - 65, rect.y, rect.width - 65, EditorGUIUtility.singleLineHeight), "T");
                EditorGUI.PropertyField(new Rect(rect.x + rect.width - 50, rect.y, 50, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("waitTime"), GUIContent.none);
                rect.y += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, rect.height), element.FindPropertyRelative("rotation"), GUIContent.none);
            };

            waypointsList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Waypoints");
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startingWaypoint"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotateSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gizmosColor"));
            EditorGUILayout.Space();
            waypointsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
