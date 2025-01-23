namespace StumblePlatformer.Scripts.Multiplayers
{
    public struct MultiplayerConstants
    {
        public const string DefaultIP = "127.0.0.1";
        public const ushort DefaultPort = 9998;

#if UNITY_EDITOR
        public const int MinPlayerCount = 1;
#else
        public const int MinPlayerCount = 2;
#endif
        public const int MaxPlayerCount = 7;
        public const int AcceptablePlayerCount = 2;

        public const float MaxHeartBeatTime = 15f;
        public const float ListLobbiesTimerMax = 3f;
        public const string KeyRelayJoinCode = "RelayJoinCode";
    }
}
