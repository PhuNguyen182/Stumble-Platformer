using System;
using StumblePlatformer.Scripts.Common.Messages;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class MessageBroketManager
    {
        private readonly IServiceProvider _provider;
        private readonly BuiltinContainerBuilder _builder;

        public bool IsInitialize { get; private set; }

        public MessageBroketManager()
        {
            _builder = new();
            _builder.AddMessagePipe();

            AddMeggageBrokers();
            _provider = _builder.BuildServiceProvider();
            GlobalMessagePipe.SetProvider(_provider);
            IsInitialize = true;
        }

        private void AddMeggageBrokers()
        {
            _builder.AddMessageBroker<RespawnMessage>();
            _builder.AddMessageBroker<ReportPlayerHealthMessage>();
            _builder.AddMessageBroker<KillCharactersMessage>();
            _builder.AddMessageBroker<PlayerDamageMessage>();
            _builder.AddMessageBroker<EndGameMessage>();
            _builder.AddMessageBroker<LevelEndMessage>();
        }

        public void Forget()
        {

        }
    }
}
