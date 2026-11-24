using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelEntry : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
        {
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var cam = Camera.main;
        if (cam == null)
        {
            return;
        }

        var hit = Physics2D.OverlapPoint(cam.ScreenToWorldPoint(mouse.position.ReadValue()));
        if (hit != null && hit.gameObject == gameObject)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
