using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Gameplay.GameManagers;
using StumblePlatformer.Scripts.Common.Enums;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class FinishZone : MonoBehaviour
    {
        private IPublisher<LevelEndMessage> _playerFinishPublisher;

        private void Start()
        {            
            if (GameplayInitializer.Instance != null && GameplayInitializer.Instance.IsAllMessagesInit())
                _playerFinishPublisher = GlobalMessagePipe.GetPublisher<LevelEndMessage>();
        }

        public void ReportFinish(PlayerController playerController)
        {
            LevelEndMessage levelEndMessage = default;
            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
            {
                levelEndMessage = new()
                {
                    ClientID = ulong.MaxValue,
                    Result = EndResult.Win
                };
            }

            else
            {
                levelEndMessage = new()
                {
                    ClientID = playerController.OwnerClientId,
                    Result = EndResult.Win
                };
            }

            _playerFinishPublisher.Publish(levelEndMessage);
        }
    }
}
