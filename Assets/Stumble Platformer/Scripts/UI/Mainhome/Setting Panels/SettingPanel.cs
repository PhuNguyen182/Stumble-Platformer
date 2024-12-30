using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.Common.Constants;
using StumblePlatformer.Scripts.UI.Mainhome.MainPanels;
using GlobalScripts.Audios;

namespace StumblePlatformer.Scripts.UI.Mainhome.SettingPanels
{
    public class SettingPanel : DerivedPanel
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Slider rotateXCamera;
        [SerializeField] private Slider rotateYCamera;
        [SerializeField] private Slider masterVolume;
        [SerializeField] private Slider musicVolume;
        [SerializeField] private Slider soundFXVolume;

        private const string RotateXSaveKey = "RotateX";
        private const string RotateYSaveKey = "RotateY";

        private void Awake()
        {
            RegisterSliders();
        }

        protected override void Start()
        {
            base.Start();
            GetSettingValues();
        }

        private void RegisterSliders()
        {
            backButton.onClick.AddListener(BackToMain);
            rotateXCamera.onValueChanged.AddListener(UpdateRotateX);
            rotateYCamera.onValueChanged.AddListener(UpdateRotateY);
            masterVolume.onValueChanged.AddListener(UpdateMasterVolume);
            musicVolume.onValueChanged.AddListener(UpdateMusicVolume);
            soundFXVolume.onValueChanged.AddListener(UpdateSoundFXVolume);
        }

        private void GetSettingValues()
        {
            rotateXCamera.value = PlayerPrefs.GetFloat(RotateXSaveKey, CharacterConstants.DefaultRotateXCamera);
            rotateYCamera.value = PlayerPrefs.GetFloat(RotateYSaveKey, CharacterConstants.DefaultRotateYCamera);

            masterVolume.value = AudioManager.Instance.MasterVolume;
            musicVolume.value = AudioManager.Instance.MusicVolume;
            soundFXVolume.value = AudioManager.Instance.SoundVolume;
        }

        private void UpdateRotateX(float value)
        {
            PlayerPrefs.SetFloat(RotateXSaveKey, value);
        }

        private void UpdateRotateY(float value)
        {
            PlayerPrefs.SetFloat(RotateYSaveKey, value);
        }

        private void UpdateMasterVolume(float volume)
        {
            AudioManager.Instance.MasterVolume = volume;
        }

        private void UpdateMusicVolume(float volume)
        {
            AudioManager.Instance.MusicVolume = volume;
        }

        private void UpdateSoundFXVolume(float volume)
        {
            AudioManager.Instance.SoundVolume = volume;
        }
    }
}
