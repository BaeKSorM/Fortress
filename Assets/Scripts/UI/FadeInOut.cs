using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeInOut : MonoBehaviour
{
    public static FadeInOut Instance;
    public float fadeDuration = 1.0f;
    public Image image;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        image = transform.GetChild(0).GetComponent<Image>();
    }
    public IEnumerator FadeIn()
    {
        image.gameObject.SetActive(true);
        float startTime = Time.time;
        float endTime = startTime + fadeDuration;
        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float alpha = elapsedTime / fadeDuration;
            image.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        image.color = new Color(0, 0, 0, 1);
    }
    public IEnumerator FadeOut()
    {
        float startTime = Time.time;
        float endTime = startTime + fadeDuration;
        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float alpha = 1 - elapsedTime / fadeDuration;
            image.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        image.color = new Color(0, 0, 0, 0);
        image.gameObject.SetActive(false);
    }
}
