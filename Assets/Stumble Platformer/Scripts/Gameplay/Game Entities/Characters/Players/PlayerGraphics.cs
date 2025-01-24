using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StumblePlatformer.Scripts.Gameplay.GameManagers;
using StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals;
using StumblePlatformer.Scripts.Gameplay.Databases;
using TMPro;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Characters.Players
{
    public class PlayerGraphics : NetworkBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private Canvas playerNameCanvas;
        [SerializeField] private Transform characterPivot;
        [SerializeField] private CharacterVisual characterVisual;
        [SerializeField] private ParticleSystem dustStep;
        [SerializeField] private PlayerEffectDatabase effectCollection;

        private Transform _cameraTransform;
        private Quaternion _originalOrientation;

        public CharacterVisual CharacterVisual => characterVisual;

        private void Start()
        {
            _originalOrientation = playerNameCanvas.transform.rotation;
            _cameraTransform = PlayGroundManager.Instance.CameraHandler.MainCamera.transform;
            playerNameCanvas.worldCamera = PlayGroundManager.Instance.CameraHandler.MainCamera;
        }

        public void UpdateCanvas()
        {
            if (_cameraTransform != null)
                playerNameCanvas.transform.rotation = _cameraTransform.rotation * _originalOrientation;
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        public void SetPlayerNameRpc(string playerName)
        {
            playerNameText.text = playerName;
        }

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
            SetDustEffectActiveRpc(isActive);
        }

        public void PlayDeadEffect()
        {
            Vector3 position = transform.position + Vector3.up * 0.5f;
            SimplePool.Spawn(effectCollection.LaserDeadEffect, EffectContainer.Transform
                             , position, Quaternion.identity);
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        private void SetDustEffectActiveRpc(bool isActive)
        {
            var emission = dustStep.emission;
            emission.enabled = isActive;
        }
    }
}
