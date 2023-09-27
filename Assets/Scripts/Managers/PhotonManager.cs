using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using Unity.Mathematics;


public class PhotonManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (UIManager.Instance.roomNames.Contains(room.Name))
            {
                continue;
            }
            UIManager.Instance.roomNames.Add(room.Name);
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 연결");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 연결");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("방연결");
        UIManager.Instance.inviteKey.text = "Invite : " + PhotonNetwork.CurrentRoom.Name;
        UIManager.Instance.roomUI.gameObject.SetActive(true);
        SettingPlayerInfo();
        if (!photonView.IsMine)
        {
            UIManager.Instance.buttons.gameObject.SetActive(false);
        }
        photonView.RPC("SettingPlayerInfo", RpcTarget.OthersBuffered);
        // UIManager.Instance.RefreshPlayerInfo();
    }
    [PunRPC]
    public void SettingPlayerInfo()
    {
        PhotonNetwork.Instantiate("Prefabs/UI/PlayerInfomation", Vector2.zero, quaternion.identity).transform.SetParent(UIManager.Instance.content);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        UIManager.Instance.MakeRoom();
    }
}
