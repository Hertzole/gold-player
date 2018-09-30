using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GoldPlayerTests
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator MovingPlatformGeneratesGarbage()
        {
            // Use the Assert class to test conditions
            GameObject playerGO = new GameObject("[TEST] Test Player");
            yield return null;
        }
    }
}
