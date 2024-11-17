using StumblePlatformer.Scripts.Common.Messages;
using StumblePlatformer.Scripts.Gameplay.GameManagers;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public interface IPlayRule
    {
        public int CurrentPlayerID { get; set; }

        public void Win();
        public void Lose(PlayerLoseMessage message);
        public void Finish(PlayerFinishMessage message);
        public void Fall(PlayerFallMessage message);
        public void OnPlayerWin();
        public void OnPlayerLose();
        public void OnPlayerFall();
        public void OnPlayerFinish();
        public void SetStateController(GameStateController gameStateController);
    }
}
