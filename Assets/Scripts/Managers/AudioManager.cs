using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EstiamGameJam2025
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AudioManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("AudioManager");
                        instance = go.AddComponent<AudioManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip selectSound;
        [SerializeField] private AudioClip connectSound;
        [SerializeField] private AudioClip correctSound;
        [SerializeField] private AudioClip errorSound;
        [SerializeField] private AudioClip successSound;
        [SerializeField] private AudioClip failSound;
        [SerializeField] private AudioClip explosionSound;
        [SerializeField] private AudioClip alarmSound;
        [SerializeField] private AudioClip rewindSound;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Crée les AudioSources si elles n'existent pas
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
            }
        }

        public void PlaySound(string soundName)
        {
            AudioClip clip = GetClipByName(soundName);
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        public void PlayMusic(AudioClip musicClip)
        {
            if (musicClip != null && musicSource != null)
            {
                musicSource.clip = musicClip;
                musicSource.Play();
            }
        }

        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        public void SetSFXVolume(float volume)
        {
            if (sfxSource != null)
            {
                sfxSource.volume = Mathf.Clamp01(volume);
            }
        }

        public void SetMusicVolume(float volume)
        {
            if (musicSource != null)
            {
                musicSource.volume = Mathf.Clamp01(volume);
            }
        }

        private AudioClip GetClipByName(string name)
        {
            switch (name.ToLower())
            {
                case "click":
                    return clickSound;
                case "select":
                    return selectSound;
                case "connect":
                    return connectSound;
                case "correct":
                    return correctSound;
                case "error":
                    return errorSound;
                case "success":
                    return successSound;
                case "fail":
                    return failSound;
                case "explosion":
                    return explosionSound;
                case "alarm":
                    return alarmSound;
                case "rewind":
                    return rewindSound;
                default:
                    Debug.LogWarning($"Son non trouvé: {name}");
                    return null;
            }
        }
    }
}