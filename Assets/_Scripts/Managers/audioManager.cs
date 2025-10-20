using UnityEngine;
using System;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Channels (auto-create if empty)")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Library")]
    public NamedClip[] musics; // napr. "MainMenu", "Level1Music"
    public NamedClip[] sfx;    // napr. "ButtonClick", "CoinPickup", "Jump", "SwordSlash"

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!musicSource) musicSource = gameObject.AddComponent<AudioSource>();
        if (!sfxSource) sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
    }

    public void PlayMusic(string name)
    {
        var m = musics.FirstOrDefault(x => x.name == name);
        if (m?.clip == null) return;

        musicSource.Stop();
        musicSource.clip = m.clip;
        musicSource.volume = m.volume;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    public void PlaySFX(string name)
    {
        var s = sfx.FirstOrDefault(x => x.name == name);
        if (s?.clip == null) return;
        sfxSource.PlayOneShot(s.clip, s.volume);
    }
}

[Serializable]
public class NamedClip
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}
