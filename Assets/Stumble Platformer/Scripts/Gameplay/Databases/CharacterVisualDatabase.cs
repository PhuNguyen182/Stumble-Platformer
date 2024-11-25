using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;

namespace StumblePlatformer.Scripts.Gameplay.Databases
{
    [CreateAssetMenu(fileName = "Character Visual Database", menuName = "Scriptable Objects/Databases/Character Visual Database")]
    public class CharacterVisualDatabase : ScriptableObject
    {
        [SerializeField] private List<SkinKeyValuePair> CharacterSkins;

        public Dictionary<string, CharacterSkin> SkinCollections { get; private set; }

        public void Initialize()
        {
            SkinCollections = CharacterSkins.ToDictionary(key => key.SkinID, value => value.CharacterSkin);
        }

        public bool TryGetCharacterSkin(string skinId, out CharacterSkin characterSkin)
        {
            if(SkinCollections == null)
            {
                characterSkin = null;
                return false;
            }

            return SkinCollections.TryGetValue(skinId, out characterSkin);
        }
    }
}
