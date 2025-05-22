using System;
using System.Collections;
using UnityEngine;

public class Pawn : MonoBehaviour,IDamageable
{
    [Header("Health")]
    public float Health;
    public float MaxHealth = 100f;

    private Action GotHit;
    
    public float MovementSpeed = 1f;
    
    public virtual void UpdateHealth(float value , Vector3 direction)
    {
        Health += value;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }else if (Health <= 0)
        {
            Die();
        }


        TakeHit(direction);
        
        
        
    }

    public virtual void TakeHit(Vector3 direction)
    {
        print("Got Hit");
    }
    
    

    public IEnumerator UpdateHealthRoutine(float delay, int duration, float value)
    {
        int counter = 0;

        while (counter < duration) 
        {
         
            yield return new WaitForSeconds(delay);
            UpdateHealth(value , Vector3.zero);
            counter++;
        }
    }   


    public virtual void Die()
    {
        print("Died");
        Destroy(gameObject);
    }
}



public interface IDamageable
{
    void UpdateHealth(float value , Vector3 direction);
}

