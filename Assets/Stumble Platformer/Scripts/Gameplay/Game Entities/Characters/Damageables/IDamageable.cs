namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Damageables
{
    public interface IDamageable
    {
        public void TakeHealthDamage(HealthDamage damageData);
        public void TakePhysicalAttack(PhysicalDamage damageData);
    }
}
