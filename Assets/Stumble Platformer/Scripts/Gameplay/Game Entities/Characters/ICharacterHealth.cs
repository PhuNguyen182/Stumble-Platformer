using StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public interface ICharacterHealth
    {
        public int HealthPoint { get; }
        public void TakeDamage(HealthDamage damage);
        public void SetHealth(int health);
    }
}
