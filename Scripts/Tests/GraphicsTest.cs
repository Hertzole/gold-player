#if GOLD_PLAYER_TESTS && !GOLD_PLAYER_DISABLE_GRAPHICS
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.TestTools;

namespace Hertzole.GoldPlayer.Tests
{
    internal class GraphicsTest : BaseGoldPlayerTest
    {
        private GoldPlayerGraphics graphics;

        protected override GoldPlayerController SetupPlayer()
        {
            GoldPlayerController player = base.SetupPlayer();

            graphics = player.gameObject.AddComponent<GoldPlayerGraphics>();

            GameObject graphicsParent = new GameObject("[TEST] Player Graphics");
            graphicsParent.transform.SetParent(player.transform);

            GameObject graphicsCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            graphicsCube.transform.SetParent(graphicsParent.transform);
            graphicsCube.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.TwoSided;

            GameObject emptyParent = new GameObject();
            GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            emptyParent.transform.SetParent(graphicsParent.transform);
            newCube.transform.SetParent(emptyParent.transform);

            graphics.Objects = new GoldPlayerGraphics.GraphicsObject[]
            {
                new GoldPlayerGraphics.GraphicsObject()
                {
                    Target = graphicsCube.transform,
                    IsParent = false,
                    WhenMyGraphics = HandleGraphics.DisableTarget,
                    WhenOtherGraphics = HandleGraphics.EnableTarget
                },
                new GoldPlayerGraphics.GraphicsObject()
                {
                    IsParent = true,
                    Target = emptyParent.transform,
                    WhenMyGraphics = HandleGraphics.DisableRenderers,
                    WhenOtherGraphics = HandleGraphics.EnableRenderers
                }
            };

            graphics.Owner = GraphicsOwner.Me;

            return player;
        }

        [UnityTest]
        public IEnumerator TestOwner()
        {
            graphics.Owner = GraphicsOwner.Me;
            Assert.IsFalse(graphics.Objects[0].Target.gameObject.activeSelf);
            graphics.Owner = GraphicsOwner.Other;
            Assert.IsTrue(graphics.Objects[0].Target.gameObject.activeSelf);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestEnableTarget()
        {
            graphics.Objects[0].WhenOtherGraphics = HandleGraphics.EnableTarget;
            graphics.Owner = GraphicsOwner.Other;
            TestObject(graphics.Objects[0].Target, true, true, ShadowCastingMode.TwoSided);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestDisableTarget()
        {
            graphics.Objects[0].WhenOtherGraphics = HandleGraphics.DisableTarget;
            graphics.Owner = GraphicsOwner.Other;
            TestObject(graphics.Objects[0].Target, false, true, ShadowCastingMode.TwoSided);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestEnableRenderers()
        {
            graphics.Objects[0].WhenOtherGraphics = HandleGraphics.EnableRenderers;
            graphics.Owner = GraphicsOwner.Other;
            TestObject(graphics.Objects[0].Target, true, true, ShadowCastingMode.TwoSided);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestDisableRenderers()
        {
            graphics.Objects[0].WhenOtherGraphics = HandleGraphics.DisableRenderers;
            graphics.Owner = GraphicsOwner.Other;
            TestObject(graphics.Objects[0].Target, true, false, ShadowCastingMode.TwoSided);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestShadowsOnly()
        {
            graphics.Objects[0].WhenOtherGraphics = HandleGraphics.ShawdosOnly;
            graphics.Owner = GraphicsOwner.Other;
            TestObject(graphics.Objects[0].Target, true, true, ShadowCastingMode.ShadowsOnly);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestOriginalShadowMode()
        {
            graphics.Objects[0].WhenOtherGraphics = HandleGraphics.ShawdosOnly;
            graphics.Owner = GraphicsOwner.Other;
            TestObject(graphics.Objects[0].Target, true, true, ShadowCastingMode.ShadowsOnly);
            graphics.Objects[0].WhenMyGraphics = HandleGraphics.EnableTarget;
            graphics.Owner = GraphicsOwner.Me;
            TestObject(graphics.Objects[0].Target, true, true, ShadowCastingMode.TwoSided);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestNullTarget()
        {
            Assert.IsNotNull(graphics.Objects[0].renderers);
            yield return null;
            graphics.Objects[0].Target = null;
            Assert.IsNull(graphics.Objects[0].renderers);
        }

        [UnityTest]
        public IEnumerator TestIsParent()
        {
            Assert.IsNotNull(graphics.Objects[1].renderers);
            Assert.AreEqual(graphics.Objects[1].renderers.Length, 1);
            Assert.AreEqual(graphics.Objects[1].renderers[0], graphics.Objects[1].Target.GetChild(0).GetComponent<Renderer>());
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestTargetNulLRenderer()
        {
            GameObject temp = new GameObject();
            graphics.Objects[0].Target = temp.transform;

            sceneObjects.Add(temp);

            Assert.IsNull(graphics.Objects[0].renderers);
            Assert.IsNull(graphics.Objects[0].originalRenderShadows);
            yield return null;
        }

        [UnityTest]
        public IEnumerator EqualsCheck()
        {
            GameObject tempObject = new GameObject();

            sceneObjects.Add(tempObject);

            GoldPlayerGraphics.GraphicsObject a = new GoldPlayerGraphics.GraphicsObject()
            {
                IsParent = false,
                WhenMyGraphics = HandleGraphics.DisableRenderers,
                WhenOtherGraphics = HandleGraphics.EnableRenderers,
                Target = tempObject.transform
            };
            GoldPlayerGraphics.GraphicsObject b = new GoldPlayerGraphics.GraphicsObject()
            {
                IsParent = false,
                WhenMyGraphics = HandleGraphics.DisableRenderers,
                WhenOtherGraphics = HandleGraphics.EnableRenderers,
                Target = tempObject.transform
            };

            Assert.IsTrue(a == b);
            Assert.IsFalse(a.Equals(tempObject));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

            b.IsParent = true;
            Assert.IsTrue(a != b);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestResetComponent()
        {
            graphics.Reset();

            Assert.AreEqual(graphics.Objects.Length, 2);
            Assert.IsNotNull(graphics.Objects[0].renderers);

            yield return null;
        }

        private void TestObject(Transform target, bool active, bool rendererEnabled, ShadowCastingMode shadows)
        {
            if (active)
            {
                Assert.IsTrue(target.gameObject.activeSelf);
            }
            else
            {
                Assert.IsFalse(target.gameObject.activeSelf);
            }

            if (rendererEnabled)
            {
                Assert.IsTrue(target.GetComponent<Renderer>().enabled);
            }
            else
            {
                Assert.IsFalse(target.GetComponent<Renderer>().enabled);
            }

            Assert.AreEqual(target.GetComponent<Renderer>().shadowCastingMode, shadows);
        }
    }
}
#endif