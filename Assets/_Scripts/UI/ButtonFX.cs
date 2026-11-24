using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonFX : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float pressScale = 0.88f;
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float punchScale = 1.15f;
    [SerializeField] private float lerpSpeed = 20f;
    [SerializeField] private float punchDecay = 2.5f;

    private Button button;
    private Vector3 baseScale;
    private float punch;
    private bool pressed;
    private bool hovered;
    private Coroutine routine;

    private void Awake()
    {
        button = GetComponent<Button>();
        baseScale = transform.localScale;
        button.onClick.AddListener(Punch);
    }

    private void OnEnable()
    {
        punch = 0f;
        pressed = false;
        hovered = false;
        transform.localScale = baseScale;
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(Punch);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        EnsureRoutine();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        EnsureRoutine();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
        EnsureRoutine();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        pressed = false;
        EnsureRoutine();
    }

    public void Punch()
    {
        punch = 1f;
        EnsureRoutine();
    }

    private void EnsureRoutine()
    {
        if (routine == null) routine = StartCoroutine(ScaleRoutine());
    }

    private IEnumerator ScaleRoutine()
    {
        while (true)
        {
            punch = Mathf.MoveTowards(punch, 0f, Time.unscaledDeltaTime * punchDecay);

            var goal = baseScale;
            if (pressed) goal *= pressScale;
            else if (hovered) goal *= hoverScale;
            goal *= 1f + punch * (punchScale - 1f);

            transform.localScale = Vector3.Lerp(transform.localScale, goal, Mathf.Clamp01(Time.unscaledDeltaTime * lerpSpeed));

            var settled = punch <= 0f && !pressed && !hovered &&
                          (transform.localScale - goal).sqrMagnitude < 0.000001f;
            if (settled)
            {
                transform.localScale = baseScale;
                routine = null;
                yield break;
            }
            yield return null;
        }
    }
}
