using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerGraphics : MonoBehaviour
    {
        [SerializeField] private Transform characterPivot;
        [SerializeField] private CharacterVisual characterVisual;

        public CharacterVisual CharacterVisual => characterVisual;

        public void SetCharacterVisual(CharacterVisual characterVisual)
        {
            if (characterVisual == null)
                return;

            this.characterVisual ??= Instantiate(characterVisual, characterPivot.position, characterPivot.rotation, characterPivot);
        }
    }
}
