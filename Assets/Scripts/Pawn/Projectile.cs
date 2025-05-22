using System;
using UnityEngine;

public class Projectile : Pawn
{


    [SerializeField] private int MaxRicochets;    
    private int currentRicochets;

    private Rigidbody2D rb;
    private LineRenderer line;

    private Vector3 targetPosition;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
    }


    private void Update()
    {

        if (targetPosition != Vector3.zero)
        {
            rb.linearVelocity = targetPosition * 10;
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        print("RICHOCHETTING");

        line.positionCount = 0;
        if (currentRicochets < MaxRicochets)
        {
            currentRicochets++;

            targetPosition = other.contacts[0].normal;
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