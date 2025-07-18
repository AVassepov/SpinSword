using UnityEngine;

public class Character : Pawn
{


    public BonusEffect BonusEffect;

    [SerializeField] private GameObject healthBar;
    private UnityEngine.UI.Image healthBarImage;
    
    
    public Transform WeaponAnchor;


    public Transform WeaponTargetTransform;
    
    [HideInInspector] public GameObject healthbarInstance;
    public Weapon CurrentWeapon;
  

    public void Start()
    {
        healthbarInstance = Instantiate(healthBar, transform.position, Quaternion.identity); 

        Health = MaxHealth;
        
        healthBarImage = healthbarInstance.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
        healthbarInstance.transform.parent = null;

        if(CurrentWeapon != null)
        {
            CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.Constant);
            CurrentWeapon.ActivateAllEffects(PassiveEffect.ActivationCondition.WhileEquiped);
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
            if (CurrentWeapon) {
                CurrentWeapon.Drop();
            }
            Die();
        }
        healthBarImage.fillAmount = Health / MaxHealth;

        
        
    }

    
    
    
}

[System.Serializable]
public class BonusEffect
{
    public float WeaponSize;
    public float BaseDamage;
    public float DamageSpeedMult = 1;
    public float BonusDuration;
    public float SwingSpeed;
    public float MovementSpeed;
    public float MaxHP;
    public float Stamina;
    public float CoolDown;
    public float ApplyPoison;
    public float ApplyBleed;
}
