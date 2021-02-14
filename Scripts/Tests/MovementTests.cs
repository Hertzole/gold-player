using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Hertzole.GoldPlayer.Tests
{
    internal class MovementTests : BaseGoldPlayerTest
    {
        private MovingPlatformsClass Platforms { get { return player.Movement.MovingPlatforms; } }

        /// <summary>
        /// Used to test if the player stops running when CanRun is set to false while running.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CanRunStops()
        {
            Debug.Log("CanRunStops :: START");

            input.moveDirection = new Vector2(0, 1);
            input.isRunning = true;
            player.Movement.Acceleration = 0;

            // Skip 2 frames to make sure the player gets their speed up.
            yield return null;
            yield return null;

            Debug.Log("CanRunStops :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(7f, player.Controller.velocity.z);

            yield return null;
            Debug.Log("CanRunStops :: Set CanRun to false.");
            player.Movement.CanRun = false;
            // Skip 2 frames to make sure the player slows down.
            yield return null;
            yield return null;

            Debug.Log("CanRunStops :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(3f, player.Controller.velocity.z);
        }

        /// <summary>
        /// Used to test if the player starts running when CanRun is set to true when the player should be running.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CanRunContinues()
        {
            Debug.Log("CanRunContinues :: START");

            input.moveDirection = new Vector2(0, 1);
            input.isRunning = true;
            player.Movement.Acceleration = 0;
            player.Movement.CanRun = false;

            // Skip 2 frames to make sure the player gets their speed up.
            yield return null;
            yield return null;

            Debug.Log("CanRunContinues :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(3f, player.Controller.velocity.z);

            yield return null;
            Debug.Log("CanRunContinues :: Set CanRun to true.");
            player.Movement.CanRun = true;
            // Skip 2 frames to make sure the player slows down.
            yield return null;
            yield return null;

            Debug.Log("CanRunContinues :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(7f, player.Controller.velocity.z);
        }

        /// <summary>
        /// Used to test if the player stops moving if CanMoveAround is set to false when moving.
        /// </summary>
        [UnityTest]
        public IEnumerator CanMoveStops()
        {
            Debug.Log("CanMoveStops :: START");

            input.moveDirection = new Vector2(0, 1);
            player.Movement.Acceleration = 0;

            // Skip 2 frames to make sure the player gets their speed up.
            yield return null;
            yield return null;

            Debug.Log("CanMoveStops :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(3f, player.Controller.velocity.z);

            yield return null;
            Debug.Log("CanMoveStops :: Set CanMoveAround to false.");
            player.Movement.CanMoveAround = false;

            // Skip 2 frames to make sure the player slows down.
            yield return null;
            yield return null;

            Debug.Log("CanMoveStops :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(0f, player.Controller.velocity.z);
        }

        /// <summary>
        /// Used to test if the player starts moving when CanMoveAround is set to true when the player should be moving.
        /// </summary>
        [UnityTest]
        public IEnumerator CanMoveContinues()
        {
            Debug.Log("CanMoveContinues :: START");

            input.moveDirection = new Vector2(0, 1);
            player.Movement.Acceleration = 0;
            player.Movement.CanMoveAround = false;

            // Skip 2 frames to make sure the player gets their speed up.
            yield return null;
            yield return null;

            Debug.Log("CanMoveContinues :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(0f, player.Controller.velocity.z);

            yield return null;
            Debug.Log("CanMoveContinues :: Set CanMoveAround to true.");
            player.Movement.CanMoveAround = true;

            // Skip 2 frames to make sure the player slows down.
            yield return null;
            yield return null;

            Debug.Log("CanMoveContinues :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(3f, player.Controller.velocity.z);
        }

        /// <summary>
        /// Used to test if the player starts moving when CanMoveAround is set to true when the player should be moving and running.
        /// </summary>
        [UnityTest]
        public IEnumerator CanMoveRunningContinues()
        {
            Debug.Log("CanMoveRunningContinues :: START");

            input.moveDirection = new Vector2(0, 1);
            input.isRunning = true;
            player.Movement.Acceleration = 0;
            player.Movement.CanMoveAround = false;

            // Skip 2 frames to make sure the player gets their speed up.
            yield return null;
            yield return null;

            Debug.Log("CanMoveRunningContinues :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(0f, player.Controller.velocity.z);

            yield return null;
            Debug.Log("CanMoveRunningContinues :: Set CanMoveAround to true.");
            player.Movement.CanMoveAround = true;

            // Skip 2 frames to make sure the player slows down.
            yield return null;
            yield return null;

            Debug.Log("CanMoveRunningContinues :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(7f, player.Controller.velocity.z);
        }

        /// <summary>
        /// Used to test if CanMoveAround even works.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CanMoveAround()
        {
            Debug.Log("CanMoveAround :: START");

            input.moveDirection = new Vector2(0, 1);
            player.Movement.Acceleration = 0;
            player.Movement.CanMoveAround = false;

            // Skip 2 frames to let the simulation play a bit.
            yield return null;
            yield return null;

            Debug.Log("CanMoveAround :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(0f, player.Controller.velocity.z);

            yield return null;
            Debug.Log("CanMoveAround :: Set CanMoveAround to false.");
            player.Movement.CanMoveAround = true;

            // Skip 2 frames to make sure the player gets their speed up.
            yield return null;
            yield return null;

            Debug.Log("CanMoveAround :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(3f, player.Controller.velocity.z);
        }

        /// <summary>
        /// Used to test if CanLookAround even works.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CanLookAround()
        {
            Debug.Log("CanLookAround:: START");

            input.mouseInput = new Vector2(10, -10);
            player.Camera.CanLookAround = true;

            yield return null;

            Debug.Log("CanLookAround :: Camera rotation: " + player.Camera.CameraHead.localEulerAngles.x + " | Player rotation: " + player.transform.eulerAngles.y);

            Assert.AreApproximatelyEqual(20f, player.Camera.CameraHead.localEulerAngles.x, 0.1f);
            Assert.AreApproximatelyEqual(20f, player.transform.eulerAngles.y, 0.1f);

            // Let one more frame go before we disable CanLookAround.
            yield return null;
            Debug.Log("CanLookAround :: Set CanLookAround to false.");
            player.Camera.CanLookAround = false;
            yield return null;

            Debug.Log("CanLookAround :: Camera rotation: " + player.Camera.CameraHead.localEulerAngles.x + " | Player rotation: " + player.transform.eulerAngles.y);

            Assert.AreApproximatelyEqual(40f, player.Camera.CameraHead.localEulerAngles.x, 0.1f);
            Assert.AreApproximatelyEqual(40f, player.transform.eulerAngles.y, 0.1f);
        }

        /// <summary>
        /// Used to test if CanRun even works.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CanRun()
        {
            Debug.Log("CanRun :: START");

            input.moveDirection = new Vector2(0, 1);
            input.isRunning = true;
            player.Movement.Acceleration = 0;
            player.Movement.CanRun = false;

            // Skip 2 frames to let the simulation play a bit.
            yield return null;
            yield return null;

            Debug.Log("CanRun :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(3f, player.Controller.velocity.z);

            yield return null;
            Debug.Log("CanRun :: Set CanRun to true.");
            player.Movement.CanRun = true;

            // Skip 2 frames to make sure the player gets their speed up.
            yield return null;
            yield return null;

            Debug.Log("CanRun :: Player velocity: " + player.Controller.velocity.z);

            Assert.AreApproximatelyEqual(7f, player.Controller.velocity.z);
        }

        /// <summary>
        /// Used to test if CanJump even works.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CanJump()
        {
            Debug.Log("CanJump :: START");

            input.isJumping = true;

            // Skip a frame to let the simulation play a bit.
            yield return null;

            Debug.Log("CanJump :: Player velocity: " + player.Controller.velocity.y);

            Assert.AreApproximatelyEqual(8.944f, player.Controller.velocity.y, 0.1f);

            while (player.Movement.IsJumping || !player.Movement.IsGrounded)
            {
                yield return null;
            }

            Debug.Log("CanJump :: Set CanJump to false.");
            player.Movement.CanJump = false;
            yield return new WaitForSecondsRealtime(0.3f); // Give plenty of time for the player to settle.

            input.isJumping = true;

            yield return null;

            Debug.Log("CanJump :: Player velocity: " + player.Controller.velocity.y);

            Assert.AreApproximatelyEqual(0, player.Controller.velocity.y, 0.1f);
        }

        /// <summary>
        /// Used to test if CanCrouch even works.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CanCrouch()
        {
            Debug.Log("CanCrouch :: START");

            float originalHeight = player.Controller.height;
            input.isCrouching = true;

            // Skip a frame to let the simulation play a bit.
            yield return null;

            Debug.Log("CanCrouch :: Player height: " + player.Controller.height);

            Assert.AreApproximatelyEqual(player.Movement.CrouchHeight, player.Controller.height);

            Debug.Log("CanCrouch :: Set isCrouching to false.");
            input.isCrouching = false;

            yield return null;

            Debug.Log("CanCrouch :: Player height: " + player.Controller.height);

            Assert.AreApproximatelyEqual(originalHeight, player.Controller.height);

            yield return null;

            Debug.Log("CanCrouch :: Set CanCrouch to false and isCrouching to true.");
            player.Movement.CanCrouch = false;
            input.isCrouching = true;

            yield return null;

            Debug.Log("CanCrouch :: Player height: " + player.Controller.height);
            // It should no longer crouch.
            Assert.AreApproximatelyEqual(originalHeight, player.Controller.height);
        }

        /// <summary>
        /// Used to test if Invert X works on the camera.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CameraInvertX()
        {
            Debug.Log("CameraInvertX :: START");

            input.mouseInput = new Vector2(10, 0);
            player.Camera.InvertXAxis = true;

            yield return null;

            Debug.Log("CameraInvertX :: Player rotation: " + player.transform.eulerAngles.y);

            Assert.AreApproximatelyEqual(340f, player.transform.eulerAngles.y, 0.1f);

            Debug.Log("CameraInvertX :: Set InvertX to false.");
            player.Camera.InvertXAxis = false;

            // Let 2 frames pass to get past the 0 rotation.
            yield return null;
            yield return null;

            Debug.Log("CameraInvertX :: Player rotation: " + player.transform.eulerAngles.y);

            Assert.AreApproximatelyEqual(20f, player.transform.eulerAngles.y, 0.1f);
        }

        /// <summary>
        /// Used to test if Invert Y works on the camera.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator CameraInvertY()
        {
            Debug.Log("CameraInvertY :: START");

            input.mouseInput = new Vector2(0, -10);
            player.Camera.InvertYAxis = true;

            yield return null;

            Debug.Log("CameraInvertY :: Camera rotation: " + player.Camera.CameraHead.localEulerAngles.x);

            Assert.AreApproximatelyEqual(340f, player.Camera.CameraHead.localEulerAngles.x, 0.1f);

            Debug.Log("CameraInvertY :: Set InvertY to false.");
            player.Camera.InvertYAxis = false;

            // Let 2 frames pass to get past the 0 rotation.
            yield return null;
            yield return null;

            Debug.Log("CameraInvertY :: Camera rotation: " + player.Camera.CameraHead.localEulerAngles.x);

            Assert.AreApproximatelyEqual(20f, player.Camera.CameraHead.localEulerAngles.x, 0.1f);
        }

        /// <summary>
        /// Used to test if the player follows along with a moving platform.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator FollowMovingPlatform()
        {
            Debug.Log("FollowMovingPlatform :: START");
            Debug.Log("FollowingMovingPlatform :: Set Enabled to true.");
            player.Movement.MovingPlatforms.Enabled = true;

            yield return null;
            AreApproximatelyEqualVector3(new Vector3(0, 0.08f, 0f), player.transform.position, 0.1f);
            yield return null;

            for (int i = 0; i < sceneObjects.Count; i++)
            {
                sceneObjects[i].transform.position += new Vector3(200, 0, 200);
                sceneObjects[i].transform.Rotate(Vector3.up * 45, Space.World);
            }

            yield return null;

            AreApproximatelyEqualVector3(new Vector3(200, 0.08f, 200), player.transform.position, 0.1f);
            Assert.AreApproximatelyEqual(45, player.transform.eulerAngles.y);

            yield return null;
            Debug.Log("FollowingMovingPlatform :: Set Enabled to false.");
            player.Movement.MovingPlatforms.Enabled = false;
            yield return null;

            for (int i = 0; i < sceneObjects.Count; i++)
            {
                sceneObjects[i].transform.position -= new Vector3(200, 0, 200);
                sceneObjects[i].transform.Rotate(Vector3.up * -45, Space.World);
            }

            yield return null;

            AreApproximatelyEqualVector3(new Vector3(200, 0f, 200), player.transform.position, 0.1f);
        }

        /// <summary>
        /// Used to test if the player follows a moving platform position.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator MovingPlatformPosition()
        {
            Debug.Log("MovingPlatformPosition :: START");

            Debug.Log("MovingPlatformPosition :: Set MovePosition to true.");
            player.Movement.MovingPlatforms.MovePosition = true;

            yield return null;
            AreApproximatelyEqualVector3(new Vector3(0, 0.08f, 0f), player.transform.position, 0.1f);
            yield return null;

            for (int i = 0; i < sceneObjects.Count; i++)
            {
                sceneObjects[i].transform.position += new Vector3(200, 0, 200);
            }

            yield return null;

            AreApproximatelyEqualVector3(new Vector3(200, 0.08f, 200), player.transform.position, 0.1f);

            yield return null;
            Debug.Log("MovingPlatformPosition :: Set MovePosition to false.");
            player.Movement.MovingPlatforms.MovePosition = false;
            yield return null;

            for (int i = 0; i < sceneObjects.Count; i++)
            {
                sceneObjects[i].transform.position -= new Vector3(200, 0, 200);
            }

            yield return null;

            AreApproximatelyEqualVector3(new Vector3(200, 0, 200), player.transform.position, 0.1f);
        }

        /// <summary>
        /// Used to test if the player follows a moving platform rotation.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator MovingPlatformRotation()
        {
            Debug.Log("MovingPlatformRotation :: START");

            Debug.Log("MovingPlatformRotation :: Set MoveRotation to true.");
            player.Movement.MovingPlatforms.MoveRotation = true;

            yield return null;
            AreApproximatelyEqualVector3(new Vector3(0, 0.08f, 0f), player.transform.position, 0.1f);
            yield return null;

            for (int i = 0; i < sceneObjects.Count; i++)
            {
                sceneObjects[i].transform.Rotate(Vector3.up * 45, Space.World);
            }

            yield return null;

            Assert.AreApproximatelyEqual(45, player.transform.eulerAngles.y);

            yield return null;
            Debug.Log("MovingPlatformRotation :: Set MoveRotation to false.");
            player.Movement.MovingPlatforms.MoveRotation = false;
            yield return null;

            for (int i = 0; i < sceneObjects.Count; i++)
            {
                sceneObjects[i].transform.Rotate(Vector3.up * -45, Space.World);
            }

            yield return null;

            Assert.AreApproximatelyEqual(45, player.transform.eulerAngles.y);
        }

        [UnityTest]
        public IEnumerator MovingPlatformsMaxAngle()
        {
            player.Movement.MovingPlatforms.MovePosition = true;

            yield return null;
            AreApproximatelyEqualVector3(new Vector3(0, 0.08f, 0f), player.transform.position, 0.1f);
            yield return null;

            for (int i = 0; i < sceneObjects.Count; i++)
            {
                sceneObjects[i].transform.position += new Vector3(200, 0, 200);
            }

            yield return null;

            AreApproximatelyEqualVector3(new Vector3(200, 0.08f, 200), player.transform.position, 0.1f);

            yield return null;
            for (int i = 0; i < sceneObjects.Count; i++)
            {
                Vector3 rot = sceneObjects[i].transform.eulerAngles;
                rot.x -= Platforms.MaxAngle - 1;
                sceneObjects[i].transform.eulerAngles = rot;
            }
            yield return null;

            for (int i = 0; i < sceneObjects.Count; i++)
            {
                sceneObjects[i].transform.position -= new Vector3(200, 0, 200);
            }

            yield return null;
            yield return null;

            AreApproximatelyEqualVector3(new Vector3(0, 0.2f, 0), player.transform.position, 0.5f);

            for (int i = 0; i < 20; i++)
            {
                yield return null;
            }

            for (int i = 0; i < sceneObjects.Count; i++)
            {
                Vector3 rot = sceneObjects[i].transform.eulerAngles;
                rot.x -= 5;
                sceneObjects[i].transform.eulerAngles = rot;
            }

            for (int i = 0; i < 20; i++)
            {
                yield return null;
            }

            for (int i = 0; i < sceneObjects.Count; i++)
            {
                sceneObjects[i].transform.position += new Vector3(200, 0, 200);
            }

            yield return null;
            yield return null;

            AreApproximatelyEqualVector3(new Vector3(0, 0.2f, 0), player.transform.position, 0.5f);

            yield return null;
        }

        [UnityTest]
        public IEnumerator MovingPlatformsNoHeadBob()
        {
            player.HeadBob.EnableBob = true;

            for (int i = 0; i < 600; i++)
            {
                for (int j = 0; j < sceneObjects.Count; j++)
                {
                    sceneObjects[j].transform.position = Vector3.MoveTowards(sceneObjects[j].transform.position, sceneObjects[j].transform.position + Vector3.forward, 2 * Time.deltaTime);
                }

                Assert.AreEqual(0, player.HeadBob.BobCycle);

                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator MovingPlatformsSmallMovement()
        {
            player.Movement.MovingPlatforms.Enabled = true;

            for (int i = 0; i < 2000; i++)
            {
                for (int j = 0; j < sceneObjects.Count; j++)
                {
                    sceneObjects[j].transform.position += Vector3.forward * Time.deltaTime * 0.00099f;
                }

                yield return null;
            }

            Assert.AreNotEqual(0, player.transform.position.z);
        }

        [UnityTest]
        public IEnumerator MultiplierTests()
        {
            input.moveDirection = new Vector2(0, -1);

            for (int i = 0; i < 300; i++)
            {
                yield return null;
            }

            AreApproximatelyEqualVector3(new Vector3(0, 0, 2), player.Velocity, 0.1f);

            player.Movement.MoveSpeedMultiplier = 2f;

            for (int i = 0; i < 300; i++)
            {
                yield return null;
            }

            AreApproximatelyEqualVector3(new Vector3(0, 0, 4), player.Velocity, 0.1f);

            input.moveDirection = new Vector2(0, 0);

            for (int i = 0; i < 100; i++)
            {
                yield return null;
            }

            player.SetPosition(Vector3.zero);

            yield return null;
            yield return null;

            input.isJumping = true;

            float highest = 0;

            yield return null;
            yield return null;

            while (!player.Movement.IsGrounded)
            {
                if (player.transform.position.y > highest)
                {
                    highest = player.transform.position.y;
                }

                yield return null;
            }

            Assert.AreApproximatelyEqual(2f, highest, 0.25f);

            yield return null;
            yield return null;

            player.Movement.JumpHeightMultiplier = 2;
            input.isJumping = true;
            highest = 0;

            yield return null;
            yield return null;

            while (!player.Movement.IsGrounded)
            {
                if (player.transform.position.y > highest)
                {
                    highest = player.transform.position.y;
                }

                yield return null;
            }

            Assert.AreApproximatelyEqual(4f, highest, 0.25f);
        }

        [UnityTest]
        public IEnumerator ValidateGravity()
        {
            player.Movement.gravity = -10;
            player.Movement.ForceInitialize(null);
            Assert.AreEqual(player.Movement.Gravity, 10);

            player.Movement.Gravity = -10;
            Assert.AreEqual(player.Movement.Gravity, 10);

            yield return null;
        }

        [UnityTest]
        public IEnumerator ValidateGroundStick()
        {
            player.Movement.groundStick = -10;
            player.Movement.ForceInitialize(null);
            Assert.AreEqual(player.Movement.GroundStick, 10);

            player.Movement.GroundStick = -10;
            Assert.AreEqual(player.Movement.GroundStick, 10);

            yield return null;
        }

        [UnityTest]
        public IEnumerator ValidateGroundCheckRays()
        {
            player.Movement.RayAmount = 0;
            player.Movement.GroundCheck = GroundCheckType.Raycast;
            player.Movement.RayAmount = 10;
            player.Movement.ForceInitialize(null);
            Assert.AreEqual(player.Movement.groundCheckRays.Length, player.Movement.RayAmount + 1);

            Vector3[] rays = new Vector3[0];
            LogAssert.Expect(LogType.Error, "The provided array needs to be the same as Ray Amount + 1 (11)");
            player.Movement.CreateGroundCheckRayCircle(ref rays, Vector3.zero, 0f);

            yield return null;
        }

        [UnityTest]
        public IEnumerator ValidateSmoothedMovementInput()
        {
            yield return RunTimeScaleTest(ValidateSmoohtedMovementInputRoutine(), ValidateSmoohtedMovementInputRoutine());
        }

        private IEnumerator ValidateSmoohtedMovementInputRoutine()
        {
            input.moveDirection = new Vector2(0, 1);
            for (int i = 0; i < 100; i++)
            {
                yield return null;
            }

            AreApproximatelyEqualVector2(new Vector2(0, 1), player.Movement.SmoothedMovementInput, 0.1f);
            input.moveDirection = new Vector2(1, 1);

            for (int i = 0; i < 100; i++)
            {
                yield return null;
            }

            AreApproximatelyEqualVector2(new Vector2(0.7f, 0.7f), player.Movement.SmoothedMovementInput, 0.1f);
        }

        [UnityTest]
        public IEnumerator HitHeadOnCeiling()
        {
            GameObject prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prim.transform.position = new Vector3(0, 5, 0);
            sceneObjects.Add(prim);
            player.Movement.JumpHeightMultiplier = 10;

            yield return RunTimeScaleTest(HitHeadOnCeilingRoutine(), HitHeadOnCeilingRoutine());
        }

        private IEnumerator HitHeadOnCeilingRoutine()
        {
            input.isJumping = true;

            yield return null;
            yield return null;

            int framesStuck = 0;

            while (!player.Movement.IsGrounded)
            {
                if (player.transform.position.y > 2.35f)
                {
                    framesStuck++;
                }
                Assert.IsFalse(framesStuck > 10);
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator CheckIsRunning()
        {
            yield return RunTimeScaleTest(CheckIsRunningRoutine(), CheckIsRunningRoutine());
        }

        private IEnumerator CheckIsRunningRoutine()
        {
            input.moveDirection = new Vector2(0, -1);
            input.isRunning = true;

            for (int i = 0; i < 60; i++)
            {
                yield return null;
            }

            Assert.IsTrue(player.Movement.IsRunning);
        }

        [UnityTest]
        public IEnumerator CameraOnlyRotateTest()
        {
            yield return RunTimeScaleTest(CameraOnlyRotateTestRoutine(), CameraOnlyRotateTestRoutine());
        }

        private IEnumerator CameraOnlyRotateTestRoutine()
        {
            player.Camera.BodyAngle = -180;
            input.moveDirection = new Vector2(0, -1);
            input.mouseInput = new Vector2(0, -100);

            yield return null;

            player.Camera.RotateCameraOnly = true;

            for (int i = 0; i < 60; i++)
            {
                yield return null;
            }

            Assert.IsTrue(player.transform.position.z > 0);
            Assert.IsTrue(player.transform.position.x < 0.5f);
            Assert.IsTrue(player.transform.position.x > -0.5f);
        }
    }
}
