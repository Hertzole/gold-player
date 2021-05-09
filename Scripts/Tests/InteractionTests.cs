#if GOLD_PLAYER_TESTS && !GOLD_PLAYER_DISABLE_INTERACTION
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Hertzole.GoldPlayer.Tests
{
    internal class InteractionTests : BaseGoldPlayerTest
    {
        private GoldPlayerInteractable interactableCube;
        private GoldPlayerInteraction interaction;

        protected override void CreateTestScene()
        {
            base.CreateTestScene();

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "[TEST] Interact Cube";
            cube.transform.position = new Vector3(0f, 1.6f, 2f);

            interactableCube = cube.AddComponent<GoldPlayerInteractable>();
            interactableCube.IsInteractable = true;
            interactableCube.IsHidden = true;
            interactableCube.UseCustomMessage = false;

            sceneObjects.Add(cube);
        }

        protected override GoldPlayerController SetupPlayer()
        {
            GoldPlayerController player = base.SetupPlayer();

            interaction = player.gameObject.AddComponent<GoldPlayerInteraction>();
            interaction.CameraHead = player.Camera.CameraHead;
            interaction.InteractInput = GoldPlayerTestInput.INTERACT;
            interaction.InteractionLayer = 1;

            return player;
        }

        [UnityTest]
        public IEnumerator TestIsInteractable()
        {
            bool interacted = false;

            interactableCube.OnInteract.AddListener(() => interacted = true);
            interactableCube.IsInteractable = true;
            input.isInteracting = true;

            yield return null;

            Assert.IsTrue(interacted);

            interacted = false;
            interactableCube.IsInteractable = false;
            input.isInteracting = true;

            yield return null;

            Assert.IsFalse(interacted);
        }

        [UnityTest]
        public IEnumerator CheckRange()
        {
            bool interacted = false;
            player.GetComponent<GoldPlayerInteraction>().InteractionRange = 3;
            interactableCube.OnInteract.AddListener(() => interacted = true);

            input.isInteracting = true;

            yield return null;

            Assert.IsTrue(interacted);

            interacted = false;
            interactableCube.transform.position = new Vector3(0, 1.6f, 4f);
            input.isInteracting = true;

            yield return null;

            Assert.IsFalse(interacted);
            interacted = false;

            player.GetComponent<GoldPlayerInteraction>().InteractionRange = 5;
            input.isInteracting = true;

            yield return null;

            Assert.IsTrue(interacted);
        }

        [UnityTest]
        public IEnumerator TestIgnoreTriggers()
        {
            bool interacted = false;
            interactableCube.GetComponent<BoxCollider>().isTrigger = true;
            player.GetComponent<GoldPlayerInteraction>().IgnoreTriggers = true;
            interactableCube.OnInteract.AddListener(() => interacted = true);

            input.isInteracting = true;

            yield return null;

            Assert.IsFalse(interacted);

            interacted = false;
            player.GetComponent<GoldPlayerInteraction>().IgnoreTriggers = false;

            input.isInteracting = true;

            yield return null;

            Assert.IsTrue(interacted);

            interacted = false;
            interactableCube.GetComponent<BoxCollider>().isTrigger = false;

            input.isInteracting = true;

            yield return null;

            Assert.IsTrue(interacted);
        }

        [UnityTest]
        public IEnumerator TestMessage()
        {
            GoldPlayerInteraction iPlayer = player.GetComponent<GoldPlayerInteraction>();
            interactableCube.UseCustomMessage = false;

            yield return null;

            string message = iPlayer.CurrentHitInteractable.UseCustomMessage ? iPlayer.CurrentHitInteractable.CustomMessage : iPlayer.InteractMessage;

            Assert.AreEqual(message, iPlayer.InteractMessage);
            interactableCube.UseCustomMessage = true;
            interactableCube.CustomMessage = "new";

            yield return null;

            message = iPlayer.CurrentHitInteractable.UseCustomMessage ? iPlayer.CurrentHitInteractable.CustomMessage : iPlayer.InteractMessage;
            Assert.AreEqual(message, "new");

            interactableCube.UseCustomMessage = false;

            yield return null;

            message = iPlayer.CurrentHitInteractable.UseCustomMessage ? iPlayer.CurrentHitInteractable.CustomMessage : iPlayer.InteractMessage;
            Assert.AreEqual(message, iPlayer.InteractMessage);
        }

        [UnityTest]
        public IEnumerator TestLimits()
        {
            interactableCube.LimitedInteractions = true;
            interactableCube.MaxInteractions = 3;

            bool interacted = false;
            bool reachedEnd = false;
            interactableCube.OnInteract.AddListener(() => interacted = true);
            interactableCube.OnReachedMaxInteractions.AddListener(() => reachedEnd = true);

            for (int i = 0; i < interactableCube.MaxInteractions; i++)
            {
                interacted = false;
                input.isInteracting = true;
                yield return null;
                Assert.AreEqual(i + 1, interactableCube.Interactions);
                Assert.IsTrue(interacted);
                yield return null;
            }

            interacted = false;
            input.isInteracting = true;
            yield return null;

            Assert.IsFalse(interacted);
            Assert.IsTrue(reachedEnd);

            interacted = false;
            interactableCube.Interact();

            yield return null;

            Assert.IsFalse(interacted);
        }

        [UnityTest]
        public IEnumerator TestBypass()
        {
            interactableCube.LimitedInteractions = true;
            interactableCube.MaxInteractions = 1;
            interactableCube.IsInteractable = false;
            bool interacted = false;
            interactableCube.OnInteract.AddListener(() => interacted = true);

            interactableCube.Interact();
            Assert.IsFalse(interacted);
            interactableCube.Interact(true, true);
            Assert.IsTrue(interacted);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestNullCollider()
        {
            interaction.haveCheckedInteractable = false;

            RaycastHit empty = new RaycastHit();
            interaction.HitInteraction(empty);

            Assert.IsFalse(interaction.haveCheckedInteractable);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestRigidbodyGet()
        {
            GameObject go = new GameObject();
            go.AddComponent<BoxCollider>();
            go.transform.position = new Vector3(0, 1.6f, 1.5f);
            go.transform.SetParent(interactableCube.transform);

            sceneObjects.Add(go);
            Rigidbody rig = interactableCube.gameObject.AddComponent<Rigidbody>();
            rig.isKinematic = true;

            input.isInteracting = true;
            yield return null;
            yield return null;

            Assert.IsNotNull(((GoldPlayerInteractable)interaction.CurrentHitInteractable).GetComponent<Rigidbody>());
        }

        [UnityTest]
        public IEnumerator TestReset()
        {
            interaction.CameraHead = null;
            Assert.IsNull(interaction.CameraHead);

            interaction.Reset();
            Assert.IsNotNull(interaction.CameraHead);
            Assert.AreEqual(interaction.CameraHead, player.Camera.CameraHead);

            yield return null;
        }
    }
}
#endif
