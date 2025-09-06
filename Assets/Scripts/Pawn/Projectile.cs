using NUnit.Framework.Constraints;
using System;
using UnityEngine;

public class Projectile : Pawn
{


    [SerializeField] private int MaxRicochets;    
    private int currentRicochets;

    private Rigidbody2D rb;
    private LineRenderer line;


    [SerializeField] private float Damage = 10;
    public Vector3 TargetPosition;
    public float Velocity;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
    }


    private void Update()
    {

        if (TargetPosition != Vector3.zero)
        {
            rb.linearVelocity = TargetPosition * Velocity;
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        print("RICHOCHETTING");


        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();


        if (damageable != null)
        {
            damageable.UpdateHealth(-Damage, -other.contacts[0].normal * 10);
            other.rigidbody.AddForce(rb.linearVelocity,ForceMode2D.Impulse);
            Die();
        
        }


            line.positionCount = 0;
        if (currentRicochets < MaxRicochets)
        {
            currentRicochets++;

            TargetPosition = other.contacts[0].normal;
            line.positionCount = 2;
            line.SetPosition(0, transform.position);
            line.SetPosition(1 , other.contacts[0].normal*1000);

        }
        else
        {
            Die();
        }  
    }

    public override void TakeHit(Vector3 direction)
    {
      
    }

}