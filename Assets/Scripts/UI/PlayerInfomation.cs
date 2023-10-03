using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInfomation : MonoBehaviourPunCallbacks
{
    public TMP_Text playerOrder;
    public TMP_Text playerName;
    public Image playerCannonImage;
    public Image playerTankTopImage;
    public Image playerTankBottomImage;
    public Transform tankImageBG;
    void Start()
    {
        if (transform.parent == null)
        {
            Destroy(gameObject);
        }
        playerOrder = transform.GetChild(0).GetComponent<TMP_Text>();
        playerName = transform.GetChild(1).GetComponent<TMP_Text>();
        tankImageBG = transform.GetChild(2);
        playerCannonImage = tankImageBG.GetChild(0).GetComponent<Image>();
        playerTankTopImage = tankImageBG.GetChild(1).GetComponent<Image>();
        playerTankBottomImage = tankImageBG.GetChild(2).GetComponent<Image>();
    }
}
