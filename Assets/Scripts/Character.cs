using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State
    {
        Idle,
        RunningToEnemy,
        RunningFromEnemy,
        BeginAttack,
        Attack,
        BeginShoot,
        Shoot,
        Dead
    }

    public enum Weapon
    {
        Pistol,
        Bat,
        Punch
    }

    public Weapon weapon;
    public float runSpeed;
    public float distanceFromEnemy;
    public Character target;
    State state;
    Animator animator;
    Vector3 originalPosition;
    Quaternion originalRotation;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        state = State.Idle;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void SetState(State newState)
    {
        state = newState;
    }

    public bool IsDead()
    {
        return state == State.Dead;
    }

    [ContextMenu("Attack")]
    void AttackEnemy()
    {
        if (IsDead())
        {
            Debug.Log("I am dead =(");
            return;
        }
        if (target.IsDead())
        {
            Debug.Log("Target is dead =(");
            return;
        }
        switch (weapon) {
            case Weapon.Bat:
            case Weapon.Punch:
                state = State.RunningToEnemy;
                break;

            case Weapon.Pistol:
                state = State.BeginShoot;
                break;
        }
    }

    bool RunTowards(Vector3 targetPosition, float distanceFromTarget)
    {
        Vector3 distance = targetPosition - transform.position;
        if (distance.magnitude < 0.00001f) {
            transform.position = targetPosition;
            return true;
        }

        Vector3 direction = distance.normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        targetPosition -= direction * distanceFromTarget;
        distance = (targetPosition - transform.position);

        Vector3 step = direction * runSpeed;
        if (step.magnitude < distance.magnitude) {
            transform.position += step;
            return false;
        }

        transform.position = targetPosition;
        return true;
    }

    public void Hit()
    {
        animator.SetTrigger("Death");
        state = State.Dead;
    }
    
    public void HitTarget()
    {
        target.Hit();
    }
    
    public 

    void FixedUpdate()
    {
        switch (state) {
            case State.Idle:
                animator.SetFloat("Speed", 0.0f);
                transform.rotation = originalRotation;
                break;

            case State.RunningToEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(target.transform.position, distanceFromEnemy))
                    state = State.BeginAttack;
                break;

            case State.BeginAttack:
                SetAttackAnimation();
                state = State.Attack;
                break;

            case State.Attack:
                break;

            case State.BeginShoot:
                animator.SetTrigger("Shoot");
                state = State.Shoot;
                break;

            case State.Shoot:
                break;

            case State.RunningFromEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(originalPosition, 0.0f))
                    state = State.Idle;
                break;
            case State.Dead:
                break;
        }
    }

    void SetAttackAnimation()
    {
        switch (weapon)
        {
            case Weapon.Bat:
                animator.SetTrigger("MeleeAttack");
                break;
            case Weapon.Punch:
                animator.SetTrigger("Punch");
                break;
        }
    }
}
