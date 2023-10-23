using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Help : MonoBehaviour
{
    public GameObject helpPanel;
    public GameObject weaponExplain;
    public Button weaponLeftButton;
    public Button weaponRightButton;
    public int weaponExplainCount;
    public GameObject controlExplain;
    public Button controlLeftButton;
    public Button controlRightButton;
    public int controlExplainCount;
    void Start()
    {
        helpPanel = GameObject.Find("HelpPanel");
        weaponExplain = helpPanel.transform.GetChild(1).gameObject;
        controlExplain = helpPanel.transform.GetChild(2).gameObject;
        weaponRightButton = weaponExplain.transform.GetChild(0).GetComponent<Button>();
        weaponLeftButton = weaponExplain.transform.GetChild(1).GetComponent<Button>();
        controlLeftButton = controlExplain.transform.GetChild(0).GetComponent<Button>();
        controlRightButton = controlExplain.transform.GetChild(1).GetComponent<Button>();
    }

    void WeaponExplainRight()
    {
        if (controlExplainCount > 8)
        {
            controlExplainCount = 0;
        }
    }
    void WeaponExplainLeft()
    {
    }
}

