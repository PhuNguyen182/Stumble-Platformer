namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public interface IPlayeRule
    {
        public void Win();
        public void Lose();
        public void OnPlayerFinish();
        public void OnPlayerFall();
    }
}
