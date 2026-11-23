using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void OnSwitchSceneClicked(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void OnModalSwitchClicked(GameObject modal)
    {
        if (modal != null)
        {
            modal.SetActive(!modal.activeSelf);
        }
    }

}
