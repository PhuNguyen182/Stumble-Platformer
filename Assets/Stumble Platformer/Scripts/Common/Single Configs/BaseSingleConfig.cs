namespace StumblePlatformer.Scripts.Common.SingleConfigs
{
    public abstract class BaseSingleConfig<TConfig>
    {
        public static TConfig Current;

        public static void Dispose()
        {
            Current = default;
        }
    }
}
