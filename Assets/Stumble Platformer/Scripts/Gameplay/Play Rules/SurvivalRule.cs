using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Common.Enums;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public class SurvivalRule : BasePlayRule
    {
        [SerializeField] private float playDuration = 30f;

        private bool _hasLosedGame;
        private float _currentTimer;

        private IPublisher<LevelEndMessage> _levelEndPublisher;

        public float PlayDuration => playDuration;
        public float CurrentTimer => _currentTimer;

        public override void OnUpdate(float deltatime)
        {
            if(_currentTimer > 0 && !_hasLosedGame)
            {
                _currentTimer -= Time.deltaTime;
                if(_currentTimer <= 0 && !_hasLosedGame)
                {
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
            _hasLosedGame = false;
            _currentTimer = playDuration;
        }

        protected override void RegisterCustomMessages()
        {
            _levelEndPublisher = GlobalMessagePipe.GetPublisher<LevelEndMessage>();
        }

        public override void OnEndGame(EndResult endResult)
        {
            if (endResult == EndResult.Win)
                playerHandler.SetPlayerCompleteLevel(true);

            playerHandler.SetPlayerActive(false);
            playerHandler.SetPlayerPhysicsActive(false);
            cameraHandler.SetFollowCameraActive(false);
        }

        public override void OnLevelEnded(EndResult endResult)
        {
            EndGame(new EndGameMessage
            {
                ID = CurrentPlayerID,
                Result = endResult
            });
        }

        public override void OnPlayerDamage()
        {
            _hasLosedGame = true;
            _levelEndPublisher.Publish(new LevelEndMessage
            {
                ID = CurrentPlayerID,
                Result = EndResult.Lose
            });
        }

        public override void OnPlayerHealthUpdate() { }
    }
}
