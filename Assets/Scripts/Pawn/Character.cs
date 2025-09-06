using System;
using TMPro;
using UnityEngine;

public class Character : Pawn
{


    public BonusEffect BonusEffect;

    [SerializeField] private GameObject healthBar;
    private UnityEngine.UI.Image healthBarImage;


    [HideInInspector] public GameObject healthbarInstance;

    public WeaponElements WeaponElements;
  

    public void Start()
    {
        healthbarInstance = Instantiate(healthBar, transform.position, Quaternion.identity); 

        Health = MaxHealth;
        
        healthBarImage = healthbarInstance.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
        healthbarInstance.transform.parent = null;

        if (healthbarInstance.transform.childCount>2)
        {
            TextMeshProUGUI textMeshProUGUI = healthbarInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.text = gameObject.name.Replace("(Clone)", "");
        }

        if(WeaponElements.CurrentWeapon != null)
        {
            WeaponElements.CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.Constant);
            WeaponElements.CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.WhileEquiped);
        }

    }

    public override void UpdateHealth(float value , Vector3 direction)
    {
        
        Health += value;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }else if (Health <= 0)
        {
            Destroy(healthbarInstance);
            if (WeaponElements.CurrentWeapon) {
                WeaponElements.CurrentWeapon.Drop();
            }
            Die();
        }
        healthBarImage.fillAmount = Health / MaxHealth;

        
        
    }

    
    
    
}

[System.Serializable]
public struct BonusEffect
{
    public float WeaponSize;
    public float BaseDamage;
    public float DamageSpeedMult;
    public float BonusDuration;
    public float SwingSpeed;
    public float MovementSpeed;
    public float MaxHP;
    public float Stamina;
    public float CoolDown;
    public float ApplyPoison;
    public float ApplyBleed;
}

[Serializable]
public struct WeaponElements
{
    public Transform WeaponAnchor;
    public Transform WeaponTargetTransform;
    public Weapon CurrentWeapon;
}