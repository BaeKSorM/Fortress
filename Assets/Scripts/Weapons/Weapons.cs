using System.Collections;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public Animator animator;
    public static Weapons Instance;
    public new Rigidbody2D rigidbody;
    public GameObject explosionArea;
    public int damage;
    public int horizontalDivision;
    public int verticalDivision;
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        Instance = this;
        animator = GetComponent<Animator>();
        explosionArea = transform.GetChild(0).gameObject;
    }
    protected void Start()
    {
        switch (UIManager.Instance.selectedWeaponType)
        {
            case UIManager.SelectedWeaponType.Shot:
                damage = 20;
                break;
            case UIManager.SelectedWeaponType.Three_Ball:
                damage = 10;
                break;
            case UIManager.SelectedWeaponType.One_Bounce:
                damage = 20;
                break;
            case UIManager.SelectedWeaponType.Roller:
                damage = 10;
                break;
            case UIManager.SelectedWeaponType.Back_Roller:
                damage = 10;
                break;
            case UIManager.SelectedWeaponType.Granade:
                damage = 20;
                break;
            case UIManager.SelectedWeaponType.Spliter:
                damage = 20;
                break;
            case UIManager.SelectedWeaponType.Breaker:
                damage = 20;
                break;
            case UIManager.SelectedWeaponType.Sniper:
                damage = 70;
                break;
        }
    }
    void Update()
    {
    }
    public IEnumerator Explosion()
    {
        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.SetTrigger("Explosion");
        explosionArea.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
