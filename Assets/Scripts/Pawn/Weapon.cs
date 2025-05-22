using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{

    public float MovementSpeed;
    public float WeaponDamage;
    [Range(0.01f, 1f)]
    public float RotationSpeed;
    public float KnockbackForce = 1f;
    
    
    
    Vector3 knockbackDirection;
    
    private Rigidbody2D RB;
    private WeaponUI UI;

    public WeaponType Type;

    public Transform Target;

    private bool clashed;
    private bool lingeringClash;
    public float RecoveryTime = 1f;

    [SerializeField] private GameObject damageCanvas;
    public enum WeaponType
    {
        Sword,
        Spear,
        Club
            
    }
    
    
    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        UI = GetComponent<WeaponUI>();
    }

    void Update()
    {


        if (Target != null)
        {
         
            //  Vector2 weaponPosition = Vector2.Lerp(transform.position, Target.position, RotationSpeed);
            Quaternion weaponRotation = Quaternion.Lerp(transform.rotation, Target.rotation, RotationSpeed );
        
           // RB.MovePosition(weaponPosition);
           if (!clashed)
           {
               RB.AddForce((Target.position  - transform.position) *MovementSpeed );
               transform.rotation = weaponRotation;
           }
          
        }
        
        
    }
    private void OnCollisionEnter2D(Collision2D other)
    {

        if (Target != null)
        {
            Character character = other.gameObject.GetComponent<Character>();

            Rigidbody2D enemyRB = other.gameObject.GetComponent<Rigidbody2D>();

            print(other.relativeVelocity);



            knockbackDirection = -other.contacts[0].normal;


            if (enemyRB && !lingeringClash && (!character || character.CurrentWeapon != this))
            {
                // neutralize a bit before the big hit


                print("Applied :" + KnockbackForce * GetVelocity(other.relativeVelocity) + "knockback force");

                if (other.gameObject.GetComponent<Weapon>() != null)
                {
                    clashed = true;
                    lingeringClash = true;
                    StartCoroutine(ClashRecovery());
                    enemyRB.AddForce(
                        -other.contacts[0].normal * KnockbackForce * RB.linearVelocity * 0.8f,
                        ForceMode2D.Impulse);
                }
                else
                {
                    enemyRB.AddForce(-other.contacts[0].normal * KnockbackForce * RB.linearVelocity,
                        ForceMode2D.Impulse);
                }
            }

            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();


            if (damageable != null)
            {
                if ((-other.contacts[0].normal.x + 0.2 >= transform.up.x &&
                     -other.contacts[0].normal.x - 0.2 <= transform.up.x) ||
                    (-other.contacts[0].normal.y + 0.2 >= transform.up.y &&
                     -other.contacts[0].normal.y - 0.2 <= transform.up.y))
                {
                    if (Type != WeaponType.Club)
                    {
                        float bonus = 1;

                        if (Type == WeaponType.Spear)
                        {
                            bonus = 1.5f;
                        }


                        print("Stabbed");
                        if (!other.gameObject.GetComponent<Character>() || other.gameObject.GetComponent<Character>().CurrentWeapon != this)
                        {
                            damageable.UpdateHealth(-WeaponDamage  * bonus *
                                                    RB.mass * Math.Abs(RB.linearVelocityX) * Math.Abs(RB.linearVelocityY) , knockbackDirection);
                            SpawnDamageCanvas(WeaponDamage  * bonus *
                                              RB.mass * Math.Abs(RB.linearVelocityX) * Math.Abs(RB.linearVelocityY),
                                other.transform);
                        }
                    }
                }
                else
                {
                    if (Type != WeaponType.Spear)
                    {
                        if (Type == WeaponType.Sword)
                        {
                            print("Slashed");
                            if (!other.gameObject.GetComponent<Character>() || other.gameObject.GetComponent<Character>().CurrentWeapon != this)
                            {
                                damageable.UpdateHealth(-WeaponDamage * RB.mass * Math.Abs(RB.linearVelocityX) * Math.Abs(RB.linearVelocityY) , knockbackDirection);
                                SpawnDamageCanvas(WeaponDamage * RB.mass * Math.Abs(RB.linearVelocityX) * Math.Abs(RB.linearVelocityY), other.transform);
                            }
                        }
                        else
                        {
                            print("Smashed");
                            if (!other.gameObject.GetComponent<Character>() || other.gameObject.GetComponent<Character>().CurrentWeapon != this)
                            {
                                damageable.UpdateHealth(-WeaponDamage * RB.mass* Math.Abs(RB.linearVelocityX) * Math.Abs(RB.linearVelocityY) , knockbackDirection);
                                SpawnDamageCanvas(WeaponDamage * RB.mass* Math.Abs(RB.linearVelocityX) * Math.Abs(RB.linearVelocityY),
                                    other.transform);
                            }

                        }

                    }
                }


            }

        }
    }



    private IEnumerator ClashRecovery()
    {
        yield return new WaitForSeconds(RecoveryTime);
        
        clashed = false;
        yield return new WaitForSeconds(RecoveryTime * 2);
        
        lingeringClash = false;
        
        
    }
    
    private float GetVelocity(Vector2  relativeVelocity)
    {
        return Math.Abs(relativeVelocity.x) +Math.Abs(relativeVelocity.y);
    }
    
    

    private void SpawnDamageCanvas(float damage , Transform parent)
    {
        GameObject canvas = Instantiate(damageCanvas, parent.position, Quaternion.identity);
        
        canvas.GetComponent<DamageCanvas>().Setup( Mathf.Round(damage * 100.0f) * 0.01f , new Vector2(Random.Range (-0.5f, 0.5f), 1));
    }


    public void PickUp(Transform target)
    {
        Target = target;
        gameObject.layer = LayerMask.NameToLayer("Weapon");
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
        UI.enabled = false;
    }

    public void Drop()
    {
        Target = null;
        gameObject.layer = LayerMask.NameToLayer("Dropped Weapon");
        GetComponentInChildren<SpriteRenderer>().color = Color.gray;
        UI.enabled = true;
    }
}
