using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    
    
    private NavMeshAgent agent;

    public Encounter Encounter;

    [HideInInspector]public Player Player;

    [SerializeField]private Behaviour enemyBehaviour;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;


        /*  if(Anchor && Weapon) { 
          WeaponTargetTransform = Anchor;
          Weapon.Target = WeaponTargetTransform;
          CurrentWeapon = Weapon;

          WeaponAnchor.transform.parent = null;
          Weapon.transform.parent = null;
          }*/
        CurrentWeapon.Target = WeaponTargetTransform;
        WeaponTargetTransform.transform.parent = null;
        CurrentWeapon.transform.parent = null;
    }


    private void FixedUpdate()
    {


        if (enemyBehaviour == Behaviour.Stationary)
        {
            transform.Rotate(new Vector3(0, 0, -5));
        }
        else if (enemyBehaviour == Behaviour.Chaser)
        {
            transform.Rotate(new Vector3(0, 0, -5));
            agent.SetDestination(Player.transform.position);
        }
         
        
        healthbarInstance.transform.position = gameObject.transform.position;
        
        
        if(WeaponTargetTransform){
            Vector2 weaponPosition = Vector2.Lerp( WeaponTargetTransform.position , WeaponAnchor.position , CurrentWeapon.RotationSpeed  );
            Quaternion weaponRotation = Quaternion.Lerp(WeaponTargetTransform.rotation, WeaponAnchor.rotation, CurrentWeapon.RotationSpeed );

            WeaponTargetTransform.transform.position = weaponPosition;
            WeaponTargetTransform.transform.rotation = weaponRotation;
        }
    }
    public override void Die()
    {
        if (Encounter != null)
        {
            Encounter.RemoveEnemy(this);
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        
        //agent.SetDestination(new Vector3(0, 0, 1));
    }



    /// <summary>
    ///  1) Stand Still and spin around
    ///  2)Run to player at all times
    ///  3)run at player, when weapon hits the player retreat, weapon doesnt spin and points at player
    ///  4)Stands back and tries to keep a certain distance away, shoots projectiles
    ///  5)Keeps distance , when the player is not pointing weapon towards it for some time it rushes the player down, hits and retreats
    ///  6)Run to a random position in sight, if player is near the path take a detour to hit them
    ///  7)Run to a random position in room , detour to take a hit while near 
    ///  8)Wander to random position in room, when spots the enemy switches to Chaser Behaviour
    ///  9) Walk between 2-3 preditermined locations.
    /// </summary>
    public enum Behaviour
    {
        Stationary,
        Chaser,
        Poker,
        Ranger,
        Stalker,
        Blind,
        Wanderer,
        Patroller

    }
    
}
