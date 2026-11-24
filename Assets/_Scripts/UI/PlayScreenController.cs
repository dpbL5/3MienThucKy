using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayScreenController : MonoBehaviour
{
    [System.Serializable]
    public class IngredientPage
    {
        public Sprite[] ingredients;
    }

    [Header("Cooking")]
    [SerializeField] private float roundSeconds = 45f;
    [SerializeField] private Image timerFill;
    [SerializeField] private int requiredCount = 6;
    [SerializeField] private GameObject noodlesVisual;

    [Header("Stove")]
    [SerializeField] private Image stovePowerImage;
    [SerializeField] private Sprite powerOnSprite;
    [SerializeField] private Sprite powerOffSprite;

    [Header("Panel")]
    [SerializeField] private Image[] cardIngredientImages;
    [SerializeField] private IngredientPage[] pages;

    [Header("Pot")]
    [SerializeField] private Transform potAnchor;
    [SerializeField] private float potSpawnRadius = 1.1f;

    [Header("Result")]
    [SerializeField] private GameObject resultPopup;
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite starFilledSprite;
    [SerializeField] private Sprite starEmptySprite;

    private bool stoveOn;
    private bool finished;
    private float remaining;
    private int page;
    private Coroutine stovePulse;
    private Coroutine resultFx;
    private readonly HashSet<int> addedIngredients = new HashSet<int>();

    private void Start()
    {
        remaining = roundSeconds;
        SetStoveVisual(false);
        UpdateTimerFill();
        ApplyPage();
        if (resultPopup != null) resultPopup.SetActive(false);
    }

    private void Update()
    {
        if (finished) return;
        remaining = Mathf.Max(0f, remaining - Time.deltaTime);
        UpdateTimerFill();
        if (remaining <= 0f) Evaluate(false);
    }

    public void ToggleStove()
    {
        if (finished) return;
        stoveOn = !stoveOn;
        SetStoveVisual(stoveOn);
    }

    public void NextPage()
    {
        if (pages == null || pages.Length == 0) return;
        page = (page + 1) % pages.Length;
        ApplyPage();
    }

    public void OnCardClicked(string slotIndexText)
    {
        if (finished) return;
        if (!int.TryParse(slotIndexText, out var slot)) return;
        if (cardIngredientImages == null || slot < 0 || slot >= cardIngredientImages.Length) return;
        if (!stoveOn)
        {
            if (stovePulse != null) StopCoroutine(stovePulse);
            stovePulse = StartCoroutine(PulseStove());
            return;
        }
        int globalIndex = page * cardIngredientImages.Length + slot;
        if (!addedIngredients.Add(globalIndex)) return;
        bool isNoodles = pages != null && pages.Length > 0 && pages[page] != null && pages[page].ingredients != null &&
            slot < pages[page].ingredients.Length && pages[page].ingredients[slot] != null &&
            pages[page].ingredients[slot].name.Contains("rice_noodles");
        if (isNoodles && noodlesVisual != null) noodlesVisual.SetActive(true);
        if (!isNoodles) SpawnInPot(globalIndex);
        ApplyPage();
        if (addedIngredients.Count >= requiredCount) Evaluate(true);
    }

    public void Retry()
    {
        SceneManager.LoadSceneAsync(gameObject.scene.name);
    }

    private void ApplyPage()
    {
        if (pages == null || pages.Length == 0 || pages[page] == null) return;
        var pageIngredients = pages[page].ingredients;
        if (pageIngredients == null) return;
        for (var i = 0; i < cardIngredientImages.Length; i++)
        {
            var image = cardIngredientImages[i];
            if (image == null) continue;
            if (i < pageIngredients.Length && pageIngredients[i] != null)
            {
                image.sprite = pageIngredients[i];
                image.enabled = true;
                var color = image.color;
                color.a = addedIngredients.Contains(page * cardIngredientImages.Length + i) ? 0.35f : 1f;
                image.color = color;
            }
            else
            {
                image.enabled = false;
            }
        }
    }

    private void SpawnInPot(int globalIndex)
    {
        if (potAnchor == null || pages == null || pages.Length == 0 || cardIngredientImages == null) return;
        int slot = globalIndex % cardIngredientImages.Length;
        int ingredientPage = globalIndex / cardIngredientImages.Length;
        if (ingredientPage >= pages.Length || pages[ingredientPage] == null) return;
        var ingredients = pages[ingredientPage].ingredients;
        if (ingredients == null || slot >= ingredients.Length || ingredients[slot] == null) return;
        var spawned = new GameObject(ingredients[slot].name);
        spawned.transform.SetParent(potAnchor, false);
        var renderer = spawned.AddComponent<SpriteRenderer>();
        renderer.sprite = ingredients[slot];
        renderer.sortingOrder = 5;
        var angle = Random.Range(0f, Mathf.PI * 2f);
        var distance = Random.Range(0f, potSpawnRadius);
        spawned.transform.localPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * distance;
        spawned.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-25f, 25f));
        spawned.transform.localScale = Vector3.one * Random.Range(0.85f, 1.05f);
    }

    private void SetStoveVisual(bool on)
    {
        if (stovePowerImage == null) return;
        stovePowerImage.sprite = on ? powerOnSprite : powerOffSprite;
    }

    private void UpdateTimerFill()
    {
        if (timerFill == null) return;
        timerFill.fillAmount = roundSeconds <= 0f ? 0f : remaining / roundSeconds;
    }

    private void Evaluate(bool completed)
    {
        finished = true;
        stoveOn = false;
        SetStoveVisual(false);
        int stars = 0;
        if (completed)
        {
            float elapsed = roundSeconds - remaining;
            float frac = elapsed / Mathf.Max(0.0001f, roundSeconds);
            stars = frac <= 1f / 3f ? 3 : frac <= 2f / 3f ? 2 : 1;
        }
        ShowStars(stars);
        if (resultPopup != null) resultPopup.SetActive(true);
        UISound.Play(stars > 0 ? "confirmation" : "error");
        if (resultFx != null) StopCoroutine(resultFx);
        resultFx = StartCoroutine(PlayResultFX(stars));
    }

    private IEnumerator PlayResultFX(int stars)
    {
        var light = resultPopup != null ? resultPopup.transform.Find("Light") : null;
        var lightImage = light != null ? light.GetComponent<Image>() : null;
        var frame = resultPopup != null ? resultPopup.transform.Find("ResultFrame") : null;

        if (lightImage != null)
        {
            var color = lightImage.color;
            color.a = 0f;
            lightImage.color = color;
            light.localRotation = Quaternion.identity;
        }
        if (frame != null) frame.localScale = Vector3.one * 0.85f;

        float e = 0f;
        while (e < 1f)
        {
            e += Time.unscaledDeltaTime / 0.35f;
            if (frame != null) frame.localScale = Vector3.Lerp(Vector3.one * 0.85f, Vector3.one, EaseOutBack(Mathf.Clamp01(e)));
            yield return null;
        }
        if (frame != null) frame.localScale = Vector3.one;

        if (starImages != null)
        {
            for (var i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] == null) continue;
                if (i >= stars)
                {
                    starImages[i].rectTransform.localScale = Vector3.one;
                    continue;
                }
                yield return PopStar(starImages[i].rectTransform);
            }
        }

        if (stars <= 0 || lightImage == null) yield break;

        e = 0f;
        while (e < 1f)
        {
            e += Time.unscaledDeltaTime / 0.6f;
            var color = lightImage.color;
            color.a = Mathf.Lerp(0f, 0.9f, Mathf.Clamp01(e));
            lightImage.color = color;
            light.Rotate(0f, 0f, -30f * Time.unscaledDeltaTime);
            yield return null;
        }

        while (resultPopup != null && resultPopup.activeSelf)
        {
            light.Rotate(0f, 0f, -25f * Time.unscaledDeltaTime);
            var color = lightImage.color;
            color.a = 0.9f + Mathf.Sin(Time.unscaledTime * 2.5f) * 0.08f;
            lightImage.color = color;
            yield return null;
        }
    }

    private IEnumerator PopStar(RectTransform star)
    {
        float e = 0f;
        while (e < 1f)
        {
            e += Time.unscaledDeltaTime / 0.3f;
            star.localScale = Vector3.one * EaseOutBack(Mathf.Clamp01(e));
            yield return null;
        }
        star.localScale = Vector3.one;
    }

    private static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    private IEnumerator PulseStove()
    {
        if (stovePowerImage == null) yield break;
        var t = stovePowerImage.transform;
        var baseScale = t.localScale;
        float e = 0f;
        while (e < 1f)
        {
            e += Time.unscaledDeltaTime * 4f;
            float k = Mathf.Sin(Mathf.Clamp01(e) * Mathf.PI) * 0.18f;
            t.localScale = baseScale * (1f + k);
            yield return null;
        }
        t.localScale = baseScale;
        stovePulse = null;
    }

    private void ShowStars(int stars)
    {
        if (starImages == null) return;
        for (var i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null) continue;
            starImages[i].sprite = i < stars ? starFilledSprite : starEmptySprite;
        }
    }
}
