#if GOLD_PLAYER_TESTS
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.GoldPlayer.Tests
{
	public class MovementSpeedTests 
	{
		[Test]
		public void TestForwardsMax()
		{
			MovementSpeeds speeds = new MovementSpeeds(5, 5, 5)
			{
				ForwardSpeed = 10
			};

			Assert.AreEqual(speeds.Max, 10);
		}
		
		[Test]
		public void TestSidewaysMax()
		{
			MovementSpeeds speeds = new MovementSpeeds(5, 5, 5)
			{
				SidewaysSpeed = 10
			};

			Assert.AreEqual(speeds.Max, 10);
		}
		
		[Test]
		public void TestBackwardsMax()
		{
			MovementSpeeds speeds = new MovementSpeeds(5, 5, 5)
			{
				BackwardsSpeed = 10
			};

			Assert.AreEqual(speeds.Max, 10);
		}

		[Test]
		public void TestEquals()
		{
			MovementSpeeds a = new MovementSpeeds(5, 10, 3);
			MovementSpeeds b = new MovementSpeeds(5, 10, 3);

			Assert.IsTrue(a.Equals(b));
			Assert.IsFalse(a.Equals(new object()));

			b.ForwardSpeed = 0;

			Assert.IsFalse(a.Equals(b));
		}
		
		[Test]
		public void TestEqualsOperator()
		{
			MovementSpeeds a = new MovementSpeeds(5, 10, 3);
			MovementSpeeds b = new MovementSpeeds(5, 10, 3);

			Assert.IsTrue(a == b);
			Assert.IsFalse(a != b);

			b.ForwardSpeed = 0;

			Assert.IsFalse(a == b);
		}
		
		[Test]
		public void TestHashCode()
		{
			MovementSpeeds a = new MovementSpeeds(5, 10, 3);
			MovementSpeeds b = new MovementSpeeds(5, 10, 3);

			Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

			b.ForwardSpeed = 0;

			Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
		}
	}
}
#endif