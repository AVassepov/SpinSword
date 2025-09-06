using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TheMagician : Boss
{


    public override void EnemyLogic()
    {



       if(agent.remainingDistance <= 0.1f)
        {
            SetDestination();
        }

        agent.SetDestination(Vector3.zero);

        if (!AnimatorBusy && !stunned)
        {
            AnimatorBusy = true;
            StartCoroutine(SetNextMove());
        }

    }

    private Vector3 SetDestination()
    {

        Vector3 destination = Player.transform.position + new Vector3(0, 13, 0);

        NavMeshPath navMeshPath = new NavMeshPath();

        while (!agent.CalculatePath(destination, navMeshPath))
        {
            destination -= new Vector3(0,1,0);
        }



        return destination;
    }

    private IEnumerator SetNextMove()
    {
        AnimatorBusy = true;
        int attackType = Random.Range(0, 6);

        yield return new WaitForSeconds(1);
        // Start a new attack

        switch (attackType)
        {
            case 0:
                CastFireBall();
                print("Fireball");
                break;
            case 1:
                Sweep();
                print("Sweep");
                break;
            case 2:
                RightSwing();
                print("Right Swing");
                break;
            case 3:
                LeftSwing();
                print("Left Swing");
                break;
            case 4:
                ForwardSweep();
                print("Forward Sweep");
                break;
            case 5:
                StartCoroutine(ForwardPunch(Random.Range(0, 2)));
                print("Forward Punch");
                break;
        }

        yield return null;
    }
    private void CastFireBall()
    {
        StartCoroutine(DelayedShot(2, Player.transform.position , 20, ShootingOrigins[0].position, 10 , 2));
        animator.SetTrigger("Fireball");
    }

    private void Sweep()
    {
        animator.SetTrigger("Sweep");
    }

    private void RightSwing()
    {
        animator.SetTrigger("Right Swing");
    }

    private void LeftSwing()
    {
        animator.SetTrigger("Left Swing");
    }

    private void ForwardSweep()
    {
        animator.SetTrigger("Forward Sweep");
        TelegraphAttack(3, true, ShootingOrigins[0].position , Vector2.down);

    }



    private IEnumerator ForwardPunch(int index)
    {
        animator.enabled = false;
        agent.SetDestination(transform.position);
        yield return new WaitForSeconds(1);
        LimbElements[index].CurrentWeapon.transform.LookAt(Player.transform.position);
        StartCoroutine(Punchmovement(LimbElements[index], Player.transform.position));

        yield return null;
    }


    private IEnumerator Punchmovement(WeaponElements usedLimb, Vector3 target)
    {
        Rigidbody2D rb = usedLimb.CurrentWeapon.RB;
        usedLimb.WeaponTargetTransform.position = target * 10;

        yield return new WaitForSeconds(1);
        StartCoroutine(FistStuck(usedLimb));

        yield return null;
    }


    private IEnumerator FistStuck(WeaponElements usedLimb)
    {
        BossLimb limb = usedLimb.CurrentWeapon as BossLimb;
        limb.GetStunned();

        yield return new WaitForSeconds(3);


        PunchRecovery(usedLimb);
        yield return null;
    }

    private void PunchRecovery(WeaponElements usedLimb)
    {
        animator.enabled = true;
    }

}
