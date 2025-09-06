using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D.IK;
using static UnityEngine.Rendering.DebugUI;

public class Boss : Enemy
{

    public Animator animator;
    public List<WeaponElements> LimbElements;

    [SerializeField] private int PhaseCount;

    [SerializeField] private int StunTime;

    private int currentPhase;

    protected bool stunned;
    public bool AnimatorBusy;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
        SetUpLimbs();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

    }



    public void SetUpLimbs()
    {
        for (int i = 0; i < LimbElements.Count; i++)
        {

            LimbElements[i].WeaponTargetTransform.parent = null;
            LimbElements[i].CurrentWeapon.transform.parent = null;

        
                
        }
    }
    public void Attack(int AttackTypeIndex) {

        animator.Play("Attack" + AttackTypeIndex);

    }

    public override void MoveWeapon()
    {
        for (int i = 0; i < LimbElements.Count; i++)
        {
            Vector2 weaponPosition = Vector2.Lerp(
            LimbElements[i].WeaponTargetTransform.position,
            LimbElements[i].WeaponAnchor.position,
            LimbElements[i].CurrentWeapon.RotationSpeed);

                Quaternion weaponRotation = Quaternion.Lerp(
            LimbElements[i].WeaponTargetTransform.rotation,
            LimbElements[i].WeaponAnchor.rotation,
            LimbElements[i].CurrentWeapon.RotationSpeed);

            LimbElements[i].WeaponTargetTransform.transform.position = weaponPosition;
            LimbElements[i].WeaponTargetTransform.transform.rotation = weaponRotation;
         
        }
    }

    

    private IEnumerator StunSelf()
    {


        stunned = true;

        animator.SetTrigger("Stun");
        yield return new WaitForSeconds(StunTime);

        stunned = false;

        for (int i = 0; i < LimbElements.Count; i++)
        {         
            LimbElements[i].CurrentWeapon.enabled = true;
        }


        yield return null;
    }


    public IEnumerator TelegraphAttack(float duration , bool isLine , Vector3 origin, Vector3 direction ) {

        if (isLine)
        {
            lineRenderer.enabled = false;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction);

            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, direction);



            yield return new WaitForSeconds(duration);

            lineRenderer.enabled = true;
        }
        else
        {
            //write the logic for this when working on later bosses
        }




            yield return null;
    }

    public void StunLimb(BossLimb limb)
    {
        limb.enabled = false;

        bool allStunned = false;


        for (int i = 0; i < LimbElements.Count; i++)
        {
            if (LimbElements[i].CurrentWeapon.isActiveAndEnabled)
            {
                allStunned=true;
            }
        }

        if (!allStunned)
        {
            StartCoroutine(ResetLimb(limb));

        }
        else
        {
            StopAllCoroutines();
            StunSelf();
        }


    }

    public IEnumerator ResetLimb(BossLimb limb)
    {
        yield return new WaitForSeconds(StunTime);

        limb.enabled = true;
        yield return null;
    }


    private void StartNextPhase()
    {
        currentPhase++;
        StartCoroutine(ResetHealth());
    }


    public override void Die()
    {
        if (currentPhase != PhaseCount) {
            StartNextPhase();
        }
        else { 
            if (Encounter != null)
            {
                Encounter.RemoveEnemy(this);
            }
            Destroy(gameObject);
        }
    }

    public IEnumerator ResetHealth()
    {
        float delayChange = 0;
        while (Health < MaxHealth)
        {
            yield return new WaitForSeconds(0.1f - delayChange);
            UpdateHealth(1, Vector3.zero);
            if(delayChange < 0.1f) { 
                delayChange += 0.01f;
            }
        }

        yield return null;
    }


    public enum BossState
    {
        Idle,
        Start,
        Attack1,
        Attack2, 
        Attack3,
        Death,
        Stunned,
        PhaseTransition,
        Block
    }


}
