using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archer
{
    public enum MusicFileName
    {
        BGM_01,
        BGM_02,
    }

    public enum SFXFileName
    {
        GunEmpty,
        GunFire,
        GunReload,
    }

    public class SoundManager : SingletonBase<SoundManager>
    {
        public Dictionary<MusicFileName, AudioClip> musicAssetContainer = new Dictionary<MusicFileName, AudioClip>();
        public Dictionary<SFXFileName, AudioClip> sfxAssetContainer = new Dictionary<SFXFileName, AudioClip>();

        private bool isMusicOrderForA = true;

        private AudioSource musicAudioSource_A;
        private AudioSource musicAudioSource_B;

        private AudioSource sfxAudioSource;

        private bool isInitialized = false;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            GameObject musicSourceA = new GameObject("Music Audio Source A");
            musicAudioSource_A = musicSourceA.AddComponent<AudioSource>();
            musicAudioSource_A.loop = true;
            musicAudioSource_A.playOnAwake = false;
            musicAudioSource_A.transform.SetParent(transform);

            GameObject musicSourceB = new GameObject("Music Audio Source B");
            musicAudioSource_B = musicSourceB.AddComponent<AudioSource>();
            musicAudioSource_B.loop = true;
            musicAudioSource_B.playOnAwake = false;
            musicAudioSource_B.transform.SetParent(transform);

            GameObject sfxSource = new GameObject("SFX Audio Source");
            sfxAudioSource = sfxSource.AddComponent<AudioSource>();
            sfxAudioSource.loop = false;
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.spatialBlend = 1f; // 3D 사운드 효과
            sfxAudioSource.transform.SetParent(transform);

            LoadMusicAssets();
            LoadSFXAssets();
        }

        private void LoadMusicAssets()
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Sound/Music/");
            foreach (var clip in clips)
            {
                MusicFileName fileName = (MusicFileName)System.Enum.Parse(typeof(MusicFileName), clip.name);
                musicAssetContainer.Add(fileName, clip);
            }
        }

        private void LoadSFXAssets()
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Sound/SFX/");
            foreach (var clip in clips)
            {
                SFXFileName fileName = (SFXFileName)System.Enum.Parse(typeof(SFXFileName), clip.name);
                sfxAssetContainer.Add(fileName, clip);
            }
        }

        private void Update()
        {
            if (isMusicOrderForA)
            {
                if (musicAudioSource_A.volume < 0.3f)
                {
                    musicAudioSource_A.volume += Time.deltaTime * 0.5f;
                    musicAudioSource_B.volume = 0.3f - musicAudioSource_A.volume;
                }
            }
            else
            {
                if (musicAudioSource_A.volume > 0f)
                {
                    musicAudioSource_A.volume -= Time.deltaTime * 0.5f;
                    musicAudioSource_B.volume = 0.3f - musicAudioSource_A.volume;
                }
            }

            if (musicAudioSource_A.volume <= 0)
            {
                musicAudioSource_A.Stop();
                musicAudioSource_A.volume = 0;
            }

            if (musicAudioSource_B.volume <= 0)
            {
                musicAudioSource_B.Stop();
                musicAudioSource_B.volume = 0;
            }

        }

        public void PlayMusic(MusicFileName musicName)
        {
            if (!musicAssetContainer.ContainsKey(musicName)) return;

            AudioClip targetMusicClip = musicAssetContainer[musicName];

            isMusicOrderForA = !isMusicOrderForA;
            if (isMusicOrderForA)
            {
                musicAudioSource_A.clip = targetMusicClip;
                musicAudioSource_A.volume = 0;
                musicAudioSource_A.Play();
            }
            else
            {
                musicAudioSource_B.clip = targetMusicClip;
                musicAudioSource_B.volume = 0;
                musicAudioSource_B.Play();
            }
        }

        public void StopMusic()
        {
            isMusicOrderForA = false;
            musicAudioSource_A.Stop();
            musicAudioSource_B.Stop();
            musicAudioSource_A.volume = 0;
            musicAudioSource_B.volume = 0;
        }

        public void PlaySFX(SFXFileName sfxName, Vector3 position) // 3D로 재생해야하므로 위치정보가 추가로 필요함
        {
            if (!sfxAssetContainer.ContainsKey(sfxName)) return;

            AudioSource newSFXSource = Instantiate(sfxAudioSource, transform);
            newSFXSource.gameObject.SetActive(true);
            newSFXSource.clip = sfxAssetContainer[sfxName];
            newSFXSource.transform.position = position;
            newSFXSource.Play();
            
            Destroy(newSFXSource.gameObject, newSFXSource.clip.length);
        }
    }
}
