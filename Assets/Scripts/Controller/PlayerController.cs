using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Profiling.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public Transform shootPositon;
    public GameObject cannon;
    public GameObject bullet;
    public GameObject chargingGauge;
    public SpriteRenderer spriteRenderer;
    public Slider chargingGaugeBar;

    public float movementSpeed;
    public float groundCheckRadius;
    public float jumpForce;
    public float slopeCheckDistance;
    public float maxSlopeAngle;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D fullFriction;
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
    public float cannonRotateMaxAngle;
    public float cannonRotateMinAngle;
    public float groundAngle;
    public bool canMove;
    public bool myTurn;

    public float dividPower;

    public Slider hpBar;

    private Vector2 newVelocity;
    private Vector2 newForce;
    private Vector2 slopeNormalPerp;
    private new Rigidbody2D rigidbody;
    private CapsuleCollider2D capsuleCollider;
    public BoxCollider2D boxCollider;
    public float setScale;
    public float threeBallAngle;
    public int lookPosition = 1;
    public int movePower = 1;

    void Start()
    {
        Instance = this;
        rigidbody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        cannon = GameObject.Find("Cannon");
        spriteRenderer = GetComponent<SpriteRenderer>();
        chargingGaugeBar = chargingGauge.GetComponent<Slider>();
        chargingGaugeBar.maxValue = shootPowerMax;
        chargingMax = chargingGaugeBar.maxValue;
        setScale = transform.localScale.x;
        rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
    }

    void Update()
    {
        SelectAction();
        Turn();
        if (Input.anyKey)
        {
            PressingKey();
        }
        cannonMove();
        RotateFloorAngle();
    }

    public void SelectAction()
    {
        if (myTurn && canMove && !isShoot)
        {
            if (Input.GetKey(leftKey))
            {
                rigidbody.constraints = RigidbodyConstraints2D.None;
                xInput = -1;
                lookPosition = -1;
                UIManager.Instance.niddle.transform.eulerAngles -= new Vector3(0, 0, 10f * Time.deltaTime);
            }
            else if (Input.GetKey(rightKey))
            {
                rigidbody.constraints = RigidbodyConstraints2D.None;
                xInput = 1;
                lookPosition = 1;
                UIManager.Instance.niddle.transform.eulerAngles -= new Vector3(0, 0, 10f * Time.deltaTime);
            }
            else if (Input.GetKeyUp(leftKey) || Input.GetKeyUp(rightKey))
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
            Shoot(UIManager.Instance.selectedWeaponType);
            Shooted();
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
                    Shoot(UIManager.Instance.selectedWeaponType);
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

    public void cannonMove()
    {
        if (myTurn && isRotatecannon)
        {
            int directionCheck = (int)transform.localScale.x;
            if (Input.GetKey(upKey))
            {
                cannonUp(directionCheck);
            }
            if (Input.GetKey(downKey))
            {
                cannonDown(directionCheck);
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
        CheckGround();
        SlopeCheck();
        ApplyMovement();
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

    public void Shoot(UIManager.SelectedWeaponType selectedWeaponType)
    {
        switch (selectedWeaponType)
        {
            case UIManager.SelectedWeaponType.Shot:
            case UIManager.SelectedWeaponType.One_Bounce:
            case UIManager.SelectedWeaponType.Roller:
            case UIManager.SelectedWeaponType.Back_Roller:
            case UIManager.SelectedWeaponType.Granade:
            case UIManager.SelectedWeaponType.Spliter:
            case UIManager.SelectedWeaponType.Breaker:
            case UIManager.SelectedWeaponType.Sniper:
                ShootType((int)selectedWeaponType);
                break;
            case UIManager.SelectedWeaponType.Three_Ball:
                ShootType((int)selectedWeaponType, threeBallAngle);
                break;
            default:
                break;
        }
        // Debug.Log(((int)selectedWeaponType));
        UIManager.Instance.DecreaseWeaponCount((int)selectedWeaponType);
    }

    public void ShootType(int weaponNumber)
    {
        GameObject weaponClone = Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/" + weaponNames[weaponNumber]), shootPositon.position, Quaternion.identity);
        weaponClone.GetComponent<Rigidbody2D>().AddForce((shootPositon.position - cannon.transform.position) * chargingGaugeBar.value, ForceMode2D.Impulse);
        Debug.Log(chargingGaugeBar.value);
    }
    public void ShootType(int weaponNumber, float angle)
    {
        float addingAngle = angle;
        angle = -angle;
        for (int i = 0; i < 3; ++i)
        {
            GameObject weaponClone = Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/" + weaponNames[weaponNumber]), shootPositon.position, Quaternion.identity);
            weaponClone.GetComponent<Rigidbody2D>().AddForce((shootPositon.position - cannon.transform.position) * chargingGaugeBar.value, ForceMode2D.Impulse);
            weaponClone.GetComponent<Rigidbody2D>().AddForce(Vector2.up * angle, ForceMode2D.Impulse);
            angle += addingAngle;
        }
    }

    public void cannonUp(int _directionCheck)
    {
        cannon.transform.eulerAngles += new Vector3(0, 0, .1f * _directionCheck);
    }

    public void cannonDown(int _directionCheck)
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
        facingDirection *= -1;
        transform.localScale = new Vector3(facingDirection * setScale, setScale, setScale);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Explosion"))
        {
            hpBar.value -= other.GetComponentInParent<Weapons>().damage;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            rigidbody.velocity = Vector2.zero;
        }
    }
}
