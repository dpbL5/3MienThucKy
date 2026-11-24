using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class UISound
{
    private static AudioSource source;
    private static readonly Dictionary<string, AudioClip[]> groups = new Dictionary<string, AudioClip[]>();
    private static readonly List<Button> hooked = new List<Button>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (source != null) return;

        var go = new GameObject("UISoundPlayer");
        Object.DontDestroyOnLoad(go);
        source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;

        SceneManager.sceneLoaded += OnSceneLoaded;
        HookAll();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HookAll();
    }

    private static void HookAll()
    {
        if (source == null) return;

        var buttons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            Hook(button);
        }
    }

    private static void Hook(Button button)
    {
        if (button == null || hooked.Contains(button)) return;

        hooked.Add(button);
        button.onClick.AddListener(() => Play("click"));
    }

    public static void Play(string group)
    {
        if (source == null) return;

        if (!groups.TryGetValue(group, out var clips) || clips.Length == 0)
        {
            var loaded = new List<AudioClip>();
            for (var i = 1; i <= 9; i++)
            {
                var clip = Resources.Load<AudioClip>($"{group}_{i:000}");
                if (clip == null) break;
                loaded.Add(clip);
            }

            clips = loaded.ToArray();
            groups[group] = clips;
        }

        if (clips.Length == 0) return;

        source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}
