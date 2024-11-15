using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    public class MessageBroketManager
    {
        private readonly IServiceProvider _provider;
        private readonly BuiltinContainerBuilder _builder;

        public MessageBroketManager()
        {
            _builder = new();
            _builder.AddMessagePipe();

            AddMeggageBrokers();
            _provider = _builder.BuildServiceProvider();
            GlobalMessagePipe.SetProvider(_provider);
        }

        private void AddMeggageBrokers()
        {
            _builder.AddMessageBroker<RespawnMessage>();
            _builder.AddMessageBroker<InitializeLevelMessage>();
        }
    }
}
