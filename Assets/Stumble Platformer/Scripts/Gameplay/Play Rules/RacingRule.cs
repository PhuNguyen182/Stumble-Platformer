using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Common.Messages;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public class RacingRule : BasePlayRule
    {
        private IPublisher<LevelEndMessage> _levelEndPublisher;
        private IPublisher<KillCharactersMessage> _killCharactersPublisher;

        protected override void RegisterCustomMessages()
        {
            _levelEndPublisher = GlobalMessagePipe.GetPublisher<LevelEndMessage>();
            _killCharactersPublisher = GlobalMessagePipe.GetPublisher<KillCharactersMessage>();
        }

        public override void OnEndGame(EndResult endResult)
        {
            _killCharactersPublisher.Publish(new KillCharactersMessage { });

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
        }

        public override void OnLevelEnded(EndResult endResult)
        {
            Debug.Log($"Player End Racing: {endResult}");
            EndGame(new EndGameMessage
            {
                ID = CurrentPlayerID,
                Result = endResult
            });
        }

        public override void OnPlayerFall()
        {
            Debug.Log("Player Fall In Racing");
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
