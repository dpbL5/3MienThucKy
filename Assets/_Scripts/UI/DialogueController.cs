using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [TextArea(2, 4)]
    [SerializeField] private string[] lines;
    [SerializeField] private string nextSceneName = "Level_Play_1";

    private int index = -1;

    private void Start()
    {
        ShowNext();
    }

    private void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        ShowNext();
    }

    public void ShowNext()
    {
        index++;
        if (index >= lines.Length)
        {
            SceneManager.LoadSceneAsync(nextSceneName);
            return;
        }
        if (dialogueText != null) dialogueText.text = lines[index];
    }

    public void Skip()
    {
        SceneManager.LoadSceneAsync(nextSceneName);
    }
}
