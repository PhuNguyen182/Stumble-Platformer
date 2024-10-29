using System;

namespace StumblePlatformer.Scripts.Gameplay.Common.SingleConfigs
{
    public abstract class BaseSingleConfig<TConfig> : IDisposable
    {
        public static TConfig Current;

        public abstract void Dispose();
    }
}
