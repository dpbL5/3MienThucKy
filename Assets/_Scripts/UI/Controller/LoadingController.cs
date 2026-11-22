using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    
    [SerializeField] private float loadingSpeed; 
    
    [SerializeField] private Image loadingBowl;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private string homeSceneName = "Home";

    void Start()
    {
        loadingBowl.fillAmount = 0f;
        StartCoroutine(FakeLoading());
    }

    IEnumerator FakeLoading()
    {
        while (loadingBowl.fillAmount < 1f)
        {
            loadingText.SetText($"{loadingBowl.fillAmount * 100f:0.0}%");
            loadingBowl.fillAmount += loadingSpeed * Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(homeSceneName);
    }
}
