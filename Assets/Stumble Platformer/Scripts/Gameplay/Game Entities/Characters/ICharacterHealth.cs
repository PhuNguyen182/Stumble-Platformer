namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public interface ICharacterHealth
    {
        public int HealthPoint { get; }
        public void TakeDamage(int damage);
        public void SetHealth(int health);
    }
}
