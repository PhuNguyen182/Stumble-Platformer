using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public class SurvivalRule : BasePlayRule
    {
        [SerializeField] private float playDuration = 30f;

        private bool _hasLosedGame;
        private float _currentTimer;

        public float PlayDuration => playDuration;
        public float CurrentTimer => _currentTimer;

        protected override void OnStart()
        {
            _hasLosedGame = false;
            _currentTimer = playDuration;
        }

        public override void OnUpdate(float deltatime)
        {
            if(_currentTimer > 0 && !_hasLosedGame)
            {
                _currentTimer -= Time.deltaTime;
                if(_currentTimer <= 0 && !_hasLosedGame)
                {
                    // If in siggle mode
                    EndLevel(new LevelEndMessage
                    {
                        ID = CurrentPlayerID,
                        Result = EndResult.Win
                    });
                }
            }
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
            // If in single mode
            EndGame(new EndGameMessage
            {
                ID = CurrentPlayerID,
                Result = endResult
            });
        }

        public override void OnPlayerDamage()
        {
            _hasLosedGame = true;
            EndLevel(new LevelEndMessage
            {            
                ID = CurrentPlayerID,
                Result = EndResult.Lose
            });
        }
    }
}
