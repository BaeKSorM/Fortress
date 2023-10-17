using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    public int cannonMoveSpeed;
    public int maxHp;
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
        shield = transform.GetChild(2).gameObject;
        hpBar = transform.GetChild(0).GetChild(0).GetComponent<Slider>();
        hpBar.maxValue = maxHp;
        if (UIManager.Instance.playerOrder == 0)
        {
            myTurn = true;
        }
        else
        {
            myTurn = false;
        }
    }
    void Update()
    {
        if (!PV.IsMine)
            return;

        if (Input.anyKey)
        {
            PressingKey();
        }
        Turn();
        CannonMove();
        RotateFloorAngle();
        Charging();
        if (isShooted && weaponClone == null)
        {
            StartCoroutine(TurnEndCheck());
            isShooted = false;
        }
    }
    public bool isShooted;
    IEnumerator TurnEndCheck()
    {
        float timer = 0;
        while (timer < 2.0f)
        {
            timer += 0.01f;
            if (weaponClone != null)
            {
                yield break;
            }
            yield return new WaitForSeconds(0.01f);
        }
        UIManager.Instance.TurnEnd();
    }
    public void Charging()
    {
        if (Input.GetKeyUp(KeyCode.Space) && isShoot)
        {
            int weaponType = (int)UIManager.Instance.selectedWeaponType;
            int itemType = (int)UIManager.Instance.selectedItemType;
            Shoot(weaponType, itemType);
        }
        if (myTurn && Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
            isMove = false;
            isRotatecannon = false;
            isShoot = true;
            isAction = true;
            ChargingShootPower();
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
                    // lookPosition = 0;
                    rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
                }
            }
            else if (!isShoot)
            {
                if (Input.GetKey(leftKey))
                {
                    lookPosition = -1;
                }
                else if (Input.GetKey(rightKey))
                {
                    lookPosition = 1;
                }
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
        myTurn = false;
        chargingGauge.SetActive(false);
        // rigidbody.constraints = RigidbodyConstraints2D.None;
        isShooted = true;
    }

    public void ChargingShootPower()
    {
        if (isShoot)
        {
            chargingGauge.SetActive(true);
            chargingGaugeBar.value = 0;
            UIManager.Instance.weaponOptions.gameObject.SetActive(false);
            UIManager.Instance.itemOptions.gameObject.SetActive(false);
        }
    }

    private void LaunchCharging()
    {
        if (isShoot)
        {
            if (isIncrease)
            {
                chargingGaugeBar.value += framePerAddingValue * Time.deltaTime;
                if (chargingGaugeBar.value == chargingMax)
                {
                    isIncrease = false;
                    int weaponType = (int)UIManager.Instance.selectedWeaponType;
                    int itemType = (int)UIManager.Instance.selectedItemType;
                    Shoot(weaponType, itemType);
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
                ShootType(weaponType, itemType);
                break;
            case 1:
                ShootType(weaponType, itemType, threeBallAngle);
                break;
            default:
                break;
        }
        UIManager.Instance.DecreaseWeaponCount(weaponType);
        UIManager.Instance.DecreaseItemCount(itemType);
    }
    public GameObject weaponClone;
    public TMP_Text d;
    public void ShootType(int _weaponType, int _itemType)
    {
        float chargingValue = chargingGaugeBar.value;

        // 게임 오브젝트 생성 및 로컬 플레이어에 의한 조작
        Vector2 force = (shootPositon.position - cannon.transform.position) * chargingValue;
        switch (_itemType)
        {
            case 1:
                StartCoroutine(DoubleShot(force, _weaponType));
                break;
            case 2:
                PV.RPC("Shield", RpcTarget.All);
                PV.RPC("NormalShoot", RpcTarget.All, force, _weaponType);
                break;
            default:
                PV.RPC("NormalShoot", RpcTarget.All, force, _weaponType);
                break;
        }
    }
    public IEnumerator DoubleShot(Vector2 force, int _weaponType)
    {
        PV.RPC("NormalShoot", RpcTarget.All, force, _weaponType);
        while (weaponClone != null)
        {
            yield return null;
        }
        PV.RPC("NormalShoot", RpcTarget.All, force, _weaponType);
        Shooted();
    }
    [PunRPC]
    public void NormalShoot(Vector2 force, int weaponType)
    {
        weaponClone = Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/" + weaponNames[weaponType]), shootPositon.position, Quaternion.identity);
        weaponClone.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        Shooted();
    }
    public void ShootType(int _weaponType, int _itemType, float angle)
    {
        float chargingValue = chargingGaugeBar.value;
        Vector2 force = (shootPositon.position - cannon.transform.position) * chargingValue;
        switch (_itemType)
        {
            case 1:
                StartCoroutine(DoubleShot(angle, force, _weaponType));
                break;
            case 2:
                PV.RPC("Shield", RpcTarget.All);
                PV.RPC("ThreeShoot", RpcTarget.All, angle, force, _weaponType);
                break;
            default:
                PV.RPC("ThreeShoot", RpcTarget.All, angle, force, _weaponType);
                break;
        }
    }
    public IEnumerator DoubleShot(float angle, Vector2 force, int _weaponType)
    {
        PV.RPC("ThreeShoot", RpcTarget.All, angle, force, _weaponType);
        while (weaponClone != null)
        {
            yield return null;
        }
        PV.RPC("ThreeShoot", RpcTarget.All, angle, force, _weaponType);
        Shooted();
    }
    [PunRPC]
    public void ThreeShoot(float angle, Vector2 force, int weaponType)
    {
        float addingAngle = angle;
        angle = -angle;
        for (int i = 0; i < 3; ++i)
        {
            weaponClone = Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/" + weaponNames[weaponType]), shootPositon.position, Quaternion.identity);
            weaponClone.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            weaponClone.GetComponent<Rigidbody2D>().AddForce(Vector2.up * angle, ForceMode2D.Impulse);
            angle += addingAngle;
        }
        Shooted();
    }
    public GameObject shield;
    [PunRPC]
    public void Shield()
    {
        shield.SetActive(true);
    }
    public void CannonUp(int _directionCheck)
    {
        cannon.transform.eulerAngles += new Vector3(0, 0, 1f * cannonMoveSpeed * _directionCheck * Time.deltaTime);
    }

    public void CannonDown(int _directionCheck)
    {
        cannon.transform.eulerAngles += new Vector3(0, 0, -1f * cannonMoveSpeed * _directionCheck * Time.deltaTime);
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
        facingDirection *= -1;
        transform.GetChild(1).localScale = new Vector3(facingDirection * setScale, 1, 1);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Explosion"))
        {
            if (shield.activeSelf)
            {
                hpBar.value -= other.GetComponentInParent<Weapons>().damage / 2;
                other.GetComponent<CapsuleCollider2D>().enabled = false;
                shield.SetActive(false);
            }
            else
            {
                hpBar.value -= other.GetComponentInParent<Weapons>().damage;
                other.GetComponent<CapsuleCollider2D>().enabled = false;
            }
            if (UIManager.Instance.tank.GetComponent<PlayerController>().hpBar.value <= 0)
            {
                PV.RPC("GameEnd", RpcTarget.All);
            }
            UIManager.Instance.SetPlayerHp((int)hpBar.value, gameObject);
        }
    }
    public bool gameEnd;
    public bool leaveRoom;
    [PunRPC]
    public void GameEnd()
    {
        myTurn = false;
        gameEnd = true;
        //ui 메니저에서 방나가기 불러오기
    }
    public int lobbyScene = 0;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            rigidbody.velocity = Vector2.zero;
        }
    }
}
