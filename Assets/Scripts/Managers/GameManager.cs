using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    void Awake()
    {
        // UIManager 인스턴스를 설정하고 중복 생성을 방지합니다.
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
    void OnEnable()
    {
        switch (UIManager.Instance.currentScene)
        {
            case UIManager.CurrentScene.Ready:
                SceneManager.sceneLoaded += OnSceneLoaded;
                break;
            case UIManager.CurrentScene.Game:
                SceneManager.sceneLoaded += OnSceneLoaded;
                break;
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            UIManager.Instance.InitializeReadySceneUI();
            UIManager.Instance.currentScene = UIManager.CurrentScene.Ready;
        }
        else
        {
            UIManager.Instance.InitializeGameSceneUI();
            UIManager.Instance.currentScene = UIManager.CurrentScene.Game;
        }
    }
}
