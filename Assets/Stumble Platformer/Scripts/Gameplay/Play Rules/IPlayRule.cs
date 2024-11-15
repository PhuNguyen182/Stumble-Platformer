using StumblePlatformer.Scripts.Gameplay.GameHandlers;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public interface IPlayRule
    {
        public void Win();
        public void Lose();
        public void Finish();
        public void OnPlayerWin();
        public void OnPlayerLose();
        public void OnPlayerFinish();
        public void SetStateController(GameStateController gameStateController);
    }
}
