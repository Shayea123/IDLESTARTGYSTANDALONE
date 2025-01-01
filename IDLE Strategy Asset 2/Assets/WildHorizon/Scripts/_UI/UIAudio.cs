using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIAudio : MonoBehaviour
    {
        [Header("Components")]
        public AudioMixer mixer;

        public bool useRandomMusic;
        public float musicDown = 0.05f;
        private float currentVolume;

        public List<AudioClip> musicList = new List<AudioClip>();
        public AudioSource musicSourse;
        public AudioSource soundSourse;

        [Header("Audio Clips")]
        public AudioClip soundButtonClick;
        public AudioClip soundBuy;
        public AudioClip soundSell;

        public Slider sliderMasterVolume;
        public Slider sliderMusicVolume;
        public Slider sliderSoundsVolume;

        private void Start()
        {
            if (useRandomMusic) musicSourse.clip = musicList[Random.Range(0, musicList.Count)];
            musicSourse.Play();

            sliderMasterVolume.value = GetMasterVolume();
            sliderMusicVolume.value = GetMusicVolume();
            sliderSoundsVolume.value = GetSoundsVolume();

            currentVolume = sliderMasterVolume.value;
            mixer.SetFloat("MasterVolume", -40);

            sliderMasterVolume.onValueChanged.AddListener(delegate
            {
                SaveMasterVolume(sliderMasterVolume.value);
            });
            sliderMusicVolume.onValueChanged.AddListener(delegate
            {
                SaveMusicVolume(sliderMusicVolume.value);
            });
            sliderSoundsVolume.onValueChanged.AddListener(delegate
            {
                SaveSoundsVolume(sliderSoundsVolume.value);
            });
        }

        private void Update()
        {
            float mixerValue = GetMasterVolume();
            if (currentVolume > GetMasterVolume()) mixer.SetFloat("MasterVolume", mixerValue += musicDown);
        }

        public void PlaySoundButtonClick()
        {
            soundSourse.PlayOneShot(soundButtonClick);
        }

        public void PlaySound(AudioClip clip)
        {
            soundSourse.PlayOneShot(clip);
        }

        private float GetMasterVolume()
        {
            float value;
            bool result = mixer.GetFloat("MasterVolume", out value);
            if (result) return value;
            else return 0f;
        }
        private float GetMusicVolume()
        {
            float value;
            bool result = mixer.GetFloat("MusicVolume", out value);
            if (result) return value;
            else return 0f;
        }
        private float GetSoundsVolume()
        {
            float value;
            bool result = mixer.GetFloat("SFXVolume", out value);
            if (result) return value;
            else return 0f;
        }

        private void SaveMasterVolume(float value)
        {
            mixer.SetFloat("MasterVolume", value);
            PlayerPrefs.SetFloat("VolumeMaster", value); //save new key to PlayerPrefs
        }
        private void SaveMusicVolume(float value)
        {
            mixer.SetFloat("MusicVolume", value);
            PlayerPrefs.SetFloat("VolumeMusic", value); //save new key to PlayerPrefs
        }
        private void SaveSoundsVolume(float value)
        {
            mixer.SetFloat("SFXVolume", value);
            PlayerPrefs.SetFloat("VolumeSounds", value); //save new key to PlayerPrefs
        }
    }
}