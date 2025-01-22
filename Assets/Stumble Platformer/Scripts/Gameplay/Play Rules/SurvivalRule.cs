using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.UI.Gameplay.MainPanels;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public class SurvivalRule : BasePlayRule
    {
        [SerializeField] private float playDuration = 30f;

        private bool _hasLosedGame;
        private float _currentTimer;
        private PlayRuleTimer _playRuleTimer;

        public float PlayDuration => playDuration;

        protected override void OnStart()
        {
            base.OnStart();
            _hasLosedGame = false;
            _currentTimer = playDuration;
        }

        public override void StartGame()
        {
            bool active = GameplaySetup.PlayMode == GameMode.SinglePlayer;
            _playRuleTimer.gameObject.SetActive(active);
        }

        public void SetPlayRuleTimer(PlayRuleTimer playRuleTimer)
        {
            _playRuleTimer = playRuleTimer;
        }

        public override void OnUpdate(float deltatime)
        {
            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
            {
                if (_currentTimer > 0 && !_hasLosedGame)
                {
                    _currentTimer -= Time.deltaTime;
                    _playRuleTimer.UpdateTime(_currentTimer);

                    if (_currentTimer <= 0 && !_hasLosedGame)
                    {
                        _currentTimer = 0;
                        _playRuleTimer.UpdateTime(_currentTimer);

                        EndLevel(new LevelEndMessage
                        {
                            ID = CurrentPlayerID,
                            Result = EndResult.Win
                        });
                    }
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
