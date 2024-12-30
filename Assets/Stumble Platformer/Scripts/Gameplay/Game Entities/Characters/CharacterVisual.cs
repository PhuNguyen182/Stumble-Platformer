using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters
{
    public class CharacterVisual : MonoBehaviour
    {
        [SerializeField] public Animator CharacterAnimator;
        [SerializeField] public SkinnedMeshRenderer CharacterRenderer;

        public void UpdateSkin(CharacterSkin characterSkin)
        {
            var currentState = CharacterAnimator.GetCurrentAnimatorStateInfo(0);
            float currentTime = currentState.normalizedTime;
            int stateHash = currentState.fullPathHash;

            UpdateMesh(characterSkin.SkinMesh);
            UpdateMaterials(characterSkin.SkinMaterials);
            UpdateAvatar(characterSkin.SkinAvatar);
            CharacterAnimator.Play(stateHash, 0, currentTime);
        }

        public void SetRunning(bool isRunning)
        {
            CharacterAnimator.SetBool(CharacterAnimationKeys.IsRunningKey, isRunning);
        }

        public void SetInputMoving(bool isInputMoving)
        {
            CharacterAnimator.SetBool(CharacterAnimationKeys.IsMoveInputKey, isInputMoving);
        }

        public void SetMove(float move)
        {
            CharacterAnimator.SetFloat(CharacterAnimationKeys.MoveKey, move);
        }

        public void SetFalling(bool isFalling)
        {
            CharacterAnimator.SetBool(CharacterAnimationKeys.IsFallingKey, isFalling);
        }

        public void SetStumble(bool isStumble)
        {
            CharacterAnimator.SetBool(CharacterAnimationKeys.IsStumbledKey, isStumble);
        }

        public void SetJump(bool isJumping)
        {
            CharacterAnimator.SetBool(CharacterAnimationKeys.IsJumpingUpKey, isJumping);
        }

        public void SetLose()
        {
            CharacterAnimator.SetTrigger(CharacterAnimationKeys.LoseKey);
        }

        public void PlayWin()
        {
            CharacterAnimator.SetTrigger(CharacterAnimationKeys.WinKey);
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
