using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance;  // UIManager의 인스턴스를 관리합니다.
    public Sprite testImage;
    // 게임 UI 요소
    public GameObject niddle;  // 움직임 슬라이더
    public enum CurrentScene { Ready, Game };  // 현재 장면 상태를 나타내는 열거형
    public CurrentScene currentScene;  // 현재 장면 상태 변수
    public enum SelectedWeaponType { Shot, Three_Ball, One_Bounce, Roller, Back_Roller, Granade, Spliter, Breaker, Sniper };  // 선택한 무기 유형
    public enum SelectedItemType { Double, Shield };  // 선택한 아이템 유형
    public SelectedWeaponType selectedWeaponType;  // 선택한 무기 유형 변수
    public SelectedItemType selectedItemType;  // 선택한 아이템 유형 변수

    // UI 요소
    public Transform weaponOptions;  // 무기 옵션 메뉴
    public Transform itemOptions;
    public List<Button> weapons;  // 무기 버튼 리스트
    public List<Button> items;  // 무기 버튼 리스트
    public Button weaponsButton;  // 무기 선택 버튼
    public Button itemsButton;  // 무기 선택 버튼
    public Button gameStart;  // 게임 시작 버튼
    public Button gameReady;
    public Button gameReadyCancel;
    public Button gameQuit;

    // Ready 장면 UI 요소
    public Transform roomUI;  // 방 관련 UI
    public Transform lobby;  // 로비
    public Transform players;  // 플레이어 목록
    public Transform content;
    public Transform maps;  // 맵 목록
    public Transform infomationBG;
    public Transform custom;  // 탱크 커스터마이징
    public Transform stats;  // 통계
    public TMP_Text restStats;
    public int restStatsCount;
    public int restStatsMaxCount;
    public enum ItemType { doubleShot, shield };
    public ItemType itemType;
    public Transform doubleShot;
    public int doubleShotCount;
    public Transform doubleShotCounts;
    public Button doubleShotPlus;
    public Button doubleShotMinus;
    public Transform shield;
    public int shieldCount;
    public Transform shieldCounts;
    public Button shieldPlus;
    public Button shieldMinus;
    public int roomNameLength;  // 방 이름 길이
    public int readiedPlayerCount;
    public Transform buttons;
    public Button previousMapButton;  // 이전 맵 버튼
    public Button nextMapButton;  // 다음 맵 버튼
    public List<Button> tankImageChangeButtons;
    public TMP_Text mapName;  // 맵 이름 텍스트
    public TMP_Text inviteKey;
    public TMP_Text mapMaxPlayerText;  // 맵 최대 플레이어 수 텍스트

    // 맵과 탱크 커스터마이징을 위한 리스트들
    public List<string> mapNames;  // 맵 이름 리스트
    public List<string> tankNames;  // 탱크 이름 리스트
    public List<int> mapMaxPlayerCounts;  // 맵 최대 플레이어 수 리스트
    public List<int> currentImageIndexes;  // 현재 탱크 이미지 인덱스 리스트
    public List<Image> currentImage;  // 

    // Room Lobby를 위한 UI 요소들
    public Button makeRoomButton;  // 방 만들기 버튼
    public Button joinRoomButton;  // 방 입장 버튼
    public Button joinRandomRoomButton;  // 랜덤 방 입장 버튼
    public Transform insertRooomKey;  // 방 키 입력 창
    public Button enterJoinRoomButton;  // 방 입장 확인 버튼
    public Button cancelJoinRoomButton;  // 방 입장 취소 버튼
    public TMP_InputField roomKey;  // 방 키 입력 필드
    public TMP_Text ConnectionStatus;  // 연결 상태 텍스트
    public TMP_Text text;

    // 기타 변수들
    #region 나중에 게임 넘어갈때 저장해서 가져감
    public Image playerCannonImage;  // 플래이어창 탱크 포 이미지
    public Image playerTankTopImage;  // 플래이어창 탱크 상단 이미지
    public Image playerTankBottomImage;  // 플래이어창 탱크 하단 이미지
    #endregion
    public GameObject wrongRoomKeyMessage;  // 잘못된 방 키 메시지
    public List<string> roomNames;  // 방 이름 리스트

    // Awake 함수: 게임 오브젝트 생성 시 호출
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

    // Ready 장면 UI 초기화 함수
    public void InitializeReadySceneUI()
    {
        PhotonNetwork.ConnectUsingSettings();

        gameStart = GameObject.Find("GoGame").GetComponent<Button>();
        gameStart.onClick.AddListener(() => GoGame());
        gameReady = GameObject.Find("ReadyGame").GetComponent<Button>();
        gameReady.onClick.AddListener(() => ReadyGame());
        gameReadyCancel = GameObject.Find("ReadyCancelGame").GetComponent<Button>();
        gameReadyCancel.onClick.AddListener(() => ReadyCancelGame());
        gameQuit = GameObject.Find("QuitGame").GetComponent<Button>();
        gameQuit.onClick.AddListener(() => QuitGame());


        lobby = GameObject.Find("Lobby").transform;
        makeRoomButton = lobby.transform.Find("MakeRoomButton").GetComponent<Button>();
        joinRoomButton = lobby.transform.Find("JoinRoomButton").GetComponent<Button>();
        joinRandomRoomButton = lobby.transform.Find("JoinRandomRoomButton").GetComponent<Button>();
        insertRooomKey = lobby.Find("InsertRoomKeyPanel");
        enterJoinRoomButton = insertRooomKey.Find("InsertRoomKeyBG").Find("Enter").GetComponent<Button>();
        enterJoinRoomButton.onClick.AddListener(() => EnterJoinRoom());
        cancelJoinRoomButton = insertRooomKey.Find("InsertRoomKeyBG").Find("Cancel").GetComponent<Button>();
        cancelJoinRoomButton.onClick.AddListener(() => InsertRoomKeyCancel());
        roomKey = insertRooomKey.Find("InsertRoomKeyBG").Find("InsertRoomKeyInputField").GetComponent<TMP_InputField>();
        wrongRoomKeyMessage = insertRooomKey.Find("WrongRoomKeyPanel").gameObject;
        makeRoomButton.onClick.AddListener(() => MakeRoom());
        joinRoomButton.onClick.AddListener(() => JoinRoom());
        joinRandomRoomButton.onClick.AddListener(() => JoinRandomRoom());
        lobby.gameObject.SetActive(false);
        insertRooomKey.gameObject.SetActive(false);
        wrongRoomKeyMessage.SetActive(false);


        roomUI = GameObject.Find("RoomUI").transform;


        players = roomUI.Find("Players");
        content = players.GetChild(0).GetChild(0).GetChild(0);


        maps = roomUI.Find("Maps");
        currentImage[3] = maps.GetComponent<Image>();
        infomationBG = maps.Find("InfomationBG");
        buttons = maps.Find("Buttons");
        mapName = infomationBG.Find("MapName").GetComponent<TMP_Text>();
        mapMaxPlayerText = infomationBG.Find("MaxPlayerCount").GetComponent<TMP_Text>();
        inviteKey = infomationBG.Find("InviteKey").GetComponent<TMP_Text>();
        previousMapButton = buttons.Find("Previous").GetComponent<Button>();
        previousMapButton.onClick.AddListener(() => PhotonPreviousImage(mapNames, currentImageIndexes[3], 3, 0));
        nextMapButton = buttons.Find("Next").GetComponent<Button>();
        nextMapButton.onClick.AddListener(() => PhotonNextImage(mapNames, currentImageIndexes[3], 3, 0));


        custom = roomUI.Find("Custom");
        currentImage[4] = custom.Find("TankCannonImage").GetComponent<Image>();
        currentImage[5] = custom.Find("TankTopImage").GetComponent<Image>();
        currentImage[6] = custom.Find("TankBottomImage").GetComponent<Image>();


        stats = roomUI.Find("Stats");
        restStats = stats.GetChild(0).GetComponent<TMP_Text>();
        doubleShot = stats.Find("DoubleShotBG");
        doubleShotCounts = doubleShot.GetChild(1);
        doubleShotPlus = doubleShot.Find("Plus").GetComponent<Button>();
        doubleShotPlus.onClick.AddListener(() => AddStat(ItemType.doubleShot));
        doubleShotMinus = doubleShot.Find("Minus").GetComponent<Button>();
        doubleShotMinus.onClick.AddListener(() => SubtractStat(ItemType.doubleShot));
        shield = stats.Find("ShieldBG");
        shieldCounts = shield.GetChild(1);
        shieldPlus = shield.Find("Plus").GetComponent<Button>();
        shieldPlus.onClick.AddListener(() => AddStat(ItemType.shield));
        shieldMinus = shield.Find("Minus").GetComponent<Button>();
        shieldMinus.onClick.AddListener(() => SubtractStat(ItemType.shield));

        roomUI.gameObject.SetActive(false);

    }

    // Game 장면 UI 초기화 함수
    public void InitializeGameSceneUI()
    {
        // 게임 장면의 UI를 초기화합니다.
        niddle = GameObject.Find("Niddle");
        weaponOptions = GameObject.Find("WeaponOptions").transform;
        itemOptions = GameObject.Find("ItemOptions").transform;
        weaponsButton = GameObject.Find("Weapons").GetComponent<Button>();
        itemsButton = GameObject.Find("Items").GetComponent<Button>();
        weaponsButton.onClick.AddListener(OpenWeaponOptions);
        itemsButton.onClick.AddListener(OpenItemOptions);
        CloseWeaponOption();
        int count = 0;
        // 무기 버튼을 설정합니다.
        for (int i = 0; i < weaponOptions.childCount; ++i)
        {
            count = i;
            weapons.Add(weaponOptions.GetChild(i).GetComponent<Button>());
            weapons[i].onClick.AddListener(() => SetWeapon(count));
            weapons[i].onClick.AddListener(CloseWeaponOption);
        }
        // for(int i=0;i<)
        for (int i = 0; i < itemOptions.childCount; ++i)
        {
            if (itemOptions.GetChild(i).name.Contains("Double"))
            {
                count = 0;
            }
            else if (itemOptions.GetChild(i).name.Contains("Shield"))
            {
                count = 1;
            }
            items[i].onClick.AddListener(() => SetItem(count));
            items.Add(weaponOptions.GetChild(i).GetComponent<Button>());
            items[i].onClick.AddListener(CloseItemOption);
        }
    }



    // GoGame 함수: 게임 장면으로 이동
    void GoGame()
    {
        SceneManager.LoadSceneAsync(mapName.text);
    }
    void ReadyGame()
    {
        photonView.RPC("RefreshReadiedPlayer", RpcTarget.All, 1);
        gameReady.gameObject.SetActive(false);
        gameReadyCancel.gameObject.SetActive(true);
    }
    void ReadyCancelGame()
    {
        photonView.RPC("RefreshReadiedPlayer", RpcTarget.All, -1);
        gameReady.gameObject.SetActive(true);
        gameReadyCancel.gameObject.SetActive(false);
    }
    void QuitGame()
    {
        PhotonNetwork.LeaveRoom();
        roomUI.gameObject.SetActive(false);
    }
    void AddStat(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.doubleShot:
                if (restStatsCount > 0 && doubleShotCount < doubleShotCounts.childCount)
                {
                    doubleShotCounts.GetChild(doubleShotCount++).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/UI/StatFilled");
                    restStats.text = "Rest Stats : " + --restStatsCount;
                }
                break;
            case ItemType.shield:
                if (restStatsCount > 0 && shieldCount < shieldCounts.childCount)
                {
                    shieldCounts.GetChild(shieldCount++).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/UI/StatFilled");
                    restStats.text = "Rest Stats : " + --restStatsCount;
                }
                break;
            default:
                break;
        }
    }
    void SubtractStat(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.doubleShot:
                if (restStatsCount < restStatsMaxCount && doubleShotCount > 0)
                {
                    doubleShotCounts.GetChild(--doubleShotCount).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/UI/StatUnFilled");
                    restStats.text = "Rest Stats : " + ++restStatsCount;
                }
                break;
            case ItemType.shield:
                if (restStatsCount < restStatsMaxCount && shieldCount > 0)
                {
                    shieldCounts.GetChild(--shieldCount).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/UI/StatUnFilled");
                    restStats.text = "Rest Stats : " + ++restStatsCount;
                }
                break;
            default:
                break;
        }
    }
    [PunRPC]
    public void RefreshReadiedPlayer(int plusMinus)
    {
        readiedPlayerCount += plusMinus;
    }
    // OpenWeaponOptions 함수: 무기 옵션 메뉴 열기
    void OpenWeaponOptions()
    {
        weaponOptions.gameObject.SetActive(true);
    }
    // CloseWeaponOption 함수: 무기 옵션 메뉴 닫기
    void CloseWeaponOption()
    {
        weaponOptions.gameObject.SetActive(false);
    }
    void OpenItemOptions()
    {
        itemOptions.gameObject.SetActive(true);
    }
    // CloseWeaponOption 함수: 무기 옵션 메뉴 닫기
    void CloseItemOption()
    {
        itemOptions.gameObject.SetActive(false);
    }

    // SetWeapon 함수: 선택한 무기 설정
    void SetWeapon(int _count)
    {
        selectedWeaponType = (SelectedWeaponType)_count;
    }
    void SetItem(int _count)
    {
        selectedItemType = (SelectedItemType)_count;
    }
    // DecreaseWeaponCount 함수: 무기 개수 감소
    public void DecreaseWeaponCount(int weaponType)
    {
        int currentWeaponCount = int.Parse(weapons[weaponType].transform.GetChild(1).GetComponent<TMP_Text>().text);
        weapons[weaponType].transform.GetChild(1).GetComponent<TMP_Text>().text = (--currentWeaponCount).ToString();
    }

    // PreviousImage 함수: 이전 이미지로 변경

    public void PhotonPreviousImage(List<string> imageNames, int _currentImageIndex, int changingImageIndex, int type)
    {
        Debug.Log(_currentImageIndex);
        string imagePath;
        if (_currentImageIndex - 1 < 0)
        {
            _currentImageIndex = imageNames.Count - 1;
        }
        else
        {
            --_currentImageIndex;
        }
        currentImageIndexes[changingImageIndex] = _currentImageIndex;
        if (changingImageIndex < 3)
        {
            imagePath = "Tanks/" + imageNames[_currentImageIndex];
            photonView.RPC("ChangeImageRPC", RpcTarget.All, imagePath, changingImageIndex, playerOrder, type);
        }
        else if (changingImageIndex < 4)
        {
            photonView.RPC("RefreshMapInfo", RpcTarget.All, _currentImageIndex);
            imagePath = "Backgrounds/" + imageNames[_currentImageIndex];
            photonView.RPC("ChangeImageRPC", RpcTarget.All, imagePath, changingImageIndex, type);
        }
    }
    public void PreviousImage(List<string> imageNames, int _currentImageIndex, int changingImageIndex, int type)
    {
        Debug.Log("P");
        string imagePath = "";
        if (_currentImageIndex - 1 < 0)
        {
            _currentImageIndex = imageNames.Count - 1;
        }
        else
        {
            --_currentImageIndex;
        }
        currentImageIndexes[changingImageIndex] = _currentImageIndex;
        if (changingImageIndex < 7)
        {
            imagePath = "Tanks/" + imageNames[_currentImageIndex];
        }

        ChangeImage(imagePath, changingImageIndex, type);
    }


    // NextImage 함수: 다음 이미지로 변경

    // 이미지이름들(포, 상단, 하단), 현재 색 번호, 바뀔이미지 번호(탱크, 맵)
    public void PhotonNextImage(List<string> imageNames, int _currentImageIndex, int changingImageIndex, int type)
    {
        Debug.Log(_currentImageIndex + 1);
        string imagePath = "";
        if (_currentImageIndex + 1 > imageNames.Count - 1)
        {
            _currentImageIndex = 0;
        }
        else
        {
            ++_currentImageIndex;
        }
        currentImageIndexes[changingImageIndex] = _currentImageIndex;
        if (changingImageIndex < 3)
        {
            imagePath = "Tanks/" + imageNames[_currentImageIndex];
            photonView.RPC("ChangeImageRPC", RpcTarget.All, imagePath, changingImageIndex, playerOrder, type);
        }
        else if (changingImageIndex < 4)
        {
            photonView.RPC("RefreshMapInfo", RpcTarget.All, _currentImageIndex);
            imagePath = "Backgrounds/" + imageNames[_currentImageIndex];
            photonView.RPC("ChangeImageRPC", RpcTarget.All, imagePath, changingImageIndex, type);
        }
    }
    public void NextImage(List<string> imageNames, int _currentImageIndex, int changingImageIndex, int type)
    {
        Debug.Log("N");
        string imagePath = "";
        if (_currentImageIndex + 1 > imageNames.Count - 1)
        {
            _currentImageIndex = 0;
        }
        else
        {
            ++_currentImageIndex;
        }
        currentImageIndexes[changingImageIndex] = _currentImageIndex;
        if (changingImageIndex < 7)
        {
            imagePath = "Tanks/" + imageNames[_currentImageIndex];
        }
        ChangeImage(imagePath, changingImageIndex, type);
    }
    public void ChangeImage(string _imagePath, int _changingImageIndex, int _type)
    {
        Sprite sprite = Resources.LoadAll<Sprite>("Images/" + _imagePath)[_type];
        currentImage[_changingImageIndex].sprite = sprite;
    }
    [PunRPC]
    // ChangeImage 함수: 이미지 변경
    public void ChangeImageRPC(string _imagePath, int _changingImageIndex, int playerOrder, int _type)
    {
        Sprite sprite = Resources.LoadAll<Sprite>("Images/" + _imagePath)[_type];
        content.GetChild(playerOrder).GetChild(2).GetChild(_changingImageIndex).GetComponent<Image>().sprite = sprite;
    }
    [PunRPC]
    // ChangeImage 함수: 이미지 변경
    public void ChangeImageRPC(string _imagePath, int _changingImageIndex, int _type)
    {
        Sprite sprite = Resources.LoadAll<Sprite>("Images/" + _imagePath)[_type];
        currentImage[_changingImageIndex].sprite = sprite;
    }
    [PunRPC]
    public void RefreshMapInfo(int _currentImageIndex)
    {
        mapName.text = mapNames[_currentImageIndex];
        mapMaxPlayerText.text = "MaxPlayers : " + mapMaxPlayerCounts[_currentImageIndex];
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
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MakeRoom();
    }
    // MakeRoom 함수: 방 만들기
    public void MakeRoom()
    {
        string roomName = RandomRoomName(roomNameLength);
        PhotonNetwork.CreateRoom(roomName);
        Debug.Log("방 생성");
    }

    // JoinRoom 함수: 방 입장
    public void JoinRoom()
    {
        insertRooomKey.gameObject.SetActive(true);
    }

    // JoinRandomRoom 함수: 랜덤 방 입장
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    // EnterJoinRoom 함수: 방 입장 확인
    public void EnterJoinRoom()
    {
        if (roomNames.Contains(roomKey.text))
        {
            PhotonNetwork.JoinRoom(roomKey.text);
        }
        else
        {
            StartCoroutine(WrongKeyMessage());
        }
    }

    // InsertRoomKeyCancel 함수: 방 키 입력 취소
    public void InsertRoomKeyCancel()
    {
        insertRooomKey.gameObject.SetActive(false);
        roomKey.text = "";
    }

    // WrongKeyMessage 함수: 잘못된 방 키 메시지 표시
    IEnumerator WrongKeyMessage()
    {
        wrongRoomKeyMessage.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        wrongRoomKeyMessage.SetActive(false);
    }

    // RandomRoomName 함수: 랜덤 방 이름 생성
    public string RandomRoomName(int _roomNameLength)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string randomName;
        do
        {
            randomName = "";
            for (int i = 0; i < _roomNameLength; i++)
            {
                randomName += chars[Random.Range(0, chars.Length)];
            }
        } while (roomNames.Contains(randomName));
        roomNames.Add(randomName);
        return randomName;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (roomNames.Contains(room.Name))
            {
                continue;
            }
            roomNames.Add(room.Name);
        }
    }
    public GameObject playerInfo;
    public override void OnJoinedRoom()
    {
        Debug.Log("방연결");
        inviteKey.text = "Invite : " + PhotonNetwork.CurrentRoom.Name;
        roomUI.gameObject.SetActive(true);
        string cannon = currentImage[4].GetComponent<Image>().sprite.name;
        int cannonIdx = cannon.IndexOf("_");
        cannon = cannon.Substring(0, cannonIdx);
        string top = currentImage[5].GetComponent<Image>().sprite.name;
        int topIdx = top.IndexOf("_");
        top = top.Substring(0, topIdx);
        string bottom = currentImage[6].GetComponent<Image>().sprite.name;
        int bottomIdx = bottom.IndexOf("_");
        bottom = bottom.Substring(0, bottomIdx);
        photonView.RPC("SettingPlayerInfo", RpcTarget.AllBufferedViaServer, cannon, top, bottom);

        if (!photonView.IsMine)
        {
            buttons.gameObject.SetActive(false);
            gameStart.gameObject.SetActive(false);
            gameReady.gameObject.SetActive(true);
        }
        else
        {
            currentImage[3].sprite = Resources.Load<Sprite>("images/Backgrounds/" + mapNames[0]);
            gameStart.gameObject.SetActive(true);
            gameReady.gameObject.SetActive(false);
        }
        gameReadyCancel.gameObject.SetActive(false);
        playerActorNumbers.Add(PhotonNetwork.LocalPlayer.ActorNumber);
        photonView.RPC("RefreshPlayerInfo", RpcTarget.AllViaServer);
    }
    public bool isSetButtons;
    public int playerOrder;
    public void SetPhotonButtons()
    {
        playerOrder = content.childCount - 1;
        for (int i = 0; i < tankImageChangeButtons.Count; i += 2)
        {
            int count = i / 2;
            tankImageChangeButtons[i] = currentImage[count + 4].transform.Find("Previous").GetComponent<Button>();
            tankImageChangeButtons[i].onClick.AddListener(() => PhotonPreviousImage(tankNames, currentImageIndexes[count], count, count));
            tankImageChangeButtons[i].onClick.AddListener(() => PreviousImage(tankNames, currentImageIndexes[count + 4], count + 4, count));
            tankImageChangeButtons[i + 1] = currentImage[count + 4].transform.Find("Next").GetComponent<Button>();
            tankImageChangeButtons[i + 1].onClick.AddListener(() => PhotonNextImage(tankNames, currentImageIndexes[count], count, count));
            tankImageChangeButtons[i + 1].onClick.AddListener(() => NextImage(tankNames, currentImageIndexes[count + 4], count + 4, count));
        }
        isSetButtons = true;
    }
    [PunRPC]
    public void SettingPlayerInfo(string cannonName, string topName, string bottomName)
    {
        playerInfo = PhotonNetwork.Instantiate("Prefabs/UI/PlayerInfomation", Vector2.zero, Quaternion.identity);
        playerInfo.transform.SetParent(content);

        playerInfo.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Images/Tanks/" + cannonName)[0];
        playerInfo.transform.GetChild(2).GetChild(1).GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Images/Tanks/" + topName)[1];
        playerInfo.transform.GetChild(2).GetChild(2).GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Images/Tanks/" + bottomName)[2];
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.ActorNumber);
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount + 1; ++i)
        {
            if (playerActorNumbers[i] == otherPlayer.ActorNumber)
            {
                PhotonNetwork.Destroy(content.GetChild(i).gameObject);
                Debug.Log(playerOrder);
                Debug.Log(i + 1);
                if (playerOrder > i)
                {
                    --playerOrder;
                }
                playerActorNumbers.RemoveAt(i);
                break;
            }
        }
        if (photonView.IsMine)
        {
            if (gameReadyCancel.gameObject.activeSelf)
            {
                photonView.RPC("RefreshReadiedPlayer", RpcTarget.All, -1);
                gameReadyCancel.gameObject.SetActive(false);
            }
            else if (gameReady.gameObject.activeSelf)
            {
                gameReady.gameObject.SetActive(false);
            }
            if (!gameStart.gameObject.activeSelf)
            {
                gameStart.gameObject.SetActive(true);
            }
        }
    }
    public List<int> playerActorNumbers;
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerActorNumbers.Add(newPlayer.ActorNumber);
        if (photonView.IsMine)
        {
            photonView.RPC("SetPlayerActorNumbers", RpcTarget.All, playerActorNumbers.ToArray());
            for (int i = 0; i < content.childCount; ++i)
            {
                string cannon = content.GetChild(i).GetChild(2).GetChild(0).GetComponent<Image>().sprite.name;
                int cannonIdx = cannon.IndexOf("_");
                cannon = cannon.Substring(0, cannonIdx);
                string top = content.GetChild(i).GetChild(2).GetChild(1).GetComponent<Image>().sprite.name;
                int topIdx = top.IndexOf("_");
                top = top.Substring(0, topIdx);
                string bottom = content.GetChild(i).GetChild(2).GetChild(2).GetComponent<Image>().sprite.name;
                int bottomIdx = bottom.IndexOf("_");
                bottom = bottom.Substring(0, bottomIdx);
                photonView.RPC("SetOriginPlayerImages", newPlayer, i, cannon, top, bottom);
            }
            int mapMaxPlayersIdx = mapNames.IndexOf(mapName.text);
            photonView.RPC("SetMapImage", newPlayer, mapName.text, mapMaxPlayersIdx);
        }
    }
    [PunRPC]
    public void SetPlayerActorNumbers(int[] currentPlayerNumbers)
    {
        playerActorNumbers = currentPlayerNumbers.ToList();
    }
    [PunRPC]
    public void SetOriginPlayerImages(int playerInfoCount, string cannonName, string topName, string bottomName)
    {
        content.GetChild(playerInfoCount).GetChild(2).GetChild(0).GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Images/Tanks/" + cannonName)[0];
        content.GetChild(playerInfoCount).GetChild(2).GetChild(1).GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Images/Tanks/" + topName)[1];
        content.GetChild(playerInfoCount).GetChild(2).GetChild(2).GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Images/Tanks/" + bottomName)[2];
    }
    [PunRPC]
    public void SetMapImage(string mapNameText, int _mapMaxPlayersIdx)
    {
        currentImage[3].sprite = Resources.Load<Sprite>("Images/Backgrounds/" + mapNameText);
        mapName.text = mapNameText;
        mapMaxPlayerText.text = "MaxPlayers : " + mapMaxPlayerCounts[_mapMaxPlayersIdx];
    }
    [PunRPC]
    public void RefreshPlayerInfo()
    {
        Debug.Log(content.childCount);
        for (int i = 0; i < content.childCount; ++i)
        {
            if (content.GetChild(i).GetComponent<PlayerInfomation>().playerOrder)
            {
                content.GetChild(i).GetComponent<PlayerInfomation>().playerOrder.text = (i + 1).ToString();
                content.GetChild(i).GetComponent<PlayerInfomation>().playerName.text = (i + 1).ToString();
            }
        }
    }
    void Update()
    {
        switch (currentScene)
        {
            case CurrentScene.Ready:
                ConnectionStatus.text = PhotonNetwork.NetworkClientState.ToString();
                if (content.childCount > 0)
                {
                    if (content.childCount != int.Parse(content.GetChild(content.childCount - 1).GetComponent<PlayerInfomation>().playerOrder.text))
                    {
                        photonView.RPC("RefreshPlayerInfo", RpcTarget.AllViaServer);
                    }
                    if (content.childCount == int.Parse(content.GetChild(content.childCount - 1).GetComponent<PlayerInfomation>().playerOrder.text) && !isSetButtons)
                    {
                        isSetButtons = false;
                        SetPhotonButtons();
                    }
                }
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    lobby.gameObject.SetActive(true);
                    makeRoomButton.gameObject.SetActive(true);
                    joinRoomButton.gameObject.SetActive(true);
                    joinRandomRoomButton.gameObject.SetActive(true);
                }
                break;
            case CurrentScene.Game:
                if (niddle.transform.eulerAngles.z > 90)
                {
                    PlayerController.Instance.canMove = false;
                }
                break;
            default:
                break;
        }
    }

}
