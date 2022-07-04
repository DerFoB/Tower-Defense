using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Sounds;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioSource MusicSource;

    [SerializeField]
    private AudioSource NoiseSource;

    [SerializeField]
    private AudioSource SfxSource;

    [SerializeField]
    private Slider MusicVolumeSlider;

    [SerializeField]
    private Slider SfxVolumeSlider;

    [SerializeField]
    private Sprite MutedSprite;

    [SerializeField]
    private Sprite UnmutedSprite;

    private readonly Dictionary<Sound, AudioClip> AudioClips = new Dictionary<Sound, AudioClip>();

    private IEnumerator RandomBackgroundNoiseCoroutine = null;

    private void Start()
    {
        var clips = Resources.LoadAll<AudioClip>("Audio");
        foreach (Sound sound in Enum.GetValues(typeof(Sound)))
        {
            string soundName = Enum.GetName(typeof(Sound), sound);
            var audioClip = clips.FirstOrDefault(x => x.name == soundName);
            if (audioClip == null)
            {
                Debug.LogError(string.Format("Class {0}, Method {1}: Sound with name \"{2}\" was not found!", nameof(SoundManager), nameof(Start), soundName));
            }
            else
            {
                this.AudioClips.Add(sound, audioClip);
            }
        }

        this.MusicVolumeSlider.onValueChanged.AddListener(OnUpdateMusicVolume);
        this.SfxVolumeSlider.onValueChanged.AddListener(OnUpdateSfxVolume);
    }

    public void PlayBackgroundMusic(Sound sound, bool stopCurrent = false)
    {
        if (stopCurrent)
        {
            StopCoroutine(this.RandomBackgroundNoiseCoroutine);
            StopBackgroundMusic();
        }

        var audioClip = AudioClips[sound];
        if (audioClip != null)
        {
            this.MusicSource.PlayOneShot(audioClip);
        }
    }

    public void StopBackgroundMusic(bool alsoStopNoise = true)
    {
        this.MusicSource.Stop();
        if (alsoStopNoise)
        {
            StopBackgroundNoise();
        }
    }

    public void StopBackgroundNoise()
    {
        if (this.RandomBackgroundNoiseCoroutine != null)
        {
            StopCoroutine(this.RandomBackgroundNoiseCoroutine);
            this.RandomBackgroundNoiseCoroutine = null;
        }

        this.NoiseSource.Stop();
    }

    public void PlaySFX(Sound sound, bool stopCurrent = false)
    {
        if (stopCurrent)
        {
            StopSFX();
        }

        var audioClip = AudioClips[sound];
        if (audioClip != null)
        {
            this.SfxSource.PlayOneShot(audioClip);
        }
    }

    public void StopSFX()
    {
        this.SfxSource.Stop();
    }

    public void PlayMenuBackgroundMusic(bool stopCurrent = true)
    {
        if (stopCurrent)
        {
            StopBackgroundMusic();
        }

        PlayBackgroundMusic(Sound.MenuBackgroundMusic);
    }

    public void PlayForestBackgroundMusic(bool stopCurrent = true)
    {
        if (stopCurrent)
        {
            StopBackgroundMusic();
        }

        PlayBackgroundMusic(Sound.ForestBackgroundMusic);
    }

    public void PlayVillageBackgroundMusic(bool stopCurrent = true)
    {
        if (stopCurrent)
        {
            StopBackgroundMusic();
        }

        PlayBackgroundMusic(Sound.ForestBackgroundMusic);
        var randomSoundEffects = new List<Sound>()
        {
            Sound.Cow,
            Sound.Tractor,
        };

        this.RandomBackgroundNoiseCoroutine = PlayRandomBackgroundNoises(randomSoundEffects);
        StartCoroutine(this.RandomBackgroundNoiseCoroutine);
    }

    public void PlayCityBackgroundMusic(bool stopCurrent = true)
    {
        if (stopCurrent)
        {
            StopBackgroundMusic();
        }

        PlayBackgroundMusic(Sound.CityBackgroundMusic);

        var randomSoundEffects = new List<Sound>()
        {
            Sound.Horns,
            Sound.MultipleHorns,
            Sound.SingleHorn,
        };

        this.RandomBackgroundNoiseCoroutine = PlayRandomBackgroundNoises(randomSoundEffects);
        StartCoroutine(this.RandomBackgroundNoiseCoroutine);
    }

    private IEnumerator PlayRandomBackgroundNoises(List<Sound> sounds)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(5f, 15f));
            PlayNoise(sounds[Random.Range(0, sounds.Count)]);
        }
    }

    public void PlayNoise(Sound sound, bool stopCurrent = false)
    {
        if (stopCurrent)
        {
            StopBackgroundNoise();
        }

        var audioClip = AudioClips[sound];
        if (audioClip != null)
        {
            this.NoiseSource.PlayOneShot(audioClip);
        }
    }

    private void OnUpdateMusicVolume(float volume)
    {
        this.MusicSource.volume = volume;
        this.NoiseSource.volume = volume; // Also for background noises
        this.MusicVolumeSlider.image.sprite = volume > 0f ? this.UnmutedSprite : this.MutedSprite;
    }

    private void OnUpdateSfxVolume(float volume)
    {
        this.SfxSource.volume = volume;
        this.SfxVolumeSlider.image.sprite = volume > 0f ? this.UnmutedSprite : this.MutedSprite;
    }
}
