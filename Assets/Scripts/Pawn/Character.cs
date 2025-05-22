using UnityEngine;

public class Character : Pawn
{


    [SerializeField] private GameObject healthBar;
    private UnityEngine.UI.Image healthBarImage;
    
    
    public Transform WeaponAnchor;

    public Transform WeaponTargetTransform;
    
    [HideInInspector] public GameObject healthbarInstance;
    public Weapon CurrentWeapon;
  

    public void Start()
    {
        healthbarInstance = Instantiate(healthBar,transform); 
        
        healthBarImage = healthbarInstance.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
        healthbarInstance.transform.parent = null;

    }

    public override void UpdateHealth(float value , Vector3 direction)
    {
        
        Health += value;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }else if (Health <= 0)
        {
            Die();
        }
        healthBarImage.fillAmount = Health / MaxHealth;

        
        
    }


    public void SetCurrentWeapon(Weapon newWeapon)
    {
        CurrentWeapon = null;
    }
    
    
    
}
