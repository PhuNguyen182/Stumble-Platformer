using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using StumblePlatformer.Scripts.Common.Constants;
using StumblePlatformer.Scripts.Gameplay.Databases;
using StumblePlatformer.Scripts.Common.Enums;

namespace GlobalScripts.Audios
{
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [SerializeField] private AudioMixer audioMixer;
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource itemSource;
        
        [Header("Audio Collections")]
        [SerializeField] private SoundEffectCollection effectDatabase;
        [SerializeField] private MusicCollection musicDatabase;

        public static event Action<float> OnMasterChange;
        public static event Action<float> OnMusicChange;
        public static event Action<float> OnSoundChange;

        private const string MasterVolumeMixer = "MasterVolume";
        private const string MusicVolumeMixer = "MusicVolume";
        private const string SoundVolumeMixer = "SoundVolume";

        private const string MasterVolumeKey = "MasterVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string SoundVolumeKey = "SoundVolume";

        public float MasterVolume
        {
            get => PlayerPrefs.GetFloat(MasterVolumeKey, SettingConstants.DefaultMasterVolume);
            set
            {
                PlayerPrefs.SetFloat(MasterVolumeKey, value);
                OnMasterChange.Invoke(value);
            }
        }

        public float MusicVolume
        {
            get => PlayerPrefs.GetFloat(MusicVolumeKey, SettingConstants.DefaultMusicVolume);
            set
            {
                PlayerPrefs.SetFloat(MusicVolumeKey, value);
                OnMusicChange.Invoke(value);
            }
        }

        public float SoundVolume
        {
            get => PlayerPrefs.GetFloat(SoundVolumeKey, SettingConstants.DefaultSoundVolume);
            set
            {
                PlayerPrefs.SetFloat(SoundVolumeKey, value);
                OnSoundChange.Invoke(value);
            }
        }

        protected override void OnAwake()
        {
            effectDatabase.Initialize();
            musicDatabase.Initialize();
        }

        private void Start()
        {
            OnMasterChange += AdjustMasterVolume;
            OnMusicChange += AdjustMusicVolume;
            OnSoundChange += AdjustSoundVolume;

            MasterVolume += 0;
            MusicVolume += 0;
            SoundVolume += 0;
        }

        public bool IsMusicPlaying()
        {
            return musicSource.isPlaying;
        }

        public void PlayMusic(AudioClip musicClip, bool loop = false, float volume = 1f)
        {
            if (musicClip == null)
                return;

            musicSource.Stop();
            musicSource.loop = loop;
            musicSource.clip = musicClip;
            musicSource.volume = volume;
            musicSource.Play();
        }

        public void PlayItemSound(SoundEffectType soundEffect, float volumeScale = 1, bool loop = false)
        {
            AudioClip sound = effectDatabase.SoundFXCollection[soundEffect];

            if (sound != null)
                PlayItemSound(sound, volumeScale, loop);
        }

        public void PlaySoundEffect(SoundEffectType soundEffect, float volumeScale = 1, bool loop = false)
        {
            AudioClip sound = effectDatabase.GetSoundEffect(soundEffect);

            if (sound != null)
                PlaySoundEffect(sound, volumeScale, loop);
        }

        public void PlayItemSound(AudioClip soundClip, float volumeScale = 1, bool loop = false)
        {
            if (soundClip == null || sfxSource == null)
                return;

            itemSource.loop = loop;
            itemSource.PlayOneShot(soundClip, volumeScale);
        }

        public void PlaySoundEffect(AudioClip soundClip, float volumeScale = 1, bool loop = false)
        {
            if (soundClip == null || sfxSource == null)
                return;

            sfxSource.loop = loop;
            sfxSource.PlayOneShot(soundClip, volumeScale);
        }

        public void PlayBackgroundMusic(BackgroundMusicType backgroundMusicType, bool loop = true, float volume = 1)
        {
            AudioClip bgm = musicDatabase.GetBackgroundMusic(backgroundMusicType);

            if (bgm != null)
                PlayMusic(bgm, loop, volume);
        }

        public void PlayBackgroundMusic(AudioClip backgroundMusic, bool loop = true, float volume = 1)
        {
            PlayMusic(backgroundMusic, loop, volume);
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        private void AdjustMasterVolume(float value)
        {
            float volume = Mathf.Log10(value) * 20;
            audioMixer.SetFloat(MasterVolumeMixer, volume);
        }

        private void AdjustMusicVolume(float value)
        {
            float volume = Mathf.Log10(value) * 20;
            audioMixer.SetFloat(MusicVolumeMixer, volume);
        }

        private void AdjustSoundVolume(float value)
        {
            float volume = Mathf.Log10(value) * 20;
            audioMixer.SetFloat(SoundVolumeMixer, volume);
        }

        private void OnDestroy()
        {
            OnMasterChange -= AdjustMasterVolume;
            OnMusicChange -= AdjustMusicVolume;
            OnSoundChange -= AdjustSoundVolume;
        }
    }
}
