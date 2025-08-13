using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Boss : Enemy
{


    [SerializeField] private List<Transform> BossLimbsAnchors;
    [SerializeField] private List<Transform> BossLimbsTransforms;
    [SerializeField] private List<BossLimb> BossLimbs;

    [SerializeField] private int PhaseCount;

    private int currentPhase;

    private Animator animator;
    private void Start()
    {

        SetUpLimbs();
    }



    public void SetUpLimbs()
    {
        for (int i = 0; i < BossLimbsAnchors.Count; i++)
        {

            BossLimbsTransforms[i].transform.parent = null;
            BossLimbs[i].transform.parent = null;

        }
    }
    public void Attack(int AttackTypeIndex) {

        animator.Play("Attack" + AttackTypeIndex);

    }



    public void StunAllLimbs()
    {
        for (int i = 0; i < BossLimbs.Count; i++)
        {
            BossLimbs[i].GetStunned();
        }
        animator.Play("Stun");
    }

    public void ResetAllLimbs()
    {
        for (int i = 0; i < BossLimbs.Count; i++)
        {
            BossLimbs[i].StartMoving();
        }
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
