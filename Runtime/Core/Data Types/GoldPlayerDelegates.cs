namespace Hertzole.GoldPlayer
{
    public static class GoldPlayerDelegates
    {
        public delegate void PlayerEvent();
        public delegate void JumpEvent(float height);
        public delegate void LandEvent(float fallHeight);
    }
}
