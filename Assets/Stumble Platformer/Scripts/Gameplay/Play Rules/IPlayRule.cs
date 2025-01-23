using StumblePlatformer.Scripts.Common.Enums;
using StumblePlatformer.Scripts.Gameplay.GameManagers;
using StumblePlatformer.Scripts.Common.Messages;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public interface IPlayRule
    {
        public string ObjectiveTitle { get; }
        public bool IsEndGame { get; set; }

        public void StartGame();
        public void EndGame(EndGameMessage message);
        public void EndLevel(LevelEndMessage message);
        public void DamagePlayer(PlayerDamageMessage message);
        public void OnEndGame(EndResult endResult);
        public void OnLevelEnded(EndResult endResult);
        public void OnPlayerDamage();
        public void OnPlayerHealthUpdate();
        public void SetStateController(GameStateController gameStateController);
    }
}
