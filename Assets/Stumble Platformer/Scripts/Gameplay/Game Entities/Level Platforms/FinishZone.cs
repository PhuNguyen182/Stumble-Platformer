using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players;
using StumblePlatformer.Scripts.Common.Enums;
using MessagePipe;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class FinishZone : MonoBehaviour
    {
        private IPublisher<LevelEndMessage> _playerFinishPublisher;

        private void Start()
        {
            _playerFinishPublisher = GlobalMessagePipe.GetPublisher<LevelEndMessage>();
        }

        public void ReportFinish(PlayerController playerController)
        {
            if (GameplaySetup.PlayMode == GameMode.SinglePlayer)
            {
                _playerFinishPublisher.Publish(new LevelEndMessage
                {
                    ID = playerController.gameObject.GetInstanceID(),
                    Result = EndResult.Win
                });
            }
            else if(GameplaySetup.PlayMode == GameMode.Multiplayer)
            {
                ulong clientId = NetworkManager.Singleton.LocalClient.ClientId;
            }
        }
    }
}
