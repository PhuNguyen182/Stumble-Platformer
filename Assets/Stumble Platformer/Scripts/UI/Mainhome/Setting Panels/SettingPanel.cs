using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StumblePlatformer.Scripts.Common.Constants;
using GlobalScripts.Audios;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Mainhome.SettingPanels
{
    public class SettingPanel : DerivedPanel
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button resetButton;

        [Header("Sliders")]
        [SerializeField] private Slider rotateXCamera;
        [SerializeField] private Slider rotateYCamera;
        [SerializeField] private Slider masterVolume;
        [SerializeField] private Slider musicVolume;
        [SerializeField] private Slider soundFXVolume;

        [Header("Texts")]
        [SerializeField] private TMP_Text rotateXValueText;
        [SerializeField] private TMP_Text rotateYValueText;
        [SerializeField] private TMP_Text masterVolumeValueText;
        [SerializeField] private TMP_Text musicVolumeValueText;
        [SerializeField] private TMP_Text soundFXVolumeValueText;

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
            resetButton.onClick.AddListener(ResetSettings);

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

            rotateXValueText.text = $"{rotateXCamera.value * 100:F0}";
            rotateYValueText.text = $"{rotateYCamera.value * 1000:F0}";
            masterVolumeValueText.text = $"{masterVolume.value * 100:F0}";
            musicVolumeValueText.text = $"{musicVolume.value * 100:F0}";
            soundFXVolumeValueText.text = $"{soundFXVolume.value * 100:F0}";
        }

        private void ResetSettings()
        {
            rotateXCamera.value = CharacterConstants.DefaultRotateXCamera;
            rotateYCamera.value = CharacterConstants.DefaultRotateYCamera;
            masterVolume.value = SettingConstants.DefaultMasterVolume;
            musicVolume.value = SettingConstants.DefaultMusicVolume;
            soundFXVolume.value = SettingConstants.DefaultSoundVolume;
        }

        private void UpdateRotateX(float value)
        {
            PlayerPrefs.SetFloat(RotateXSaveKey, value);
            rotateXValueText.text = $"{value * 100:F0}";
        }

        private void UpdateRotateY(float value)
        {
            PlayerPrefs.SetFloat(RotateYSaveKey, value);
            rotateYValueText.text = $"{value * 1000:F0}";
        }

        private void UpdateMasterVolume(float volume)
        {
            AudioManager.Instance.MasterVolume = volume;
            masterVolumeValueText.text = $"{volume * 100:F0}";
        }

        private void UpdateMusicVolume(float volume)
        {
            AudioManager.Instance.MusicVolume = volume;
            musicVolumeValueText.text = $"{volume * 100:F0}";
        }

        private void UpdateSoundFXVolume(float volume)
        {
            AudioManager.Instance.SoundVolume = volume;
            soundFXVolumeValueText.text = $"{volume * 100:F0}";
        }
    }
}
