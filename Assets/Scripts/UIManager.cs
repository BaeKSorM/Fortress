using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Slider canMoveSlider;
    public enum CurrentScene { Ready, Game };
    public CurrentScene currentScene;
    public enum SelectedWeaponType { Shot, Three_Ball, One_Bounce, Roller, Back_Roller, Granade, Spliter, Breaker, Sniper };
    public SelectedWeaponType selectedWeaponType;
    public Transform WeaponOptions;
    public List<Button> weapons;
    public Button WeaponChoose;
    public Button GameStart;
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
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
        GameStart = GameObject.Find("GoGame").GetComponent<Button>();
        GameStart.onClick.AddListener(() => GoGame());
    }
    void GoGame()
    {
        // currentScene = CurrentScene.Game;
        SceneManager.LoadSceneAsync(1);
    }
    void OpenWeaponOptions()
    {
        WeaponOptions.gameObject.SetActive(true);
    }
    void CloseWeaponOption()
    {
        WeaponOptions.gameObject.SetActive(false);
    }
    void SetWeapon(int _count)
    {
        selectedWeaponType = (SelectedWeaponType)_count;
    }
    public void DecreaseWeaponCount(int weaponType)
    {
        int currentWeaponCount = int.Parse(weapons[weaponType].transform.GetChild(1).GetComponent<TMP_Text>().text);
        weapons[weaponType].transform.GetChild(1).GetComponent<TMP_Text>().text = (--currentWeaponCount).ToString();
    }
    void Update()
    {
        switch (currentScene)
        {
            // case CurrentScene.Ready:
            //     break;
            case CurrentScene.Game:
                if (canMoveSlider.value <= 0)
                {
                    PlayerController.Instance.canMove = false;
                }
                break;
            default:
                break;
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            canMoveSlider = GameObject.Find("CanMoveSlider").GetComponent<Slider>();
            WeaponOptions = GameObject.Find("WeaponOptions").transform;
            WeaponChoose = GameObject.Find("WeaponChoose").GetComponent<Button>();
            WeaponChoose.onClick.AddListener(OpenWeaponOptions);
            CloseWeaponOption();
            for (int i = 0; i < WeaponOptions.childCount; ++i)
            {
                int count = i;
                weapons.Add(WeaponOptions.GetChild(i).GetComponent<Button>());
                weapons[i].onClick.AddListener(() => SetWeapon(count));
                weapons[i].onClick.AddListener(CloseWeaponOption);
            }
        }
    }
}
