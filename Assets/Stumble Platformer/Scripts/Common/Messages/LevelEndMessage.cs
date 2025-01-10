using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Common.Messages
{
    public struct LevelEndMessage
    {
        public int ID;
        public EndResult Result;
        public ulong ClientID;
    }
}
