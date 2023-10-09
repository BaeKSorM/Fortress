using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    public static PlayerController Instance;
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public Transform shootPositon;
    public GameObject cannon;
    public GameObject chargingGauge;
    public Slider chargingGaugeBar;

    public float movementSpeed;
    public float moveAddAngle;
    public float groundCheckRadius;
    public float jumpForce;
    public float slopeCheckDistance;
    public float maxSlopeAngle;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public float xInput;
    public float slopeDownAngle;
    public float slopeSideAngle;
    public float lastSlopeAngle;
    public float shootPowerMax;
    public float framePerAddingValue;
    public int facingDirection = 1;
    public bool isGrounded;
    public bool isOnSlope;
    public bool isJumping;
    public bool isWalkOnSlope;
    public bool isJump;
    public bool isShoot;
    public bool isMove;
    public bool isRotatecannon;
    public bool isIncrease;
    public bool isAction;
    public float chargingMax;
    public float groundAngle;
    public bool canMove;
    public bool myTurn;
    public Slider hpBar;
    private Vector2 newVelocity;
    private Vector2 newForce;
    private Vector2 slopeNormalPerp;
    private new Rigidbody2D rigidbody;
    private CapsuleCollider2D capsuleCollider;
    public float setScale;
    public float threeBallAngle;
    public int lookPosition = 1;
    public PhotonView PV;
    public void Awake()
    {
    }
    void Start()
    {
        // if (UIManager.Instance.currentScene == UIManager.CurrentScene.Ready)
        // {
        //     gameObject.SetActive(false);
        // }
        PV = GetComponent<PhotonView>();
        Instance = this;
        rigidbody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        cannon = GameObject.Find("Cannon");
        chargingGaugeBar = chargingGauge.GetComponent<Slider>();
        chargingGaugeBar.maxValue = shootPowerMax;
        chargingMax = chargingGaugeBar.maxValue;
        setScale = transform.localScale.x > 0 ? transform.localScale.x : -transform.localScale.x;
        rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
        // if (UIManager.Instance.playerOrder == 0)
        // {
        //     myTurn = true;
        // }
        // else
        // {
        //     myTurn = false;
        // }
    }

    void Update()
    {
        if (!PV.IsMine)
            return;
        Turn();
        if (Input.anyKey)
        {
            PressingKey();
        }
        CannonMove();
        RotateFloorAngle();
        Charging();
    }
    public void Charging()
    {
        if (myTurn && Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
            isMove = false;
            isRotatecannon = false;
            isShoot = true;
            isAction = true;
            ChargingShootPower();
        }
        if (Input.GetKeyUp(KeyCode.Space) && isShoot)
        {
            int weaponType = (int)UIManager.Instance.selectedWeaponType;
            int itemType = (int)UIManager.Instance.selectedItemType;
            Debug.Log(gameObject.name);
            Shoot(weaponType, itemType);
            Shooted();
        }
    }
    public void SelectAction()
    {
        if (myTurn)
        {
            if (canMove && !isShoot)
            {
                if (Input.GetKey(leftKey))
                {
                    rigidbody.constraints = RigidbodyConstraints2D.None;
                    xInput = -1;
                    lookPosition = -1;
                    UIManager.Instance.niddle.transform.eulerAngles -= new Vector3(0, 0, moveAddAngle * Time.deltaTime);
                }
                else if (Input.GetKey(rightKey))
                {
                    rigidbody.constraints = RigidbodyConstraints2D.None;
                    xInput = 1;
                    lookPosition = 1;
                    UIManager.Instance.niddle.transform.eulerAngles -= new Vector3(0, 0, moveAddAngle * Time.deltaTime);
                }
                else //if (Input.GetKeyUp(leftKey) || Input.GetKeyUp(rightKey))
                {
                    xInput = 0;
                    lookPosition = 0;
                    rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
                }
            }
            else
            {
                xInput = 0;
                rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
            }

            if (!isAction)
            {
                if (xInput != 0)
                {
                    isMove = true;
                    isRotatecannon = false;
                    isShoot = false;
                }
                else if ((Input.GetKey(upKey) || Input.GetKey(downKey)) && xInput == 0 && !isRotatecannon)
                {
                    isRotatecannon = true;
                    isMove = false;
                    isShoot = false;
                }
            }
        }
    }

    public void Shooted()
    {
        isMove = true;
        isRotatecannon = true;
        isShoot = false;
        isAction = false;
        chargingGauge.SetActive(false);
        rigidbody.constraints = RigidbodyConstraints2D.None;
    }

    public void ChargingShootPower()
    {
        if (isShoot)
        {
            chargingGauge.SetActive(true);
            chargingGaugeBar.value = 0;
        }
    }

    private void LaunchCharging()
    {
        if (isShoot)
        {
            if (isIncrease)
            {
                chargingGaugeBar.value += framePerAddingValue;
                if (chargingGaugeBar.value == chargingMax)
                {
                    isIncrease = false;
                    int weaponType = (int)UIManager.Instance.selectedWeaponType;
                    int itemType = (int)UIManager.Instance.selectedItemType;
                    Debug.Log(gameObject.name);
                    Shoot(weaponType, itemType);
                    Shooted();
                }
            }
            else
            {
                isIncrease = true;
            }
        }
    }

    public void Turn()
    {
        if (lookPosition == 1 && facingDirection == -1)
        {
            Flip();
        }
        else if (lookPosition == -1 && facingDirection == 1)
        {
            Flip();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Jump();
        }
    }

    public void PressingKey()
    {
        if (Input.GetKey(KeyCode.Space) && isShoot)
        {
            LaunchCharging();
        }
    }

    public void CannonMove()
    {
        if (myTurn && isRotatecannon)
        {
            int directionCheck = (int)transform.localScale.x;
            if (Input.GetKey(upKey))
            {
                CannonUp(directionCheck);
            }
            if (Input.GetKey(downKey))
            {
                CannonDown(directionCheck);
            }
        }
    }
    public bool isNearGrounded;
    private void RotateFloorAngle()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, whatIsGround);
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, 0.4f, whatIsGround);
        isNearGrounded = hitGround.collider != null;
        isGrounded = hit.collider != null;

        if (isGrounded)
        {
            groundAngle = Vector2.Angle(Vector2.up, hit.normal);
            groundAngle *= hit.normal.x < 0 ? 1 : -1;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, groundAngle));
        }
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;
        CheckGround();
        SlopeCheck();
        ApplyMovement();
        SelectAction();
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        if (rigidbody.velocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if (isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            isJump = true;
        }
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, capsuleCollider.size.y / 2));
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void ApplyMovement()
    {
        if (isGrounded && !isOnSlope && !isJumping)
        {
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rigidbody.velocity = newVelocity;
        }
        else if (isGrounded && isOnSlope && isWalkOnSlope && !isJumping)
        {
            newVelocity.Set(movementSpeed * slopeNormalPerp.x * -xInput, movementSpeed * slopeNormalPerp.y * -xInput);
            rigidbody.velocity = newVelocity;
        }
        else if (!isGrounded)
        {
            newVelocity.Set(movementSpeed * xInput, rigidbody.velocity.y);
            rigidbody.velocity = newVelocity;
        }
        if (isNearGrounded)
        {
            rigidbody.gravityScale = 2;
        }
        else
        {
            rigidbody.gravityScale = 5;
        }
    }

    public List<string> weaponNames = new List<string>() { "Shot", "Three-Ball", "One-Bounce", "Roller", "Back-Roller", "Granade", "Spliter", "Breaker", "Sniper" };
    public void Shoot(int weaponType, int itemType)
    {
        switch (weaponType)
        {
            case 0:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                ShootType(weaponType);
                break;
            case 1:
                ShootType(weaponType, threeBallAngle);
                break;
            default:
                break;
        }
        // Debug.Log(((int)selectedWeaponType));
        UIManager.Instance.DecreaseWeaponCount(weaponType);
        UIManager.Instance.DecreaseItemCount(itemType);
    }
    public GameObject weaponClone;
    public TMP_Text d;
    public void ShootType(int weaponNumber)
    {
        float chargingValue = chargingGaugeBar.value;

        // 게임 오브젝트 생성 및 로컬 플레이어에 의한 조작
        Vector2 force = (shootPositon.position - cannon.transform.position) * chargingValue;
        photonView.RPC("NormalShoot", RpcTarget.All, force, weaponNumber);
    }
    [PunRPC]
    public void NormalShoot(Vector2 force, int weaponNumber)
    {
        weaponClone = Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/" + weaponNames[weaponNumber]), shootPositon.position, Quaternion.identity);
        weaponClone.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
    }
    public void ShootType(int weaponNumber, float angle)
    {
        float addingAngle = angle;
        angle = -angle;
        for (int i = 0; i < 3; ++i)
        {
            float chargingValue = chargingGaugeBar.value;
            Vector2 force = (shootPositon.position - cannon.transform.position) * chargingValue;
            photonView.RPC("ThreeShoot", RpcTarget.All, angle, force, weaponNumber);
            angle += addingAngle;
        }
    }
    [PunRPC]
    public void ThreeShoot(float angle, Vector2 force, int weaponNumber)
    {
        weaponClone = Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/" + weaponNames[weaponNumber]), shootPositon.position, Quaternion.identity);
        weaponClone.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        weaponClone.GetComponent<Rigidbody2D>().AddForce(Vector2.up * angle, ForceMode2D.Impulse);
    }
    public void CannonUp(int _directionCheck)
    {
        cannon.transform.eulerAngles += new Vector3(0, 0, .1f * _directionCheck);
    }

    public void CannonDown(int _directionCheck)
    {
        cannon.transform.eulerAngles += new Vector3(0, 0, -.1f * _directionCheck);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            lastSlopeAngle = slopeDownAngle;
        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            isWalkOnSlope = false;
        }
        else
        {
            isWalkOnSlope = true;
        }
    }

    public void Jump()
    {
        if (isJump)
        {
            isJump = false;
            isJumping = true;
            newVelocity.Set(0.0f, 0.0f);
            rigidbody.velocity = newVelocity;
            newForce.Set(0.0f, jumpForce);
            rigidbody.AddForce(newForce, ForceMode2D.Impulse);
        }
    }

    public void Flip()
    {
        // facingDirection *= -1;
        // transform.localScale = new Vector3(facingDirection * setScale, 1, 1);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Explosion"))
        {
            hpBar.value -= other.GetComponentInParent<Weapons>().damage;
            if (hpBar.value <= 0)
            {
                PV.RPC("GameEnd", RpcTarget.All);
            }
        }
    }
    [PunRPC]
    public void GameEnd()
    {
        if (UIManager.Instance.tank.GetComponent<PlayerController>().hpBar.value <= 0)
        {
            Lose();
        }
        else
        {
            Win();
        }
    }
    public void Lose()
    {
        UIManager.Instance.loseButton.SetActive(true);
        UIManager.Instance.loseButton.GetComponent<Button>().onClick.AddListener(NextGame);
    }
    public void Win()
    {
        UIManager.Instance.winButton.SetActive(true);
        UIManager.Instance.winButton.GetComponent<Button>().onClick.AddListener(NextGame);
    }
    public int lobbyScene = 0;
    public void NextGame()
    {
        // Destroy(gameObject);
        SceneManager.LoadSceneAsync("StartScene");
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            rigidbody.velocity = Vector2.zero;
        }
    }
}
