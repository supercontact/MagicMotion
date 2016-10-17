using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

    public enum State
    {
        Idle,
        Moving,
        Pursuing,
        Attacking,
        Casting
    }

    public int team = 0;
    public int maxHP = 100;
    public int HP = 100;
    public float moveSpeed = 1;
    public float currentMoveSpeed = 1;
    public float attackPeriod = 1;
    public float attackDelay = 0.5f;
    public float pursueDistance = 0;
    public float decayTime = 5;
    public bool isDead = false;
    public State state = State.Idle;
    public Vector3 moveTargetPoint;
    public Unit moveTargetUnit;
    public Unit attackTarget;
    public Unit castTarget;
    public SpecialAttack currentSpecialAttack;

    protected CharacterController controller;

    private Quaternion targetRotation;
    private float attackTimer = 0;
    private float castTimer = 0;
    private float decayTimer = 0;


    public virtual void Start() {
        controller = GetComponent<CharacterController>();
    }

    public virtual void Update() {
        if (!isDead) {
            if (state == State.Moving) {
                // Moving
                float moveDistance = currentMoveSpeed * Time.deltaTime;
                controller.Move(moveDistance * (moveTargetPoint - transform.position).normalized);
                targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(moveTargetPoint - transform.position, Vector3.up));
                if (HorizontalDistance(moveTargetPoint, transform.position) < moveDistance) {
                    state = State.Idle;
                }
            } else if (state == State.Pursuing) {
                // Pursuing
                float moveDistance = currentMoveSpeed * Time.deltaTime;
                if (HorizontalDistance(moveTargetUnit.transform.position, transform.position) > pursueDistance + moveDistance) {
                    controller.Move(moveDistance * (moveTargetUnit.transform.position - transform.position).normalized);
                }
                targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(moveTargetUnit.transform.position - transform.position, Vector3.up));
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10);

            if (attackTimer > 0) {
                // Attacking
                attackTimer -= Time.deltaTime;
                float attackMoment = attackPeriod - attackDelay;
                if (attackTimer <= attackMoment && attackTimer + Time.deltaTime > attackMoment) {
                    AttackAction(attackTarget);
                }
                if (attackTimer <= 0 && state == State.Attacking) {
                    state = State.Idle;
                }
            }

            if (castTimer > 0) {
                // Casting
                castTimer -= Time.deltaTime;
                float castMoment = currentSpecialAttack.attackPeriod - currentSpecialAttack.attackDelay;
                if (castTimer <= castMoment && castTimer + Time.deltaTime > castMoment) {
                    currentSpecialAttack.AttackAction(attackTarget, this);
                }
                if (castTimer <= 0 && state == State.Casting) {
                    state = State.Idle;
                }
            }
        } else {
            decayTimer -= Time.deltaTime;
            if (decayTimer <= 0) {
                Destroy(gameObject);
                return;
            }
        }

        // Gravity
        controller.Move(10 * Vector3.down * Time.deltaTime);
    }

    public virtual void PreAttackAction(Unit target) {
        // To be overridden
    }
    public virtual void AttackAction(Unit target) {
        // To be overridden
    }
    public virtual int ReceiveDamageAction(int damage, Unit attacker) {
        // To be overridden
        return damage;
    }
    public virtual void DieAction() {
        // To be overridden
    }



    public bool isBusy() {
        return state == State.Attacking || state == State.Casting;
    }

    public void Stop() {
        if (!isBusy()) {
            state = State.Idle;
        }
    }

    public void ForceStop() {
        state = State.Idle;
        attackTimer = 0;
        castTimer = 0;
    }

    // if speed is 0, move with default unit move speed.
    public void MoveTo(Vector3 target, float speed = 0) {
        if (!isBusy()) {
            currentMoveSpeed = speed == 0 ? moveSpeed : speed;
            moveTargetPoint = target;
            state = State.Moving;
        }
    }

    public void Move(Vector3 direction, float speed = 0) {
        MoveTo(transform.position + direction * 99999, speed);
    }

    public void Pursue(Unit target, float speed = 0) {
        if (!isBusy()) {
            currentMoveSpeed = speed == 0 ? moveSpeed : speed;
            moveTargetUnit = target;
            state = State.Pursuing;
        }
    }

    public void Attack(Unit target) {
        if (!isBusy()) {
            state = State.Attacking;
            PreAttackAction(target);
            attackTimer = attackPeriod;
            attackTarget = target;
        }
    }

    public void Cast(Unit target, SpecialAttack specialAttack) {
        if (!isBusy() && specialAttack.IsUsableNow(target, this)) {
            state = State.Casting;
            currentSpecialAttack = specialAttack;
            specialAttack.PreAttackAction(target, this);
            castTimer = specialAttack.attackPeriod;
            castTarget = target;
        }
    }

    public void ReceiveDamage(int damage, Unit attacker) {
        HP -= ReceiveDamageAction(damage, attacker);
        if (HP <= 0) {
            Kill();
        }
    }

    public void Kill() {
        decayTimer = decayTime;
        isDead = true;
        DieAction();
    }



    protected float HorizontalDistance(Vector3 p1, Vector3 p2) {
        return Mathf.Sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.z - p1.z) * (p2.z - p1.z));
    }

}
