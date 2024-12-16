using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.GameManagers;
using StumblePlatformer.Scripts.Common.Enums;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public class RacingRule : BasePlayRule, ISetPlayerHandler
    {
        private PlayerHandler _playerHandler;

        private IPublisher<LevelEndMessage> _levelEndPublisher;
        private IPublisher<KillCharactersMessage> _killCharactersPublisher;
        private ISubscriber<RespawnMessage> _respawnSubscriber;

        protected override void RegisterCustomMessages()
        {
            _levelEndPublisher = GlobalMessagePipe.GetPublisher<LevelEndMessage>();
            _killCharactersPublisher = GlobalMessagePipe.GetPublisher<KillCharactersMessage>();
            _respawnSubscriber = GlobalMessagePipe.GetSubscriber<RespawnMessage>();
            _respawnSubscriber.Subscribe(RespawnPlayer).AddTo(bagBuilder);
        }

        private void RespawnPlayer(RespawnMessage message)
        {
            if (_playerHandler.PlayerInstanceID == message.ID)
                _playerHandler.RespawnPlayer();
        }

        public override void OnUpdate(float deltaTime) { }

        public void SetPlayerHandler(PlayerHandler playerHandler)
        {
            _playerHandler = playerHandler;
        }

        public override void OnEndGame(EndResult endResult)
        {
            _killCharactersPublisher.Publish(new KillCharactersMessage { });

#if UNITY_EDITOR
            string endColor = endResult switch
            {
                EndResult.Win => "#00ff00",
                EndResult.Lose => "#ff0000",
                _ => ""
            };

            string result = endResult switch
            {
                EndResult.Win => "Win Race",
                EndResult.Lose => "Lose Race",
                _ => ""
            };

            Debug.Log($"<color={endColor}>{result}</color>");
#endif
        }

        public override void OnLevelEnded(EndResult endResult)
        {
#if UNITY_EDITOR
            Debug.Log($"Player End Racing: {endResult}");
#endif
            EndGame(new EndGameMessage
            {
                ID = CurrentPlayerID,
                Result = endResult
            });
        }

        public override void OnPlayerFall()
        {
#if UNITY_EDITOR
            Debug.Log("Player Fall In Racing");
#endif
        }

        public override void OnPlayerHealthUpdate()
        {
            if(PlayerHealth <= 0)
            {
                _levelEndPublisher.Publish(new LevelEndMessage
                {
                    ID = CurrentPlayerID,
                    Result = EndResult.Lose
                });
            }
        }
    }
}
