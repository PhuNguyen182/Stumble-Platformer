using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public class CharacterVisual : MonoBehaviour
    {
        public Animator CharacterAnimator;
        public SkinnedMeshRenderer CharacterRenderer;

        public void UpdateSkin(CharacterSkin characterSkin)
        {
            UpdateMesh(characterSkin.SkinMesh);
            UpdateMaterials(characterSkin.SkinMaterials);
            UpdateAvatar(characterSkin.SkinAvatar);
        }

        private void UpdateAvatar(Avatar avatar)
        {
            CharacterAnimator.avatar = avatar;
        }

        private void UpdateMesh(Mesh mesh)
        {
            CharacterRenderer.sharedMesh = mesh;
        }

        private void UpdateMaterials(Material[] materials)
        {
            CharacterRenderer.sharedMaterials = materials;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            CharacterAnimator ??= GetComponent<Animator>();
        }
#endif
    }
}
