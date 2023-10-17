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
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // switch (UIManager.Instance.currentScene)
        // {
        //     case UIManager.CurrentScene.Ready:
        //         break;
        //     case UIManager.CurrentScene.Game:
        //         SceneManager.sceneLoaded += OnSceneLoaded;
        //         break;
        // }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            UIManager.Instance.currentScene = UIManager.CurrentScene.Ready;
            UIManager.Instance.InitializeReadySceneUI();
        }
        else if (scene.buildIndex > 1)
        {
            UIManager.Instance.currentScene = UIManager.CurrentScene.Game;
            UIManager.Instance.InitializeGameSceneUI();
        }
    }
}
