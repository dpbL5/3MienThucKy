using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    private const string SoundKey = "Settings_Sound";
    private const string MusicKey = "Settings_Music";
    private const string VibrationKey = "Settings_Vibration";
    private const string LanguageKey = "Settings_Language";

    [Header("Textures")]
    [SerializeField] private Texture2D popupTexture;
    [SerializeField] private Texture2D headerTexture;
    [SerializeField] private Texture2D titleTexture;
    [SerializeField] private Texture2D closeButtonTexture;
    [SerializeField] private Texture2D soundIconTexture;
    [SerializeField] private Texture2D musicIconTexture;
    [SerializeField] private Texture2D vibrationIconTexture;
    [SerializeField] private Texture2D sliderTexture;
    [SerializeField] private Texture2D sliderFillTexture;
    [SerializeField] private Texture2D sliderHandleTexture;
    [SerializeField] private Texture2D languagePanelTexture;
    [SerializeField] private Texture2D prevLangButtonTexture;
    [SerializeField] private Texture2D nextLangButtonTexture;

    [Header("UI")]
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider vibrationSlider;
    [SerializeField] private TMP_Text languageText;
    [SerializeField] private GameObject modalRoot;

    private readonly string[] languages = { "Tieng Viet", "English" };
    private int languageIndex;

    private void Start()
    {
        ApplySprites();
        WireButtons();

        soundSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(SoundKey, 1f));
        musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(MusicKey, 1f));
        vibrationSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(VibrationKey, 0.5f));

        soundSlider.onValueChanged.AddListener(SetSoundVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        vibrationSlider.onValueChanged.AddListener(SetVibrationVolume);

        AudioListener.volume = PlayerPrefs.GetFloat(SoundKey, 1f);
        MusicPlayer.SetVolume(PlayerPrefs.GetFloat(MusicKey, 1f));

        languageIndex = Mathf.Clamp(PlayerPrefs.GetInt(LanguageKey, 0), 0, languages.Length - 1);
        UpdateLanguageText();
    }

    public void ToggleSettings()
    {
        if (modalRoot != null)
        {
            modalRoot.SetActive(!modalRoot.activeSelf);
        }
    }

    public void SetSoundVolume(float value)
    {
        PlayerPrefs.SetFloat(SoundKey, value);
        AudioListener.volume = value;
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicKey, value);
        MusicPlayer.SetVolume(value);
    }

    public void SetVibrationVolume(float value)
    {
        PlayerPrefs.SetFloat(VibrationKey, value);
    }

    public void OnPrevLanguage()
    {
        languageIndex = (languageIndex - 1 + languages.Length) % languages.Length;
        UpdateLanguageText();
    }

    public void OnNextLanguage()
    {
        languageIndex = (languageIndex + 1) % languages.Length;
        UpdateLanguageText();
    }

    public void Close()
    {
        PlayerPrefs.Save();
        if (modalRoot != null)
        {
            modalRoot.SetActive(false);
        }
    }

    private void UpdateLanguageText()
    {
        if (languageText != null)
        {
            languageText.text = languages[languageIndex];
        }
        PlayerPrefs.SetInt(LanguageKey, languageIndex);
    }

    private void ApplySprites()
    {
        SetSprite(FindImage("Popup"), popupTexture);
        SetSprite(FindImage("Popup/Header"), headerTexture);
        SetSprite(FindImage("Popup/Title"), titleTexture);
        SetSprite(FindImage("Popup/CloseButton"), closeButtonTexture);
        SetSprite(FindImage("Popup/SoundIcon"), soundIconTexture);
        SetSprite(FindImage("Popup/MusicIcon"), musicIconTexture);
        SetSprite(FindImage("Popup/VibrationIcon"), vibrationIconTexture);
        SetSprite(FindImage("Popup/SoundSlider/Background"), sliderTexture);
        SetSprite(FindImage("Popup/SoundSlider/Fill Area/Fill"), sliderFillTexture);
        SetSprite(FindImage("Popup/SoundSlider/Handle Slide Area/Handle"), sliderHandleTexture);
        SetSprite(FindImage("Popup/MusicSlider/Background"), sliderTexture);
        SetSprite(FindImage("Popup/MusicSlider/Fill Area/Fill"), sliderFillTexture);
        SetSprite(FindImage("Popup/MusicSlider/Handle Slide Area/Handle"), sliderHandleTexture);
        SetSprite(FindImage("Popup/VibrationSlider/Background"), sliderTexture);
        SetSprite(FindImage("Popup/VibrationSlider/Fill Area/Fill"), sliderFillTexture);
        SetSprite(FindImage("Popup/VibrationSlider/Handle Slide Area/Handle"), sliderHandleTexture);
        SetSprite(FindImage("Popup/LangPanel"), languagePanelTexture);
        SetSprite(FindImage("Popup/PrevLangButton"), prevLangButtonTexture);
        SetSprite(FindImage("Popup/NextLangButton"), nextLangButtonTexture);
    }

    private void WireButtons()
    {
        SetClick("Popup/CloseButton", Close);
        SetClick("Popup/PrevLangButton", OnPrevLanguage);
        SetClick("Popup/NextLangButton", OnNextLanguage);
    }

    private void SetSprite(Image image, Texture2D texture)
    {
        if (image == null || texture == null)
        {
            return;
        }
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }

    private void SetClick(string path, UnityAction action)
    {
        var button = FindComponent<Button>(path);
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }

    private Image FindImage(string path)
    {
        return FindComponent<Image>(path);
    }

    private T FindComponent<T>(string path) where T : Component
    {
        var child = transform.Find(path);
        return child != null ? child.GetComponent<T>() : null;
    }
}
