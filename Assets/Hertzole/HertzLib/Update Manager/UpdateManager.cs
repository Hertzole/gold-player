using System.Collections.Generic;
using UnityEngine;

namespace Hertzole.HertzLib
{
    public interface IUpdate
    {
        void OnUpdate();
    }

    public interface IFixedUpdate
    {
        void OnFixedUpdate();
    }

    public interface ILateUpdate
    {
        void OnLateUpdate();
    }

    public static class UpdateManager
    {
        [DisallowMultipleComponent]
        [HelpURL("https://github.com/Hertzole/HertzLib/wiki/Update-Manager")]
        internal class UpdateManagerBehaviour : MonoBehaviour
        {

            private static UpdateManagerBehaviour instance;
            public static UpdateManagerBehaviour Instance
            {
                get
                {
                    if (!instance && !m_Destroying)
                    {
                        GameObject go = new GameObject("Update Manager");
                        instance = go.AddComponent<UpdateManagerBehaviour>();
                        DontDestroyOnLoad(go);
                    }

                    return instance;
                }
            }

            // Sets if the object is in the process of being destroyed.
            private static bool m_Destroying = false;

            private List<IFixedUpdate> m_FixedUpdateList = new List<IFixedUpdate>();
            private static List<IFixedUpdate> FixedUpdateList { get { return Instance.m_FixedUpdateList; } set { Instance.m_FixedUpdateList = value; } }
            private List<ILateUpdate> m_LateUpdateList = new List<ILateUpdate>();
            private static List<ILateUpdate> LateUpdateList { get { return Instance.m_LateUpdateList; } set { Instance.m_LateUpdateList = value; } }
            private List<IUpdate> m_UpdateList = new List<IUpdate>();
            private static List<IUpdate> UpdateList { get { return Instance.m_UpdateList; } set { Instance.m_UpdateList = value; } }

            private void OnDestroy()
            {
                // Mark the object as destroyed.
                m_Destroying = true;
            }

            private void OnApplicationQuit()
            {
                // Mark the object as destroyed.
                m_Destroying = true;
            }

            // This function is called every fixed framerate frame, if the MonoBehaviour is enabled
            private void FixedUpdate()
            {
                if (m_FixedUpdateList.Count > 0)
                {
                    for (int i = 0; i < m_FixedUpdateList.Count; i++)
                        m_FixedUpdateList[i].OnFixedUpdate();
                }
            }

            // LateUpdate is called every frame, if the Behaviour is enabled
            private void LateUpdate()
            {
                if (m_LateUpdateList.Count > 0)
                {
                    for (int i = 0; i < m_LateUpdateList.Count; i++)
                        m_LateUpdateList[i].OnLateUpdate();
                }
            }

            // Update is called every frame, if the MonoBehaviour is enabled
            private void Update()
            {
                if (m_UpdateList.Count > 0)
                {
                    for (int i = 0; i < m_UpdateList.Count; i++)
                        m_UpdateList[i].OnUpdate();
                }
            }

            /// <summary>
            /// Adds a new component that needs to be called every frame.
            /// </summary>
            /// <param name="update"></param>
            public void AddUpdate(IUpdate update)
            {
                // Don't do anything if the object will be destroyed.
                if (m_Destroying)
                    return;

                UpdateList.Add(update);
            }

            /// <summary>
            /// Removes a new that needs to be called every frame.
            /// </summary>
            /// <param name="update"></param>
            public void RemoveUpdate(IUpdate update)
            {
                // Don't do anything if the object will be destroyed.
                if (m_Destroying)
                    return;

                UpdateList.Remove(update);
            }

            /// <summary>
            /// Adds a new component that needs to be called every fixed framerate frame.
            /// </summary>
            /// <param name="update"></param>
            public void AddFixedUpdate(IFixedUpdate update)
            {
                // Don't do anything if the object will be destroyed.
                if (m_Destroying)
                    return;

                FixedUpdateList.Add(update);
            }

