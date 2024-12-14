using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Common.Messages;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public class SurvivalRule : BasePlayRule
    {
        [SerializeField] private float playDuration = 30f;

        private bool _hasFallen;
        private float _currentTimer;
        private IPublisher<LevelEndMessage> _levelEndPublisher;

        public float PlayDuration => playDuration;
        public float CurrentTimer => _currentTimer;

        private void Update()
        {
            if(_currentTimer > 0)
            {
                _currentTimer -= Time.deltaTime;

                if(_currentTimer <= 0 && !_hasFallen)
                {
                    // Win game
                    EndGame(new EndGameMessage
                    {
                        ID = CurrentPlayerID,
                        Result = EndResult.Win
                    });
                }
            }
        }

        protected override void OnStart()
        {
            _hasFallen = false;
            _currentTimer = playDuration;
        }

        protected override void RegisterCustomMessages()
        {
            _levelEndPublisher = GlobalMessagePipe.GetPublisher<LevelEndMessage>();
        }

        public override void OnEndGame(EndResult endResult)
        {
            string endColor = endResult switch
            {
                EndResult.Win => "#00ff00",
                EndResult.Lose => "#ff0000",
                _ => ""
            };

            string result = endResult switch
            {
                EndResult.Win => "You are survived!",
                EndResult.Lose => "You are dead!",
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
            // If fall, lose immediately
            _hasFallen = true;
            _levelEndPublisher.Publish(new LevelEndMessage
            {
                ID = CurrentPlayerID,
                Result = EndResult.Lose
            });
        }

        public override void OnPlayerHealthUpdate()
        {
            
        }
    }
}
