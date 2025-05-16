using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    
    
    private NavMeshAgent agent;



    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        
    }


    private void FixedUpdate()
    {
        
        transform.Rotate(new Vector3(0,0,-5));
        
        
        healthbarInstance.transform.position = gameObject.transform.position;
        
        Vector2 weaponPosition = Vector2.Lerp( WeaponTargetTransform.position , WeaponAnchor.position , CurrentWeapon.RotationSpeed  );
        Quaternion weaponRotation = Quaternion.Lerp(WeaponTargetTransform.rotation, WeaponAnchor.rotation, CurrentWeapon.RotationSpeed );

        WeaponTargetTransform.transform.position = weaponPosition;
        WeaponTargetTransform.transform.rotation = weaponRotation;

    }

    private void Update()
    {
        
        agent.SetDestination(new Vector3(0, 0, 1));
    }
    
    
}
