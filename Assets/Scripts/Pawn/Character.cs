using System;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.UIElements;

public class Character : MonoBehaviour,IDamageable
{


    [SerializeField] private GameObject healthBar;
    private UnityEngine.UI.Image healthBarImage;
    
    
    public Transform WeaponAnchor;

    public Transform WeaponTargetTransform;
    
    public float Health;
    
    public float MaxHealth = 100f;

    [HideInInspector] public GameObject healthbarInstance;
    public Weapon CurrentWeapon;
  

    public void Start()
    {
        healthbarInstance = Instantiate(healthBar,transform); 
        
        healthBarImage = healthbarInstance.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
        healthbarInstance.transform.parent = null;

    }

    public void UpdateHealth(float value)
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


    public void Die()
    {
        
    }

    public void SetCurrentWeapon(Weapon newWeapon)
    {
        CurrentWeapon = null;
    }
    
    
    
}


public interface IDamageable
{
    void UpdateHealth(float value);
}
