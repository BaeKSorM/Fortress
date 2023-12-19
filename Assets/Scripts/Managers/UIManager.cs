using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance;  // UIManager의 인스턴스를 관리합니다.
    // 게임 UI 요소
    public GameObject niddle;
    public enum CurrentScene { None, Ready, Game };  // 현재 장면 상태를 나타내는 열거형
    public CurrentScene currentScene;  // 현재 장면 상태 변수
    public enum SelectedWeaponType { Shot, Three_Ball, One_Bounce, Roller, Back_Roller, Granade, Spliter, Breaker, Sniper };  // 선택한 무기 유형
    public SelectedWeaponType selectedWeaponType;  // 선택한 무기 유형 변수
    public enum SelectedItemType { None, Double, Shield };  // 선택한 아이템 유형
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
    public enum ItemType { None, doubleShot, shield };
    public List<int> itemsCount;
    public List<int> weaponsCount;
    public Transform doubleShot;
    public Transform doubleShotCounts;
    public Button doubleShotPlus;
    public Button doubleShotMinus;
    public Transform shield;
    public Transform shieldCounts;
    public Button shieldPlus;
    public Button shieldMinus;
    public int roomNameLength;  // 방 이름 길이
    public int readiedPlayerCount = 1;
    public Transform buttons;
    public Button previousMapButton;  // 이전 맵 버튼
    public Button nextMapButton;  // 다음 맵 버튼
    public List<Button> tankImageChangeButtons;
    public TMP_Text mapNameText;  // 맵 이름 텍스트
    public string mapName;  // 맵 이름 텍스트
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
    // public TMP_Text ConnectionStatus;  // 연결 상태 텍스트

    // 기타 변수들
    #region 
    public string playerCannon;  // 플래이어창 탱크 포 이미지
    public string playerTankTop;  // 플래이어창 탱크 상단 이미지
    public string playerTankBottom;  // 플래이어창 탱크 하단 이미지
    #endregion
    public GameObject errorMessage;  // 잘못된 방 키 메시지
    public TMP_Text errorText;
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
    public GameObject readyPanel;
    // Ready 장면 UI 초기화 함수
    public void InitializeReadySceneUI()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.ConnectUsingSettings();

        // ConnectionStatus = GameObject.Find("CN").GetComponent<TMP_Text>();

        gameStart = GameObject.Find("GoGame").GetComponent<Button>();
        gameStart.onClick.AddListener(() => GoGame());
        gameReady = GameObject.Find("ReadyGame").GetComponent<Button>();
        gameReady.onClick.AddListener(() => ReadyGame());
        gameReadyCancel = GameObject.Find("ReadyCancelGame").GetComponent<Button>();
        gameReadyCancel.onClick.AddListener(() => ReadyCancelGame());
        gameQuit = GameObject.Find("QuitGame").GetComponent<Button>();
        gameQuit.onClick.AddListener(() => QuitGame());


        lobby = GameObject.Find("Lobby").transform;
        readyPanel = GameObject.Find("ReadyPanel");
        readyPanel.SetActive(false);
        makeRoomButton = lobby.transform.Find("MakeRoomButton").GetComponent<Button>();
        joinRoomButton = lobby.transform.Find("JoinRoomButton").GetComponent<Button>();
        joinRandomRoomButton = lobby.transform.Find("JoinRandomRoomButton").GetComponent<Button>();
        insertRooomKey = lobby.Find("InsertRoomKeyPanel");
        enterJoinRoomButton = insertRooomKey.Find("InsertRoomKeyBG").Find("Enter").GetComponent<Button>();
        enterJoinRoomButton.onClick.AddListener(() => EnterJoinRoom());
        cancelJoinRoomButton = insertRooomKey.Find("InsertRoomKeyBG").Find("Cancel").GetComponent<Button>();
        cancelJoinRoomButton.onClick.AddListener(() => InsertRoomKeyCancel());
        roomKey = insertRooomKey.Find("InsertRoomKeyBG").Find("InsertRoomKeyInputField").GetComponent<TMP_InputField>();
        errorMessage = GameObject.Find("ErrorMessagePanel").gameObject;
        errorText = errorMessage.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        makeRoomButton.onClick.AddListener(() => MakeRoom());
        joinRoomButton.onClick.AddListener(() => JoinRoom());
        joinRandomRoomButton.onClick.AddListener(() => JoinRandomRoom());
        lobby.gameObject.SetActive(false);
        insertRooomKey.gameObject.SetActive(false);
        errorMessage.SetActive(false);

        roomUI = GameObject.Find("RoomUI").transform;

        players = roomUI.Find("Players");
        content = players.GetChild(0).GetChild(0).GetChild(0);

        maps = roomUI.Find("Maps");
        currentImage[3] = maps.GetComponent<Image>();
        infomationBG = maps.Find("InfomationBG");
        buttons = maps.Find("Buttons");
        mapNameText = infomationBG.Find("MapName").GetComponent<TMP_Text>();
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
        restStatsCount = restStatsMaxCount;

        readiedPlayerCount = 1;

        isSetButtons = false;
        pressNextGame = false;

        if (PhotonNetwork.InLobby)
        {
            StartCoroutine(WaitFadeOut());
        }

        for (int i = 0; i < currentImageIndexes.Count; ++i)
        {
            currentImageIndexes[i] = 0;
        }
        if (!photonView.IsMine)
        {
            Debug.Log("!0");
            ReadyCancelGame();
        }
        roomUI.gameObject.SetActive(false);
        lobby.gameObject.SetActive(true);
    }
    public GameObject tank;
    public List<Vector2> mapSpawnPoints = new();
    public List<Vector2> GrassLand = new() { new(-10.9f, 1.8f), new(10.9f, 1.8f) };
    public List<Vector2> DunGeon = new() { new(-10.9f, 1.8f), new(10.9f, 1.8f) };
    public PlayerController playerController;
    public TMP_Text windPowerText;
    public GameObject selectedWeapon;
    public Image selectedWeaponIcon;
    public TMP_Text selectedWeaponName;
    public GameObject selectedItem;
    public Image selectedItemIcon;
    public TMP_Text selectedItemName;
    public Image windDirectionImage;
    public Wind wind;
    public Image myTankCannon;
    public Image myTankTop;
    public Image myTankBottom;
    public Slider hpbar;
    public TMP_Text hp;
    // Game 장면 UI 초기화 함수
    public void InitializeGameSceneUI()
    {
        // 게임 장면의 UI를 초기화합니다.
        // Prefabs / UI / PlayerInfomation;
        // PlayerInfomation
        switch (mapName)
        {
            case "GrassLand":
                mapSpawnPoints = GrassLand;
                break;
            case "Dungeon":
                mapSpawnPoints = DunGeon;
                break;
        }
        string spawnTankName = "Prefabs/Tanks/" + playerCannon + playerTankTop + playerTankBottom + "Tank";
        tank = PhotonNetwork.Instantiate(spawnTankName, mapSpawnPoints[playerOrder], Quaternion.identity);
        playerController = tank.GetComponent<PlayerController>();
        // photonView.RPC("SetPlayerCustoms", RpcTarget.All, playerCannon, playerTankTop, playerTankBottom);
        if (tank.transform.position.x > 0)
        {
            tank.transform.localScale = new Vector2(-1, 1);
            tank.GetComponent<PlayerController>().facingDirection = -1;
            tank.GetComponent<PlayerController>().lookPosition = -1;
        }

        myTankCannon = GameObject.Find("TankCannonImage").GetComponent<Image>();
        myTankTop = GameObject.Find("TankTopImage").GetComponent<Image>();
        myTankBottom = GameObject.Find("TankBottomImage").GetComponent<Image>();
        myTankCannon.sprite = Resources.LoadAll<Sprite>("Images/Tanks/" + playerCannon)[0];
        myTankTop.sprite = Resources.LoadAll<Sprite>("Images/Tanks/" + playerTankTop)[1];
        myTankBottom.sprite = Resources.LoadAll<Sprite>("Images/Tanks/" + playerTankBottom)[2];
        niddle = GameObject.Find("Niddle");
        windPowerText = GameObject.Find("Cloud").transform.GetChild(0).GetComponent<TMP_Text>();
        weaponOptions = GameObject.Find("WeaponOptions").transform;
        itemOptions = GameObject.Find("ItemOptions").transform;
        weaponsButton = GameObject.Find("Weapons").GetComponent<Button>();
        itemsButton = GameObject.Find("Items").GetComponent<Button>();
        weaponsButton.onClick.AddListener(WeaponOptions);
        itemsButton.onClick.AddListener(ItemOptions);
        selectedWeapon = GameObject.Find("selectedWeapon");
        selectedWeaponName = selectedWeapon.transform.GetChild(0).GetComponent<TMP_Text>();
        selectedWeaponIcon = selectedWeapon.transform.GetChild(1).GetComponent<Image>();
        selectedItem = GameObject.Find("selectedItem");
        selectedItemName = selectedItem.transform.GetChild(0).GetComponent<TMP_Text>();
        selectedItemIcon = selectedItem.transform.GetChild(1).GetComponent<Image>();
        windDirectionImage = GameObject.Find("Cloud").GetComponent<Image>();
        turnPanel = GameObject.Find("Turn");
        turnPanel.SetActive(false);
        hpbar = GameObject.Find("HpBar").GetComponent<Slider>();
        hpbar.maxValue = playerController.maxHp;
        hpbar.value = hpbar.maxValue;
        hp = hpbar.transform.GetChild(2).GetComponent<TMP_Text>();
        wind = GameObject.Find("Wind").GetComponent<Wind>();
        for (int i = 0; i < weaponOptions.childCount; ++i)
        {
            int count = i;
            weapons[i] = weaponOptions.GetChild(i).GetComponent<Button>();
            if (i > 0)
            {
                if (weaponsCount[i] == 0)
                {
                    weapons[i].gameObject.SetActive(false);
                }
                weapons[i].transform.GetChild(1).GetComponent<TMP_Text>().text = weaponsCount[i].ToString();
            }
            weapons[i].onClick.AddListener(() => SetWeapon(count));
            weapons[i].onClick.AddListener(WeaponOptions);
        }
        WeaponOptions();
        for (int i = 0; i < itemOptions.childCount; ++i)
        {
            int count = 0;
            if (itemOptions.GetChild(i).name.Contains("Double"))
            {
                count = 1;
            }
            else if (itemOptions.GetChild(i).name.Contains("Shield"))
            {
                count = 2;
            }
            items[i] = itemOptions.GetChild(i).GetComponent<Button>();
            if (i > 0)
            {
                if (itemsCount[i] == 0)
                {
                    items[i].gameObject.SetActive(false);
                }
                items[i].transform.GetChild(1).GetComponent<TMP_Text>().text = itemsCount[i].ToString();
            }

            items[i].onClick.AddListener(() => SetItem(count));
            items[i].onClick.AddListener(ItemOptions);
        }
        ItemOptions();
        winButton = GameObject.Find("Win");
        loseButton = GameObject.Find("Lose");
        winButton.SetActive(false);
        loseButton.SetActive(false);
        currentScene = CurrentScene.Game;
        StartCoroutine(FadeInOut.Instance.FadeOut());
    }
    public GameObject loseButton;
    public GameObject winButton;
    // GoGame 함수: 게임 장면으로 이동
    void GoGame()
    {
        int mapMaxPlayerCount = int.Parse(mapMaxPlayerText.text[mapMaxPlayerText.text.Length - 1].ToString());
        if (readiedPlayerCount == mapMaxPlayerCount)
        {
            photonView.RPC("SavePlayerColor", RpcTarget.All);
        }
        else if (readiedPlayerCount > mapMaxPlayerCount)
        {
            errorMessage.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1280, 355);
            StartCoroutine(ErrorMessage("Too Many Players \nAre Ready"));
        }
        else if (readiedPlayerCount < mapMaxPlayerCount)
        {
            errorMessage.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1280, 355);
            StartCoroutine(ErrorMessage("Not All Players \nHave Pressed Ready"));
        }
    }
    [PunRPC]
    void SavePlayerColor()
    {
        string cannon = currentImage[4].GetComponent<Image>().sprite.name;
        int cannonIdx = cannon.IndexOf("_");
        cannon = cannon.Substring(0, cannonIdx);
        string top = currentImage[5].GetComponent<Image>().sprite.name;
        int topIdx = top.IndexOf("_");
        top = top.Substring(0, topIdx);
        string bottom = currentImage[6].GetComponent<Image>().sprite.name;
        int bottomIdx = bottom.IndexOf("_");
        bottom = bottom.Substring(0, bottomIdx);
        playerCannon = cannon;
        playerTankTop = top;
        playerTankBottom = bottom;
        mapName = mapNameText.text;
        PlayerPrefs.SetString("PlayerCannon", playerCannon);
        PlayerPrefs.SetString("PlayerTankTop", playerTankTop);
        PlayerPrefs.SetString("PlayerTankBottom", playerTankBottom);
        StartCoroutine(GoToGameScene());
    }
    IEnumerator GoToGameScene()
    {
        StartCoroutine(FadeInOut.Instance.FadeIn());
        yield return new WaitForSeconds(FadeInOut.Instance.fadeDuration);
        SceneManager.LoadSceneAsync(mapNameText.text);
    }
    void ReadyGame()
    {
        photonView.RPC("RefreshReadiedPlayer", RpcTarget.All, 1, playerOrder);
        gameReady.gameObject.SetActive(false);
        gameReadyCancel.gameObject.SetActive(true);
        readyPanel.SetActive(true);
    }
    void ReadyCancelGame()
    {
        photonView.RPC("RefreshReadiedPlayer", RpcTarget.All, -1, playerOrder);
        gameReady.gameObject.SetActive(true);
        gameReadyCancel.gameObject.SetActive(false);
        readyPanel.SetActive(false);
    }
    public void QuitGame()
    {
        StartCoroutine(Quit());
    }
    IEnumerator Quit()
    {
        yield return StartCoroutine(FadeInOut.Instance.FadeIn());
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        roomUI.gameObject.SetActive(false);
        readyPanel.SetActive(false);
        isSetButtons = false;
    }
    void AddStat(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.doubleShot:
                if (restStatsCount > 0 && itemsCount[1] < doubleShotCounts.childCount)
                {
                    doubleShotCounts.GetChild(itemsCount[1]++).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/UI/StatFilled");
                    restStats.text = "Rest Stats : " + --restStatsCount;
                }
                break;
            case ItemType.shield:
                if (restStatsCount > 0 && itemsCount[2] < shieldCounts.childCount)
                {
                    shieldCounts.GetChild(itemsCount[2]++).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/UI/StatFilled");
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
                if (restStatsCount < restStatsMaxCount && itemsCount[1] > 0)
                {
                    doubleShotCounts.GetChild(--itemsCount[1]).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/UI/StatUnFilled");
                    restStats.text = "Rest Stats : " + ++restStatsCount;
                }
                break;
            case ItemType.shield:
                if (restStatsCount < restStatsMaxCount && itemsCount[2] > 0)
                {
                    shieldCounts.GetChild(--itemsCount[2]).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/UI/StatUnFilled");
                    restStats.text = "Rest Stats : " + ++restStatsCount;
                }
                break;
            default:
                break;
        }
    }
    [PunRPC]
    public void RefreshReadiedPlayer(int plusMinus, int readiedPlayerOrder)
    {
        readiedPlayerCount += plusMinus;
        if (plusMinus > 0)
        {
            content.GetChild(readiedPlayerOrder).GetChild(3).GetChild(0).gameObject.SetActive(true);
        }
        else if (readiedPlayerOrder > 0)
        {
            content.GetChild(readiedPlayerOrder).GetChild(3).GetChild(0).gameObject.SetActive(false);
        }
    }
    // OpenWeaponOptions 함수: 무기 옵션 메뉴 열기
    void WeaponOptions()
    {
        if (weaponOptions.gameObject.activeSelf)
        {
            weaponOptions.gameObject.SetActive(false);
        }
        else if (playerController.myTurn)
        {
            weaponOptions.gameObject.SetActive(true);
            itemOptions.gameObject.SetActive(false);
        }
    }

    void ItemOptions()
    {
        if (itemOptions.gameObject.activeSelf)
        {
            itemOptions.gameObject.SetActive(false);
        }
        else if (playerController.myTurn)
        {
            itemOptions.gameObject.SetActive(true);
            weaponOptions.gameObject.SetActive(false);
        }
    }

    // SetWeapon 함수: 선택한 무기 설정
    void SetWeapon(int _count)
    {
        selectedWeaponType = (SelectedWeaponType)_count;
        selectedWeaponIcon.sprite = weaponOptions.GetChild(_count).GetChild(2).GetComponent<Image>().sprite;
        selectedWeaponName.text = weaponOptions.GetChild(_count).GetChild(0).GetComponent<TMP_Text>().text;
    }
    void SetItem(int _count)
    {
        selectedItemType = (SelectedItemType)_count;
        if (_count != 0)
        {
            selectedItemIcon.sprite = itemOptions.GetChild(_count).GetChild(2).GetComponent<Image>().sprite;
            selectedItemName.text = itemOptions.GetChild(_count).GetChild(0).GetComponent<TMP_Text>().text;
        }
        else
        {
            selectedItemIcon.sprite = itemOptions.GetChild(0).GetChild(2).GetComponent<Image>().sprite;
            selectedItemName.text = "None";
        }
    }
    // DecreaseWeaponCount 함수: 무기 개수 감소
    public void DecreaseWeaponCount(int weaponType)
    {
        if (weaponType != 0)
        {
            int currentWeaponCount = int.Parse(weapons[weaponType].transform.GetChild(1).GetComponent<TMP_Text>().text);
            if (currentWeaponCount - 1 == 0)
            {
                weapons[weaponType].gameObject.SetActive(false);
                SetWeapon(0);
            }
            weapons[weaponType].transform.GetChild(1).GetComponent<TMP_Text>().text = (--currentWeaponCount).ToString();
        }
    }
    public void DecreaseItemCount(int itemType)
    {
        if (itemType != 0)
        {
            int currentItemCount = int.Parse(items[itemType].transform.GetChild(1).GetComponent<TMP_Text>().text);
            if (currentItemCount - 1 == 0)
            {
                items[itemType].gameObject.SetActive(false);
                SetItem(0);
            }
            if (itemType == 2 || !playerController.shield.activeSelf)
            {
                items[itemType].transform.GetChild(1).GetComponent<TMP_Text>().text = (--currentItemCount).ToString();
                selectedItemIcon.sprite = itemOptions.GetChild(0).GetChild(2).GetComponent<Image>().sprite;
                selectedItemName.text = "None";
            }
        }
    }
    // PreviousImage 함수: 이전 이미지로 변경
    public void SetPlayerHp(int currentHp, GameObject gameObject)
    {
        if (gameObject == tank)
        {
            hpbar.value = currentHp;
            hp.text = $"{currentHp}/{playerController.maxHp}";
        }
    }
    public void PhotonPreviousImage(List<string> imageNames, int _currentImageIndex, int changingImageIndex, int type)
    {
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
        mapNameText.text = mapNames[_currentImageIndex];
        mapMaxPlayerText.text = "MaxPlayers : " + mapMaxPlayerCounts[_currentImageIndex];
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        StartCoroutine(FadeInOut.Instance.FadeOut());
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
        StartCoroutine(FadeInOut.Instance.FadeIn());
    }

    // JoinRoom 함수: 방 입장
    public void JoinRoom()
    {
        insertRooomKey.gameObject.SetActive(true);
    }

    // JoinRandomRoom 함수: 랜덤 방 입장
    public void JoinRandomRoom()
    {
        StartCoroutine(FadeInOut.Instance.FadeIn());
        PhotonNetwork.JoinRandomRoom();
    }
    // EnterJoinRoom 함수: 방 입장 확인
    public void EnterJoinRoom()
    {
        if (roomNames.Contains(roomKey.text))
        {
            PhotonNetwork.JoinRoom(roomKey.text);
            StartCoroutine(FadeInOut.Instance.FadeIn());
        }
        else
        {
            errorMessage.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(1280, 255);
            StartCoroutine(ErrorMessage("This Key Is Not Exist"));
        }
    }
    public override void OnLeftRoom()
    {
        // Destroy(gameObject);
    }
    // InsertRoomKeyCancel 함수: 방 키 입력 취소
    public void InsertRoomKeyCancel()
    {
        insertRooomKey.gameObject.SetActive(false);
        roomKey.text = "";
    }

    // WrongKeyMessage 함수: 잘못된 방 키 메시지 표시
    IEnumerator ErrorMessage(string error)
    {
        errorMessage.SetActive(true);
        errorText.text = error;
        yield return new WaitForSeconds(1.0f);
        errorMessage.SetActive(false);
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
    IEnumerator RoomUIActivate()
    {
        yield return new WaitForSeconds(FadeInOut.Instance.fadeDuration);
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

        gameReadyCancel.gameObject.SetActive(false);
        playerActorNumbers.Add(PhotonNetwork.LocalPlayer.ActorNumber);
        photonView.RPC("RefreshPlayerInfo", RpcTarget.AllViaServer);
        yield return new WaitForSeconds(FadeInOut.Instance.fadeDuration);
        StartCoroutine(FadeInOut.Instance.FadeOut());
    }
    public override void OnJoinedRoom()
    {
        StartCoroutine(RoomUIActivate());
    }
    public bool isSetButtons;
    public int playerOrder;
    public void TurnEnd()
    {
        selectedItemType = SelectedItemType.None;
        if (!playerController.gameEnd)
        {
            int randomValue = Random.Range(0, 1000);
            if (randomValue < 100)
            {
                randomValue = Random.Range(-10, 10 + 1);
            }
            else if (randomValue < 200)
            {
                randomValue = Random.Range(-15, 15 + 1);
            }
            else if (randomValue < 500)
            {
                randomValue = Random.Range(-30, 30 + 1);
            }
            else if (randomValue < 700)
            {
                randomValue = Random.Range(-35, 35 + 1);
            }
            else if (randomValue < 950)
            {
                randomValue = Random.Range(-40, 40 + 1);
            }
            else if (randomValue < 980)
            {
                randomValue = Random.Range(-45, 45 + 1);
            }
            else if (randomValue < 995)
            {
                randomValue = Random.Range(-50, 50 + 1);
            }
            else
            {
                randomValue = Random.Range(-60, 60 + 1);
            }
            photonView.RPC("WindChange", RpcTarget.All, randomValue);
            photonView.RPC("NextTurn", RpcTarget.All, playerOrder + 1);
        }
    }
    [PunRPC]
    public void WindChange(int randomValue)
    {
        wind.windPower = (float)randomValue / 10;
        if (randomValue > 0)
        {
            windDirectionImage.sprite = Resources.LoadAll<Sprite>("Images/UI/windDirection")[1];
        }
        else if (randomValue < 0)
        {
            windDirectionImage.sprite = Resources.LoadAll<Sprite>("Images/UI/windDirection")[2];
        }
        else
        {
            windDirectionImage.sprite = Resources.LoadAll<Sprite>("Images/UI/windDirection")[0];
        }
        windPowerText.text = randomValue.ToString();
    }
    [PunRPC]
    public void NextTurn(int currentTurn)
    {
        StartCoroutine(Turn(currentTurn));
    }
    public GameObject turnPanel;
    IEnumerator Turn(int currentTurn)
    {
        if (currentTurn == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            if (playerOrder == 0)
            {
                turnPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = "나의 턴";
                turnPanel.SetActive(true);
                yield return new WaitForSeconds(1.0f);
                playerController.myTurn = true;
            }
            else
            {
                turnPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{currentTurn - 1}번째 플레이어턴";
                turnPanel.SetActive(true);
                yield return new WaitForSeconds(1.0f);
                playerController.myTurn = false;
            }
        }
        else if (currentTurn == playerOrder)
        {
            turnPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = "나의 턴";
            turnPanel.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            playerController.myTurn = true;
        }
        else
        {
            turnPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{currentTurn + 1}번째 플레이어턴";
            turnPanel.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            playerController.myTurn = false;
        }
        turnPanel.SetActive(false);
    }
    public void SetPhotonButtons()
    {
        playerOrder = content.childCount - 1;
        if (playerOrder != 0)
        {
            photonView.RPC("RefreshReadiedPlayer", RpcTarget.All, 0, playerOrder);
        }
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
        if (playerOrder > 0)
        {
            Debug.Log("!1");
            buttons.gameObject.SetActive(false);
            gameStart.gameObject.SetActive(false);
            gameReady.gameObject.SetActive(true);
        }
        else
        {
            currentImage[3].sprite = Resources.Load<Sprite>("images/Backgrounds/" + mapNames[0]);
            mapName = mapNames[0];
            mapMaxPlayerText.text = $"MaxPlayers : {mapMaxPlayerCounts[0]}";
            gameStart.gameObject.SetActive(true);
            gameReady.gameObject.SetActive(false);
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
        switch (currentScene)
        {
            case CurrentScene.Ready:
                Debug.Log("left");
                for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount + 1; ++i)
                {
                    if (playerActorNumbers[i] == otherPlayer.ActorNumber)
                    {
                        PhotonNetwork.Destroy(content.GetChild(i).gameObject);
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
                    Debug.Log("2");
                    playerOrder = 0;
                    if (gameReadyCancel.gameObject.activeSelf)
                    {
                        photonView.RPC("RefreshReadiedPlayer", RpcTarget.All, -1, -1);
                        readyPanel.SetActive(false);
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
                    buttons.gameObject.SetActive(true);
                }
                break;
            case CurrentScene.Game:
                if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    playerController.GameEnd();
                }
                break;
        }
    }
    public List<int> playerActorNumbers;
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerActorNumbers.Add(newPlayer.ActorNumber);
        if (photonView.IsMine)
        {
            Debug.Log("3");
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
            int mapMaxPlayersIdx = mapNames.IndexOf(mapNameText.text);
            photonView.RPC("SetMapImage", newPlayer, mapNameText.text, mapMaxPlayersIdx);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isSetButtons = false;
        SceneManager.LoadSceneAsync("LobbyScene");
        // PhotonNetwork.Destroy(tank);
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
    public void SetMapImage(string mapName, int _mapMaxPlayersIdx)
    {
        currentImage[3].sprite = Resources.Load<Sprite>("Images/Backgrounds/" + mapName);
        mapNameText.text = mapName;
        mapMaxPlayerText.text = "MaxPlayers : " + mapMaxPlayerCounts[_mapMaxPlayersIdx];
    }
    [PunRPC]
    public void RefreshPlayerInfo()
    {
        for (int i = 0; i < content.childCount; ++i)
        {
            if (content.GetChild(i).GetComponent<PlayerInfomation>().playerOrder)
            {
                content.GetChild(i).GetComponent<PlayerInfomation>().playerOrder.text = (i + 1).ToString();
                content.GetChild(i).GetComponent<PlayerInfomation>().playerName.text = (i + 1).ToString();
            }
        }
    }
    IEnumerator FadeInAndMoveLobby()
    {
        yield return StartCoroutine(FadeInOut.Instance.FadeIn());
        SceneManager.LoadSceneAsync("LobbyScene");
    }
    IEnumerator WaitFadeOut()
    {
        yield return StartCoroutine(FadeInOut.Instance.FadeIn());
        SceneManager.LoadSceneAsync("LobbyScene");
    }
    public bool goReady;
    void Update()
    {
        switch (currentScene)
        {
            case CurrentScene.None:
                if (Input.GetKeyDown(KeyCode.LeftControl) && !goReady)
                {
                    goReady = true;
                    StartCoroutine(FadeInAndMoveLobby());
                }
                break;
            case CurrentScene.Ready:
                // ConnectionStatus.text = PhotonNetwork.NetworkClientState.ToString();
                if (roomUI.gameObject.activeSelf && content.childCount > 0)
                {
                    if (content.childCount != int.Parse(content.GetChild(content.childCount - 1).GetComponent<PlayerInfomation>().playerOrder.text))
                    {
                        photonView.RPC("RefreshPlayerInfo", RpcTarget.AllViaServer);
                    }
                    if (content.childCount == int.Parse(content.GetChild(content.childCount - 1).GetComponent<PlayerInfomation>().playerOrder.text) && !isSetButtons && PhotonNetwork.InRoom)
                    {
                        isSetButtons = false;
                        SetPhotonButtons();
                    }
                    if (photonView.IsMine)
                    {
                        if (!content.GetChild(0).GetChild(3).GetChild(0).gameObject.activeSelf)
                        {
                            content.GetChild(0).GetChild(3).GetChild(0).gameObject.SetActive(true);
                        }
                    }
                }
                break;
            case CurrentScene.Game:
                if (niddle.transform.eulerAngles.z > 90)
                {
                    tank.GetComponent<PlayerController>().canMove = false;
                }
                if (playerController.gameEnd)
                {
                    if (PhotonNetwork.InRoom)
                    {
                        if (tank.GetComponent<PlayerController>().hpBar.value <= 0)
                        {
                            loseButton.SetActive(true);
                            loseButton.GetComponent<Button>().onClick.AddListener(NextGame);
                        }
                        else
                        {
                            winButton.SetActive(true);
                            winButton.GetComponent<Button>().onClick.AddListener(NextGame);
                        }
                        playerActorNumbers = new List<int>();
                        for (int i = 1; i < itemsCount.Count; ++i)
                        {
                            itemsCount[i] = 0;
                        }
                        // PhotonNetwork.CurrentRoom.IsOpen = false;
                        // PhotonNetwork.CurrentRoom.IsVisible = false;

                        roomNames.Remove(PhotonNetwork.CurrentRoom.Name);
                        Debug.Log(playerController.gameEnd);
                    }
                }
                if (playerOrder == 0 && !photonView.IsMine)
                {
                    photonView.IsMine = true;
                }
                if (playerOrder != 0 && photonView.IsMine)
                {
                    photonView.IsMine = false;
                }
                break;
            default:
                break;
        }
    }
    public bool pressNextGame;
    public void NextGame()
    {
        if (!pressNextGame)
        {
            isSetButtons = false;
            pressNextGame = true;
            StartCoroutine(WaitFadeOut());
        }
    }
}