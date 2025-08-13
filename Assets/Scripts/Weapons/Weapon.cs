using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    public float MovementSpeed;
    public float BaseWeaponDamage =1;
    public float SpeedModifier = 0.1f;
    public float RotationSpeed;
    public float KnockbackForce = 1f;
    public Rarity WeaponRarity;

    [SerializeField] private List<Collider2D> DamagingColliders;
    //public LineRenderer LineRenderer;


    Vector3 knockbackDirection;
    
    private Rigidbody2D RB;
    private WeaponUI UI;

    public WeaponType Type;

    public Transform Target;
    

    public List<PassiveEffect> PassiveEffects = new List<PassiveEffect>();

    private bool recovering;
    private bool clashed;
    private bool lingeringClash;
    public float RecoveryTime = 1f;

    public Character Owner;
    [SerializeField] private GameObject damageCanvas;
    public enum WeaponType : byte
    {
        Sword,
        Spear,
        Club
    }

    public enum Rarity : byte
    {
        Undetermined,
        Common,
        Rare,
        Epic,
        Legendary
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

       


         if (DamagingColliders.Contains(other.otherCollider))
         {
             print("Hit Damaging Collider");
         }
         else
         {
             print("Hit non damaging");
         }




        if (Target != null && DamagingColliders.Contains(other.otherCollider))
        {
            Owner = other.gameObject.GetComponent<Character>();

            Rigidbody2D enemyRB = other.gameObject.GetComponent<Rigidbody2D>();



            knockbackDirection = -other.contacts[0].normal;


            if (enemyRB && !lingeringClash && (!Owner || Owner.WeaponElements.CurrentWeapon != this))
            {
                // neutralize a bit before the big hit


               // print("Applied :" + KnockbackForce * RB.linearVelocity + "knockback force");
                Weapon weapon = other.gameObject.GetComponent<Weapon>();


                if (weapon != null)
                {
                    enemyRB.linearVelocity = Vector3.zero;
                    clashed = true;
                    lingeringClash = true;
                    StartCoroutine(ClashRecovery());
                    enemyRB.AddForce(
                      (other.transform.position - transform.position).normalized * KnockbackForce * RB.linearVelocity.magnitude * 0.3f,
                        ForceMode2D.Impulse);


                    FXManager.Instance.PoolObject(FXManager.Instance.GetPooledObject(FXManager.EffectType.Spark), 1, transform.position, transform.rotation);

                }
                else
                {
                    enemyRB.AddForce((other.transform.position - transform.position).normalized * KnockbackForce * RB.linearVelocity.magnitude,
                        ForceMode2D.Impulse);
                }
            }

            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();


            if (damageable != null && !recovering)
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


                     //  print("Stabbed");
                        if (!other.gameObject.GetComponent<Character>() || other.gameObject.GetComponent<Character>().WeaponElements.CurrentWeapon != this)
                        {
                            damageable.UpdateHealth(-(BaseWeaponDamage + Owner.BonusEffect.BaseDamage) * bonus *
                                                    RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier * (1+Owner.BonusEffect.DamageSpeedMult)), knockbackDirection);
                            SpawnDamageCanvas((BaseWeaponDamage + Owner.BonusEffect.BaseDamage) * bonus *
                                                    RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier * (1 + Owner.BonusEffect.DamageSpeedMult)),
                                other.transform);
                        }
                    }
                }
                else
                {
                    if (Type == WeaponType.Sword)
                    {
                       // print("Slashed");
                        if (!other.gameObject.GetComponent<Character>() || other.gameObject.GetComponent<Character>().WeaponElements.CurrentWeapon != this)
                        {
                            damageable.UpdateHealth(-(BaseWeaponDamage + Owner.BonusEffect.BaseDamage) * bonus *
                                                    RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier * (1 + Owner.BonusEffect.DamageSpeedMult)), knockbackDirection);
                            SpawnDamageCanvas((BaseWeaponDamage + Owner.BonusEffect.BaseDamage) * bonus *
                                                    RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier * (1 + Owner.BonusEffect.DamageSpeedMult)),
                               other.transform);
                        }
                    }
                    else if (Type == WeaponType.Spear)
                    {
                     //   print("Spear Slashed");
                        bonus = 0.5f;
                        if (!other.gameObject.GetComponent<Character>() || other.gameObject.GetComponent<Character>().WeaponElements.CurrentWeapon != this)
                        {
                            damageable.UpdateHealth(-(BaseWeaponDamage + Owner.BonusEffect.BaseDamage) * bonus *
                                                    RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier * (1 + Owner.BonusEffect.DamageSpeedMult)), knockbackDirection);
                            SpawnDamageCanvas((BaseWeaponDamage + Owner.BonusEffect.BaseDamage) * bonus *
                                                    RB.mass * (Math.Abs(RB.linearVelocity.magnitude) * SpeedModifier * (1 + Owner.BonusEffect.DamageSpeedMult)),
                               other.transform);
                        }
                    }
                    else
                    {
                     //   print("Smashed");
                        if (!other.gameObject.GetComponent<Character>() || other.gameObject.GetComponent<Character>().WeaponElements.CurrentWeapon != this)
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
        else
        {
            StartCoroutine(DamageRecovery());
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
        yield return new WaitForSeconds(0.3f);

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






    public void ActivateAllEffects( PassiveEffect.ActivationCondition condition)
    {
        for (int i = 0; i < PassiveEffects.Count; i++)
        {
            TryGetEffect(PassiveEffects[i], condition);
        }
    }
    public void ClearAllEffects( PassiveEffect.ActivationCondition condition)
    {
        for (int i = 0; i < PassiveEffects.Count; i++)
        {
            TryClearEffect(PassiveEffects[i], condition);
        }
    }


    private void TryGetEffect(PassiveEffect effect, PassiveEffect.ActivationCondition condition)
    {
        if (effect.Duration > 0) {
            StartCoroutine(ApplyTemporaryEffect(1, condition, effect));
        }
        else
        {
            ApplyEffect(condition,effect);
        }
    }

    private void TryClearEffect(PassiveEffect effect, PassiveEffect.ActivationCondition condition)
    {
        if (effect.Duration > 0)
        {

        }
        else
        {
            ClearEffect(condition, effect);
        }
    }




    //Passive effect related methods


    public IEnumerator ApplyTemporaryEffect( float DurationBonus, PassiveEffect.ActivationCondition condition, PassiveEffect effect)
    {
        SetEffect(1, effect);
        //START EFFECT HERE
        yield return new WaitForSeconds(effect.Duration * DurationBonus);
        SetEffect(-1, effect);
    }

    public void ApplyEffect( PassiveEffect.ActivationCondition condition, PassiveEffect effect)
    {
        if (!effect.Activated && condition == effect.Condition)
        {
            SetEffect(1, effect);
            effect.Activated = true;
        }
    }

    public void ClearEffect(PassiveEffect.ActivationCondition condition, PassiveEffect effect)
    {
        if (effect.Activated && condition == effect.Condition)
        {
            SetEffect(-1, effect);
            effect.Activated = false;
        }
    }



    public void SetEffect(float coef , PassiveEffect effect)
    {
        if (effect.Type == PassiveEffect.EffectType.WeaponSize)
        {
            Owner.BonusEffect.WeaponSize += coef * effect.Percentage;
        }
        else if (effect.Type == PassiveEffect.EffectType.WeaponSpeed)
        {
            Owner.BonusEffect.SwingSpeed += coef * effect.Percentage;
        }
        else if (effect.Type == PassiveEffect.EffectType.MovementSpeed)
        {
            Owner.BonusEffect.MovementSpeed += coef * effect.Percentage;
        }
        else if (effect.Type == PassiveEffect.EffectType.CooldownBonus)
        {
            Owner.BonusEffect.CoolDown += coef * effect.Percentage;
        }
        else if (effect.Type == PassiveEffect.EffectType.DamageSpeedMult)
        {
            Owner.BonusEffect.DamageSpeedMult += coef * effect.Percentage;
        }
        else if (effect.Type == PassiveEffect.EffectType.BaseDamage)
        {
            Owner.BonusEffect.BaseDamage += coef * effect.Percentage;
        }
        else if (effect.Type == PassiveEffect.EffectType.Duration)
        {
            Owner.BonusEffect.BonusDuration += coef * effect.Percentage;
        }
        else if (effect.Type == PassiveEffect.EffectType.Stamina)
        {
            Owner.BonusEffect.Stamina += coef * effect.Percentage;
        }
        else if (effect.Type == PassiveEffect.EffectType.ApplyBleed)
        {
            Owner.BonusEffect.ApplyBleed += coef * effect.Percentage;
        }
        else if (effect.Type == PassiveEffect.EffectType.ApplyPoison)
        {
            Owner.BonusEffect.ApplyPoison += coef * effect.Percentage;
        }
        else if (effect.Type == PassiveEffect.EffectType.MaxHP)
        {
            Owner.BonusEffect.MaxHP += coef * effect.Percentage;
        }
    }
}



[Serializable]
public struct PassiveEffect
{
    public bool Activated;
    public float Percentage;
    // 0 = permanent
    public float Duration;
    public EffectType Type;
    public ActivationCondition Condition;

    public enum ActivationCondition : byte
    {
        Constant,
        WhileEquiped,
        WhileUnequiped
    }
    public enum EffectType : byte
    {
        TBD,
        WeaponSize,
        BaseDamage,
        DamageSpeedMult,
        Duration,
        WeaponSpeed,
        MovementSpeed,
        MaxHP,
        Stamina,
        CooldownBonus,
        Poison,
        ApplyPoison,
        Bleed,
        ApplyBleed
    }

}
