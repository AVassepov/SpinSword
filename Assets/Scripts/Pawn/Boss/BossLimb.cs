using System.Collections;
using UnityEngine;

public class BossLimb : Weapon , IDamageable
{


    [SerializeField] private Enemy.Behaviour Behaviour;

    [SerializeField] private float MaxHealth = 100;
   

    private float health = 0;


    private void Awake()
    {
        health = MaxHealth;
    }

    public void GetStunned()
    {
        Owner.GetComponent<Boss>().StunLimb(this);
    }

    public void OnEnable()
    {
        health = MaxHealth;
    }

   
    public void UpdateHealth(float value, Vector3 direction)
    {
   
        health -= value;

        if(health <= 0)
        {
            GetStunned();
        }
    
    }


}
