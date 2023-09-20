using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Slider canMoveSlider;
    public enum SelectedWeaponType { Shot, Three_Ball, One_Bounce, Roller, Back_Roller, Granade, Spliter, Breaker, Sniper };
    public SelectedWeaponType selectedWeaponType;
    public Transform WeaponOptions;
    public List<Button> weapons;
    public Button WeaponChoose;

    void Start()
    {
        Instance = this;
        WeaponOptions = GameObject.Find("WeaponOptions").transform;
        WeaponChoose = GameObject.Find("WeaponChoose").GetComponent<Button>();
        WeaponChoose.onClick.AddListener(OpenWeaponOptions);
        for (int i = 0; i < WeaponOptions.childCount; ++i)
        {
            int count = i;
            weapons.Add(WeaponOptions.GetChild(i).GetComponent<Button>());
            weapons[i].onClick.AddListener(() => SetWeapon(count));
            weapons[i].onClick.AddListener(CloseWeaponOption);
        }
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
        if (canMoveSlider.value <= 0)
        {
            PlayerController.Instance.canMove = false;
        }
    }
}
