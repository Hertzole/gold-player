using UnityEngine;
using UnityEngine.Rendering;

namespace Hertzole.GoldPlayer
{
    public enum GraphicsOwner { Me = 0, Other = 1 };
    public enum HandleGraphics { EnableTarget = 0, DisableTarget = 1, EnableRenderers = 2, DisableRenderers = 3, ShawdosOnly = 4 }

    [DisallowMultipleComponent]
    [AddComponentMenu("Gold Player/Gold Player Graphics", 10)]
    public class GoldPlayerGraphics : MonoBehaviour
    {
        [System.Serializable]
        public struct GraphicsObject
        {
            [SerializeField]
            [Tooltip("The target object to modify.")]
            private Transform target;
            [SerializeField]
            [Tooltip("If true, it will try to find child renderers of the parent target.")]
            private bool isParent;
            [SerializeField]
            [Tooltip("How the graphics are handled when 'Me' is the owner.")]
            private HandleGraphics whenMyGraphics;
            [SerializeField]
            [Tooltip("How the graphics are handled when 'Other' is the owner.")]
            private HandleGraphics whenOtherGraphics;

            [SerializeField]
            [HideInInspector]
            internal Renderer[] renderers;
            [SerializeField]
            [HideInInspector]
            internal ShadowCastingMode[] originalRenderShadows;

            /// <summary> The target object to modify. </summary>
            public Transform Target { get { return target; } set { target = value; FillRenderers(); } }

            /// <summary> If true, it will try to find child renderers of the parent target. </summary>
            public bool IsParent { get { return isParent; } set { isParent = value; FillRenderers(); } }

            /// <summary> How the graphics are handled when 'Me' is the owner. </summary>
            public HandleGraphics WhenMyGraphics { get { return whenMyGraphics; } set { whenMyGraphics = value; } }
            /// <summary> How the graphics are handled when 'Other' is the owner. </summary>
            public HandleGraphics WhenOtherGraphics { get { return whenOtherGraphics; } set { whenOtherGraphics = value; } }

            internal void FillRenderers()
            {
                // If there's no target, stop here.
                if (target == null)
                {
                    renderers = null;
                    return;
                }

                // If it is a parent, get all the child renderers.
                if (isParent)
                {
                    // Get the old renderers to compare.
                    Renderer[] oldRenderers = renderers;

                    // Get all the new renderers.
                    renderers = target.GetComponentsInChildren<Renderer>();

                    bool different = false;
                    // If the old renderers are null or the sizes are different, it's already different.
                    // Else go through each renderer to check if it's the same.
                    if (oldRenderers == null || oldRenderers.Length != renderers.Length)
                    {
                        different = true;
                    }
                    else
                    {
                        for (int i = 0; i < renderers.Length; i++)
                        {
                            if (renderers[i] != oldRenderers[i])
                            {
                                different = true;
                            }
                        }
                    }

                    // If it's different, cache the shadow casting mode.
                    if (different)
                    {
                        originalRenderShadows = new ShadowCastingMode[renderers.Length];
                        for (int i = 0; i < renderers.Length; i++)
                        {
                            originalRenderShadows[i] = renderers[i].shadowCastingMode;
                        }
                    }
                }
                else
                {
                    // If it isn't a parent, get a renderer on the target.
                    Renderer ren = target.GetComponent<Renderer>();
                    // If it found a renderer, add it to the renderers.
                    // Else just mark it as null.
                    if (ren != null)
                    {
                        renderers = new Renderer[1] { ren };
                        originalRenderShadows = new ShadowCastingMode[1] { ren.shadowCastingMode };
                    }
                    else
                    {
                        renderers = null;
                        originalRenderShadows = null;
                    }
                }
            }
        }

        [SerializeField]
        [Tooltip("Graphics are inivislbe for 'me' and visible for 'others'.")]
        private GraphicsOwner owner = GraphicsOwner.Me;

#if UNITY_EDITOR
        [Space]
#endif

        [SerializeField]
        private GraphicsObject[] objects = null;

        /// <summary> Graphics are inivislbe for 'me' and visible for 'others'. </summary>
        public GraphicsOwner Owner { get { return owner; } set { if (value != owner) { UpdateGraphics(value == GraphicsOwner.Me); } owner = value; } }
        public GraphicsObject[] Objects { get { return objects; } set { objects = value; } }

        // Start is called before the first frame update
        private void Start()
        {
            UpdateGraphics(owner == GraphicsOwner.Me);
        }

        /// <summary>
        /// Updates the graphics of all objects.
        /// </summary>
        private void UpdateGraphics(bool isOwner)
        {
            // Stop here if the are no objects.
            if (objects == null || objects.Length == 0)
            {
                return;
            }

            for (int i = 0; i < objects.Length; i++)
            {
                SetObjectGraphics(objects[i], isOwner ? objects[i].WhenMyGraphics : objects[i].WhenOtherGraphics);
            }
        }

        private void SetObjectGraphics(GraphicsObject target, HandleGraphics handleGraphics)
        {
            switch (handleGraphics)
            {
                case HandleGraphics.EnableTarget:
                    if (target.renderers != null && target.renderers.Length > 0)
                    {
                        for (int i = 0; i < target.renderers.Length; i++)
                        {
                            target.renderers[i].enabled = true;
                            target.renderers[i].shadowCastingMode = target.originalRenderShadows[i];
                        }
                    }

                    if (target.Target != null)
                    {
                        target.Target.gameObject.SetActive(true);
                    }
                    break;
                case HandleGraphics.DisableTarget:
                    if (target.Target != null)
                    {
                        target.Target.gameObject.SetActive(false);
                    }
                    break;
                case HandleGraphics.EnableRenderers:
                    if (target.renderers != null && target.renderers.Length > 0)
                    {
                        for (int i = 0; i < target.renderers.Length; i++)
                        {
                            target.renderers[i].enabled = true;
                            target.renderers[i].shadowCastingMode = target.originalRenderShadows[i];
                        }
                    }

                    if (target.Target != null)
                    {
                        target.Target.gameObject.SetActive(true);
                    }
                    break;
                case HandleGraphics.DisableRenderers:
                    if (target.renderers != null && target.renderers.Length > 0)
                    {
                        for (int i = 0; i < target.renderers.Length; i++)
                        {
                            target.renderers[i].enabled = false;
                        }
                    }

                    if (target.Target != null)
                    {
                        target.Target.gameObject.SetActive(true);
                    }
                    break;
                case HandleGraphics.ShawdosOnly:
                    if (target.renderers != null && target.renderers.Length > 0)
                    {
                        for (int i = 0; i < target.renderers.Length; i++)
                        {
                            target.renderers[i].enabled = true;
                            target.renderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                        }
                    }

                    if (target.Target != null)
                    {
                        target.Target.gameObject.SetActive(true);
                    }
                    break;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            GetStandardComponents();
        }

        private void Reset()
        {
            GetStandardComponents();
        }

        private void GetStandardComponents()
        {
            // If there are any objects, fill their renderers.
            if (objects != null && objects.Length > 0)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    objects[i].FillRenderers();
                }
            }

            // If the owner value is updated through the inspector while playing, update the graphics.
            if (Application.isPlaying)
            {
                UpdateGraphics(owner == GraphicsOwner.Me);
            }
        }
    }
#endif
}