            /// <summary>
            /// Removes a new that needs to be called every fixed framerate frame.
            /// </summary>
            /// <param name="update"></param>
            public void RemoveFixedUpdate(IFixedUpdate update)
            {
                // Don't do anything if the object will be destroyed.
                if (m_Destroying)
                    return;

                FixedUpdateList.Remove(update);
            }

            /// <summary>
            /// Adds a new component that needs to be called every frame.
            /// </summary>
            /// <param name="update"></param>
            public void AddLateUpdate(ILateUpdate update)
            {
                // Don't do anything if the object will be destroyed.
                if (m_Destroying)
                    return;

                LateUpdateList.Add(update);
            }

            /// <summary>
            /// Removes a new that needs to be called every frame.
            /// </summary>
            /// <param name="update"></param>
            public void RemoveLateUpdate(ILateUpdate update)
            {
                // Don't do anything if the object will be destroyed.
                if (m_Destroying)
                    return;

                LateUpdateList.Remove(update);
            }
        }

        /// <summary>
        /// Adds a new component that needs to be called every frame.
        /// </summary>
        /// <param name="update"></param>
        public static void AddUpdate(IUpdate update)
        {
            // Make sure the Update Manager isn't being used in the editor.
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Update Manager can not be used outside the game!");
                return;
            }

            // Add the update on the instance.
            if (UpdateManagerBehaviour.Instance)
                UpdateManagerBehaviour.Instance.AddUpdate(update);
        }

        /// <summary>
        /// Removes a new that needs to be called every frame.
        /// </summary>
        /// <param name="update"></param>
        public static void RemoveUpdate(IUpdate update)
        {
            // Make sure the Update Manager isn't being used in the editor.
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Update Manager can not be used outside the game!");
                return;
            }

            // Remove the update on the instance.
            if (UpdateManagerBehaviour.Instance)
                UpdateManagerBehaviour.Instance.RemoveUpdate(update);
        }

        /// <summary>
        /// Adds a new component that needs to be called every fixed framerate frame.
        /// </summary>
        /// <param name="update"></param>
        public static void AddFixedUpdate(IFixedUpdate update)
        {
            // Make sure the Update Manager isn't being used in the editor.
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Update Manager can not be used outside the game!");
                return;
            }

            // Add the update on the instance.
            if (UpdateManagerBehaviour.Instance)
                UpdateManagerBehaviour.Instance.AddFixedUpdate(update);
        }

        /// <summary>
        /// Removes a new that needs to be called every fixed framerate frame.
        /// </summary>
        /// <param name="update"></param>
        public static void RemoveFixedUpdate(IFixedUpdate update)
        {
            // Make sure the Update Manager isn't being used in the editor.
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Update Manager can not be used outside the game!");
                return;
            }

            // Remove the update on the instance.
            if (UpdateManagerBehaviour.Instance)
                UpdateManagerBehaviour.Instance.RemoveFixedUpdate(update);
        }

        /// <summary>
        /// Adds a new component that needs to be called every frame.
        /// </summary>
        /// <param name="update"></param>
        public static void AddLateUpdate(ILateUpdate update)
        {
            // Make sure the Update Manager isn't being used in the editor.
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Update Manager can not be used outside the game!");
                return;
            }

            // Add the update on the instance.
            if (UpdateManagerBehaviour.Instance)
                UpdateManagerBehaviour.Instance.AddLateUpdate(update);
        }

        /// <summary>
        /// Removes a new that needs to be called every frame.
        /// </summary>
        /// <param name="update"></param>
        public static void RemoveLateUpdate(ILateUpdate update)
        {
            // Make sure the Update Manager isn't being used in the editor.
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Update Manager can not be used outside the game!");
                return;
            }

            // Remove the update on the instance.
            if (UpdateManagerBehaviour.Instance)
                UpdateManagerBehaviour.Instance.RemoveLateUpdate(update);
        }
    }
}
