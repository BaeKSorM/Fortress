using System.Collections;
using System.Collections.Generic;
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

    public float dividPower;

    public Slider hpBar;

    private Vector2 newVelocity;
    private Vector2 newForce;
    private Vector2 capsuleColliderSize;
    private Vector2 slopeNormalPerp;
    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    public float setScale;
    public float threeBallAngle;

    void Start()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        cannon = GameObject.Find("Cannon");
        spriteRenderer = GetComponent<SpriteRenderer>();
        chargingGaugeBar = chargingGauge.GetComponent<Slider>();
        chargingGaugeBar.maxValue = shootPowerMax;
        chargingMax = chargingGaugeBar.maxValue;
        capsuleColliderSize = cc.size;
        setScale = transform.localScale.x;
    }

    void Update()
    {
        SelectAction();
        Move();
        if (Input.anyKey)
        {
            PressingKey();
        }
        cannonMove();
        RotateFloorAngle();
    }

    public void SelectAction()
    {
        if (canMove)
        {
            if (Input.GetKey(leftKey))
            {
                xInput = -1;
                UIManager.Instance.canMoveSlider.value -= 0.1f * Time.deltaTime;
            }
            else if (Input.GetKey(rightKey))
            {
                xInput = 1;
                UIManager.Instance.canMoveSlider.value -= 0.1f * Time.deltaTime;
            }
            else if (Input.GetKeyUp(leftKey) || Input.GetKeyUp(rightKey))
            {
                xInput = 0;
            }
        }
        else
        {
            xInput = 0;
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
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
        rb.constraints = RigidbodyConstraints2D.None;
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

    public void Move()
    {
        if (isMove)
        {
            if (xInput == 1 && facingDirection == -1)
            {
                Flip();
            }
            else if (xInput == -1 && facingDirection == 1)
            {
                Flip();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Jump();
            }
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
        if (isRotatecannon)
        {
            int directionCheck = (int)transform.localScale.x;
            if (Input.GetKey(upKey) && (cannon.transform.eulerAngles.z < cannonRotateMaxAngle + transform.eulerAngles.z || cannon.transform.eulerAngles.z > 190 + cannonRotateMaxAngle + transform.eulerAngles.z))
            {
                cannonUp(directionCheck);
            }
            if (Input.GetKey(downKey) && (cannon.transform.eulerAngles.z > cannonRotateMinAngle + transform.eulerAngles.z))
            {
                cannonDown(directionCheck);
            }
        }
    }

    private void RotateFloorAngle()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, whatIsGround);
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

        if (rb.velocity.y <= 0.0f)
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
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, capsuleColliderSize.y / 2));
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void ApplyMovement()
    {
        if (isGrounded && !isOnSlope && !isJumping)
        {
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rb.velocity = newVelocity;
        }
        else if (isGrounded && isOnSlope && isWalkOnSlope && !isJumping)
        {
            newVelocity.Set(movementSpeed * slopeNormalPerp.x * -xInput, movementSpeed * slopeNormalPerp.y * -xInput);
            rb.velocity = newVelocity;
        }
        else if (!isGrounded)
        {
            newVelocity.Set(movementSpeed * xInput, rb.velocity.y);
            rb.velocity = newVelocity;
        }
    }

    public List<string> weaponNames = new List<string>() { "Shot", "Three-Ball", "One-Bounce", "Roller", "Back-Roller", "Granade", "Spliter", "Breaker", "Snipers" };

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
        }
        Debug.Log(((int)selectedWeaponType));
        UIManager.Instance.DecreaseWeaponCount((int)selectedWeaponType);
    }

    public void ShootType(int weaponNumber)
    {
        GameObject weaponClone = Instantiate(Resources.Load<GameObject>("Prefabs/" + weaponNames[weaponNumber]), shootPositon.position, Quaternion.identity);
        weaponClone.GetComponent<Rigidbody2D>().AddForce((shootPositon.position - cannon.transform.position) * chargingGaugeBar.value, ForceMode2D.Impulse);
    }
    public void ShootType(int weaponNumber, float angle)
    {
        float addingAngle = angle;
        angle = -angle;
        for (int i = 0; i < 3; ++i)
        {
            GameObject weaponClone = Instantiate(Resources.Load<GameObject>("Prefabs/" + weaponNames[weaponNumber]), shootPositon.position, Quaternion.identity);
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

        if (isOnSlope && isWalkOnSlope && xInput == 0.0f)
        {
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }

    public void Jump()
    {
        if (isJump)
        {
            isJump = false;
            isJumping = true;
            newVelocity.Set(0.0f, 0.0f);
            rb.velocity = newVelocity;
            newForce.Set(0.0f, jumpForce);
            rb.AddForce(newForce, ForceMode2D.Impulse);
        }
    }

    public void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(facingDirection * setScale, setScale, setScale);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            hpBar.value -= other.GetComponent<Weapons>().damage;
        }
    }
}
