using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Common.Enums;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public class RacingRule : BasePlayRule
    {
        private ISubscriber<RespawnMessage> _respawnSubscriber;
        private IPublisher<KillCharactersMessage> _killCharactersPublisher;

        protected override void RegisterCustomMessages()
        {
            _respawnSubscriber = GlobalMessagePipe.GetSubscriber<RespawnMessage>();
            _killCharactersPublisher = GlobalMessagePipe.GetPublisher<KillCharactersMessage>();
            _respawnSubscriber.Subscribe(RespawnPlayer).AddTo(bagBuilder);
        }

        private void RespawnPlayer(RespawnMessage message)
        {
            if (playerHandler.PlayerInstanceID == message.ID)
            {
                playerHandler.RespawnPlayer();
                cameraHandler.SetFollowCameraActive(true);
            }
        }

        public override void StartGame() { }

        public override void OnEndGame(EndResult endResult)
        {
            if (endResult == EndResult.Win)
                playerHandler.SetPlayerCompleteLevel(true);

            _killCharactersPublisher.Publish(new KillCharactersMessage());
        }

        public override void OnLevelEnded(EndResult endResult)
        {
            // If in single mode
            EndGame(new EndGameMessage
            {
                ID = CurrentPlayerID,
                Result = endResult
            });
        }

        public override void OnPlayerDamage()
        {
            cameraHandler.SetFollowCameraActive(false);
        }

        public override void OnPlayerHealthUpdate()
        {
            if(PlayerHealth <= 0)
            {
                EndLevel(new LevelEndMessage
                {
                    ID = CurrentPlayerID,
                    Result = EndResult.Lose
                });
            }
        }
    }
}
