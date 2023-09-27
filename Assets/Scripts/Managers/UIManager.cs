using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance;  // UIManager의 인스턴스를 관리합니다.

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

    // Ready 장면 UI 요소
    public Transform roomUI;  // 방 관련 UI
    public Transform lobby;  // 로비
    public Transform players;  // 플레이어 목록
    public Transform content;
    public Transform maps;  // 맵 목록
    public Transform infomationBG;
    public Image mapImage;  // 맵 이미지
    public Transform custom;  // 탱크 커스터마이징
    public Transform stats;  // 통계
    public int roomNameLength;  // 방 이름 길이
    public Image cannonImage;  // 탱크 포 이미지
    public Image playerCannonImage;  // 플래이어창 탱크 포 이미지
    public Image tankTopImage;  // 탱크 상단 이미지
    public Image playerTankTopImage;  // 플래이어창 탱크 상단 이미지
    public Image tankBottomImage;  // 탱크 하단 이미지
    public Transform buttons;
    public Image playerTankBottomImage;  // 플래이어창 탱크 하단 이미지
    public Button previousMapButton;  // 이전 맵 버튼
    public Button nextMapButton;  // 다음 맵 버튼
    public Button previousCannonButton;  // 이전 포 이미지 버튼
    public Button nextCannonButton;  // 다음 포 이미지 버튼
    public Button previousTankTopButton;  // 이전 상단 이미지 버튼
    public Button nextTankTopButton;  // 다음 상단 이미지 버튼
    public Button previousTankBottomButton;  // 이전 하단 이미지 버튼
    public Button nextTankBottomButton;  // 다음 하단 이미지 버튼
    public TMP_Text mapName;  // 맵 이름 텍스트
    public TMP_Text inviteKey;
    public TMP_Text mapMaxPlayerText;  // 맵 최대 플레이어 수 텍스트

    // 맵과 탱크 커스터마이징을 위한 리스트들
    public int currentMapIndex;
    public List<string> mapNames;  // 맵 이름 리스트
    public List<string> tankNames;  // 탱크 이름 리스트
    public List<int> mapMaxPlayerCounts;  // 맵 최대 플레이어 수 리스트
    public List<int> currentTankImageIndex;  // 현재 탱크 이미지 인덱스 리스트

    // Room Lobby를 위한 UI 요소들
    public Button makeRoomButton;  // 방 만들기 버튼
    public Button joinRoomButton;  // 방 입장 버튼
    public Button joinRandomRoomButton;  // 랜덤 방 입장 버튼
    public Transform insertRooomKey;  // 방 키 입력 창
    public Button enterJoinRoomButton;  // 방 입장 확인 버튼
    public Button cancelJoinRoomButton;  // 방 입장 취소 버튼
    public TMP_InputField roomKey;  // 방 키 입력 필드
    public TMP_Text ConnectionStatus;  // 연결 상태 텍스트

    // 기타 변수들
    public GameObject wrongRoomKeyMessage;  // 잘못된 방 키 메시지
    public List<string> roomNames;  // 방 이름 리스트

    new void OnEnable()
    {
        switch (currentScene)
        {
            case CurrentScene.Ready:
                SceneManager.sceneLoaded += OnSceneLoaded;
                break;
            case CurrentScene.Game:
                SceneManager.sceneLoaded += OnSceneLoaded;
                break;
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            InitializeGameSceneUI();
            currentScene = CurrentScene.Game;
            Debug.Log(1);
        }
        else if (scene.buildIndex == 0)
        {
            InitializeReadySceneUI();
            currentScene = CurrentScene.Ready;
            Debug.Log(0);
        }
    }
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

    // Start 함수: 게임 오브젝트가 활성화된 후에 호출
    void Start()
    {
        // 현재 장면에 따라 초기화 함수를 호출합니다.
        // switch (currentScene)
        // {
        //     case CurrentScene.Ready:
        //         InitializeReadySceneUI();
        //         break;
        //     case CurrentScene.Game:
        //         InitializeGameSceneUI();
        //         break;
        // }
    }

    // Ready 장면 UI 초기화 함수
    void InitializeReadySceneUI()
    {
        PhotonNetwork.ConnectUsingSettings();

        gameStart = GameObject.Find("GoGame").GetComponent<Button>();
        gameStart.onClick.AddListener(() => GoGame());


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
        mapImage = maps.GetComponent<Image>();
        infomationBG = maps.Find("InfomationBG");
        buttons = maps.Find("Buttons");
        mapName = infomationBG.Find("MapName").GetComponent<TMP_Text>();
        mapMaxPlayerText = infomationBG.Find("MaxPlayerCount").GetComponent<TMP_Text>();
        inviteKey = infomationBG.Find("InviteKey").GetComponent<TMP_Text>();
        previousMapButton = buttons.Find("Previous").GetComponent<Button>();
        previousMapButton.onClick.AddListener(() => PreviousMapImage());
        nextMapButton = buttons.Find("Next").GetComponent<Button>();
        nextMapButton.onClick.AddListener(() => NextMapImage());


        custom = roomUI.Find("Custom");
        cannonImage = custom.Find("TankCannonImage").GetComponent<Image>();

        // playerCannonImage = 
        previousCannonButton = cannonImage.transform.Find("Previous").GetComponent<Button>();
        previousCannonButton.onClick.AddListener(() => PreviousImage(currentTankImageIndex[0], tankNames, cannonImage, 0));
        nextCannonButton = cannonImage.transform.Find("Next").GetComponent<Button>();
        nextCannonButton.onClick.AddListener(() => NextImage(currentTankImageIndex[0], tankNames, cannonImage, 0));
        tankTopImage = custom.Find("TankTopImage").GetComponent<Image>();
        // playerTankTopImage =
        previousTankTopButton = tankTopImage.transform.Find("Previous").GetComponent<Button>();
        previousTankTopButton.onClick.AddListener(() => PreviousImage(currentTankImageIndex[1], tankNames, tankTopImage, 1));
        nextTankTopButton = tankTopImage.transform.Find("Next").GetComponent<Button>();
        nextTankTopButton.onClick.AddListener(() => NextImage(currentTankImageIndex[1], tankNames, tankTopImage, 1));
        tankBottomImage = custom.Find("TankBottomImage").GetComponent<Image>();
        // playerTankBottomImage =
        previousTankBottomButton = tankBottomImage.transform.Find("Previous").GetComponent<Button>();
        previousTankBottomButton.onClick.AddListener(() => PreviousImage(currentTankImageIndex[2], tankNames, tankBottomImage, 2));
        nextTankBottomButton = tankBottomImage.transform.Find("Next").GetComponent<Button>();
        nextTankBottomButton.onClick.AddListener(() => NextImage(currentTankImageIndex[2], tankNames, tankBottomImage, 2));
        stats = roomUI.Find("Stats");
        roomUI.gameObject.SetActive(false);

    }

    // Game 장면 UI 초기화 함수
    void InitializeGameSceneUI()
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

    // Update 함수: 매 프레임마다 호출
    void Update()
    {
        switch (currentScene)
        {
            case CurrentScene.Ready:
                ConnectionStatus.text = PhotonNetwork.NetworkClientState.ToString();
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    lobby.gameObject.SetActive(true);
                    makeRoomButton.gameObject.SetActive(true);
                    joinRoomButton.gameObject.SetActive(true);
                    joinRandomRoomButton.gameObject.SetActive(true);
                }
                // RefreshPlayerInfo();
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

    public void RefreshPlayerInfo()
    {
        Debug.Log(content.childCount);
        for (int i = 0; i < content.childCount; ++i)
        {
            content.GetChild(i).GetComponent<PlayerInfomation>().playerOrder.text = (i + 1).ToString();
            content.GetChild(i).GetComponent<PlayerInfomation>().playerName.text = (i + 1).ToString();
        }
    }
    // GoGame 함수: 게임 장면으로 이동
    void GoGame()
    {
        SceneManager.LoadSceneAsync(1);
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
    public void PreviousImage(int currentIndex, List<string> names, Image changingImage, int type)
    {
        if (currentIndex - 1 < 0)
        {
            currentIndex = names.Count - 1;
        }
        else
        {
            --currentIndex;
        }
        currentTankImageIndex[type] = currentIndex;
        ChangeImage(changingImage, "Tanks/", names, currentIndex, type);
    }

    // NextImage 함수: 다음 이미지로 변경
    public void NextImage(int currentIndex, List<string> names, Image changingImage, int type)
    {
        if (currentIndex + 1 > names.Count - 1)
        {
            currentIndex = 0;
        }
        else
        {
            ++currentIndex;
        }
        currentTankImageIndex[type] = currentIndex;
        ChangeImage(changingImage, "Tanks/", names, currentIndex, type);
    }

    // PreviousMapImage 함수: 이전 맵 이미지 표시
    public void PreviousMapImage()
    {
        if (currentMapIndex - 1 < 0)
        {
            currentMapIndex = mapNames.Count - 1;
        }
        else
        {
            --currentMapIndex;
        }
        MapSetting();
    }

    // NextMapImage 함수: 다음 맵 이미지 표시
    public void NextMapImage()
    {
        if (currentMapIndex + 1 > mapNames.Count - 1)
        {
            currentMapIndex = 0;
        }
        else
        {
            ++currentMapIndex;
        }
        MapSetting();
    }

    // MapSetting 함수: 맵 설정
    public void MapSetting()
    {
        ChangeImage(mapImage, "Backgrounds/", mapNames, currentMapIndex, 0);
        mapName.text = mapNames[currentMapIndex];
        mapMaxPlayerText.text = "MaxPlayers : " + mapMaxPlayerCounts[currentMapIndex];
    }

    // ChangeImage 함수: 이미지 변경
    public void ChangeImage(Image _changingImage, string path, List<string> names, int index, int _type)
    {
        _changingImage.sprite = Resources.LoadAll<Sprite>("Images/" + path + names[index])[_type];
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
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MakeRoom();
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
}
