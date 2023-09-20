using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public Animator animator;
    public static Weapons Instance;
    public Rigidbody2D rigidbody;
    public int damage;
    void Start()
    {
        Instance = this;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        switch (UIManager.Instance.selectedWeaponType)
        {
            case UIManager.SelectedWeaponType.Shot:
                damage = 10;
                break;
            case UIManager.SelectedWeaponType.Three_Ball:
                damage = 10;
                break;
            case UIManager.SelectedWeaponType.One_Bounce:
                damage = 20;
                break;
            case UIManager.SelectedWeaponType.Roller:
                damage = 25;
                break;
            case UIManager.SelectedWeaponType.Back_Roller:
                damage = 25;
                break;
            case UIManager.SelectedWeaponType.Granade:
                damage = 30;
                break;
            case UIManager.SelectedWeaponType.Spliter:
                damage = 20;
                break;
            case UIManager.SelectedWeaponType.Breaker:
                damage = 15;
                break;
            case UIManager.SelectedWeaponType.Sniper:
                damage = 100;
                break;
        }
    }
    void Update()
    {
        transform.right = rigidbody.velocity;
    }
    public IEnumerator Explosion()
    {
        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.SetTrigger("Explosion");
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}