using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    [SerializeField] private Transform FollowAnchor;


    protected NavMeshAgent agent;

    public Encounter Encounter;

    [HideInInspector]public Player Player;

    [SerializeField]private Behaviour enemyBehaviour;

    [SerializeField] private GameObject ProjectilePrefab;

    [SerializeField] protected List<Transform> ShootingOrigins;

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
        WeaponElements.CurrentWeapon.Target = WeaponElements.WeaponTargetTransform;
        WeaponElements.WeaponTargetTransform.parent = null;
        WeaponElements.CurrentWeapon.transform.parent = null;
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

        if (healthbarInstance) { 
            healthbarInstance.transform.position = gameObject.transform.position;
        }
        MoveWeapon();
        EnemyLogic();
    }


    public virtual void EnemyLogic()
    {
        // write logic in child classes
    }



    public override void Die()
    {
        if (Encounter != null)
        {
            Encounter.RemoveEnemy(this);
        }
        Destroy(gameObject);
    }


    public virtual void MoveWeapon()
    {
        if (WeaponElements.WeaponTargetTransform)
        {
            Vector2 weaponPosition = Vector2.Lerp(WeaponElements.WeaponTargetTransform.position, WeaponElements.WeaponAnchor.position, WeaponElements.CurrentWeapon.RotationSpeed);
            Quaternion weaponRotation = Quaternion.Lerp(WeaponElements.WeaponTargetTransform.rotation, WeaponElements.WeaponAnchor.rotation, WeaponElements.CurrentWeapon.RotationSpeed);

            WeaponElements.WeaponTargetTransform.transform.position = weaponPosition;
            WeaponElements.WeaponTargetTransform.transform.rotation = weaponRotation;
        }
    }

    public IEnumerator DelayedShot(float delay , Vector3 dir, float damage, Vector3 origin, float velocity, int projectileLive)
    {

        yield return new WaitForSeconds(delay);
        ShootProjectile( dir,  damage,  origin,  velocity,  projectileLive);
    
    }
    public void ShootProjectile(Vector3 dir, float damage, Vector3 origin, float velocity, int projectileLives)
    {

        Projectile projectile = Instantiate(ProjectilePrefab , origin, Quaternion.identity).GetComponent<Projectile>();


        projectile.Velocity = velocity;


        RaycastHit2D hit = Physics2D.Raycast(origin, dir);





        projectile.TargetPosition = hit.point;
    }

    /// <summary>
    ///  1)Stand Still and spin around
    ///  2)Run to player at all times
    ///  3)run at player, when weapon hits the player retreat, weapon doesnt spin and points at player
    ///  4)Stands back and tries to keep a certain distance away, shoots projectiles
    ///  5)Keeps distance , when the player is not pointing weapon towards it for some time it rushes the player down, hits and retreats
    ///  6)Run to a random position in sight, if player is near the path take a detour to hit them
    ///  7)Run to a random position in room , detour to take a hit while near 
    ///  8)Wander to random position in room, when spots the enemy switches to Chaser Behaviour
    ///  9)Walk between 2-3 preditermined locations.
    ///  10)Moves and rotates from one point to another relative to a point
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
        Patroller,
        Swinger

    }
    
}
