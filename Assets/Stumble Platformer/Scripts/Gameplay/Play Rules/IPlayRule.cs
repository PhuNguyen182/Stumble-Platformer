using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.GameManagers;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public interface IPlayRule
    {
        public int CurrentPlayerID { get; set; }

        public void EndGame(EndGameMessage message);
        public void EndLevel(LevelEndMessage message);
        public void Fall(PlayerFallMessage message);
        public void OnEndGame(EndResult endResult);
        public void OnLevelEnded(EndResult endResult);
        public void OnPlayerFall();
        public void OnPlayerHealthUpdate();
        public void SetStateController(GameStateController gameStateController);
    }
}
