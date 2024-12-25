using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerGraphics : MonoBehaviour
    {
        [SerializeField] private Transform characterPivot;
        [SerializeField] private CharacterVisual characterVisual;

        public CharacterVisual CharacterVisual => characterVisual;

        public void SetCharacterVisual(CharacterSkin characterSkin)
        {
            characterVisual.UpdateSkin(characterSkin);
        }

        public void SetPlayerGraphicActive(bool active)
        {
            characterVisual.gameObject.SetActive(active);
        }

        public void PlayDeadEffect()
        {

        }
    }
}
