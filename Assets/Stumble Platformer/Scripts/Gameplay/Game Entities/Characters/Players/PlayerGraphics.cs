using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;
using StumblePlatformer.Scripts.Gameplay.Databases;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerGraphics : MonoBehaviour
    {
        [SerializeField] private Transform characterPivot;
        [SerializeField] private CharacterVisual characterVisual;
        [SerializeField] private ParticleSystem dustStep;
        [SerializeField] private PlayerEffectDatabase effectCollection;

        public CharacterVisual CharacterVisual => characterVisual;

        public void SetCharacterVisual(CharacterSkin characterSkin)
        {
            characterVisual.UpdateSkin(characterSkin);
        }

        public void SetPlayerGraphicActive(bool active)
        {
            characterVisual.gameObject.SetActive(active);
        }

        public void SetDustEffectActive(bool isActive)
        {
            var emission = dustStep.emission;
            emission.enabled = isActive;
        }

        public void PlayDeadEffect()
        {
            Vector3 position = transform.position + Vector3.up * 0.5f;
            SimplePool.Spawn(effectCollection.LaserDeadEffect, EffectContainer.Transform
                             , position, Quaternion.identity);
        }
    }
}
