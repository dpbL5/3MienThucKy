using UnityEngine;

public static class MusicPlayer
{
    private const string MusicKey = "Settings_Music";

    private static AudioSource source;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (source != null) return;

        var clip = Resources.Load<AudioClip>("bgm_main");
        if (clip == null) return;

        var go = new GameObject("MusicPlayer");
        Object.DontDestroyOnLoad(go);
        source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.clip = clip;
        source.loop = true;
        source.volume = PlayerPrefs.GetFloat(MusicKey, 1f);
        source.Play();
    }

    public static void SetVolume(float value)
    {
        if (source != null) source.volume = value;
    }
}
