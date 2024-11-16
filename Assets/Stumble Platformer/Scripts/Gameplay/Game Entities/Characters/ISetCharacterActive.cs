namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public interface ISetCharacterActive
    {
        public bool IsActive { get; set; }
        public void SetCharacterActive(bool active);
    }
}
