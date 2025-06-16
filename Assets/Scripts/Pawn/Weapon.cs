using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{

    public float MovementSpeed;
    public float BaseWeaponDamage =1;
    public float SpeedModifier = 0.1f;
    [Range(0.01f, 1f)]
    public float RotationSpeed;
    public float KnockbackForce = 1f;



    //public LineRenderer LineRenderer;
    
    
    Vector3 knockbackDirection;
    
    private Rigidbody2D RB;
    private WeaponUI UI;

    public WeaponType Type;

    public Transform Target;




    private bool recovering;
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
    
    
    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        UI = GetComponent<WeaponUI>();
            
        if(Target == null)
        {
            Drop();
        }
    
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



            knockbackDirection = -other.contacts[0].normal;


            if (enemyRB && !lingeringClash && (!character || character.CurrentWeapon != this))
            {
                // neutralize a bit before the big hit


                print("Applied :" + KnockbackForce * RB.linearVelocity + "knockback force");
                Weapon weapon = other.gameObject.GetComponent<Weapon>();


                if (weapon != null)
                {
                    enemyRB.linearVelocity = Vector3.zero;
                    clashed = true;
                    lingeringClash = true;
                    StartCoroutine(ClashRecovery());
                    enemyRB.AddForce(
                      (other.transform.position - transform.position).normalized * KnockbackForce * RB.linearVelocity.magnitude * 0.3f ,
                        ForceMode2D.Impulse);


                    EffectsManager.Instance.PoolObject(EffectsManager.Instance.GetPooledObject(EffectsManager.EffectType.Spark), 1 , transform.position ,transform.rotation );

                }
                else
                {
                    enemyRB.AddForce((other.transform.position - transform.position).normalized * KnockbackForce * RB.linearVelocity.magnitude,
                        ForceMode2D.Impulse);
                }
            }

            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();


            if (damageable != null && !recovering )
            {

                float bonus = 1;
                StartCoroutine(DamageRecovery());
                if ((-other.contacts[0].normal.x + 0.2 >= transform.up.x &&
                     -other.contacts[0].normal.x - 0.2 <= transform.up.x) ||
                    (-other.contacts[0].normal.y + 0.2 >= transform.up.y &&
                     -other.contacts[0].normal.y - 0.2 <= transform.up.y))
                {

                    if (Type != WeaponType.Club)
                    {

                        if (Type == WeaponType.Spear)
                        {
                            bonus = 1.5f;
                        }


                        print("Stabbed");
                        if (!other.gameObject.GetComponent<Character>() || other.gameObject.GetComponent<Character>().CurrentWeapon != this)
                        {
                            damageable.UpdateHealth(-BaseWeaponDamage * bonus *
                                                    RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier), knockbackDirection);
                            SpawnDamageCanvas(BaseWeaponDamage * bonus *
                                              RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier),
                                other.transform);
                        }
                    }
                }
                else
                {
                        if (Type == WeaponType.Sword)
                        {
                            print("Slashed");
                            if (!other.gameObject.GetComponent<Character>() || other.gameObject.GetComponent<Character>().CurrentWeapon != this)
                            {
                            damageable.UpdateHealth(-BaseWeaponDamage * bonus *
                                                RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier), knockbackDirection); 
                            SpawnDamageCanvas(BaseWeaponDamage * bonus *
                                             RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier),
                               other.transform);
                            }
                        }else  if (Type == WeaponType.Spear)
                        {
                            print("Spear Slashed");
                            bonus = 0.5f;
                            if (!other.gameObject.GetComponent<Character>() || other.gameObject.GetComponent<Character>().CurrentWeapon != this)
                            {
                            damageable.UpdateHealth(-BaseWeaponDamage * bonus *
                                                 RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier), knockbackDirection);
                            SpawnDamageCanvas(BaseWeaponDamage * bonus *
                                             RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier),
                               other.transform);
                            }
                        }
                        else
                        {
                            print("Smashed");
                            if (!other.gameObject.GetComponent<Character>() || other.gameObject.GetComponent<Character>().CurrentWeapon != this)
                            {
                            damageable.UpdateHealth(-BaseWeaponDamage * bonus *
                                                RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier), knockbackDirection); 
                            SpawnDamageCanvas(BaseWeaponDamage * bonus *
                                              RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier),
                                other.transform);
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
        RB.linearVelocity = new Vector2 (-RB.linearVelocityX/2, -RB.linearVelocityY/2);
        lingeringClash = false;
        
        
    }

    private IEnumerator DamageRecovery()
    {

        recovering = true;
        yield return new WaitForSeconds(0.1f);

        recovering = false;


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
