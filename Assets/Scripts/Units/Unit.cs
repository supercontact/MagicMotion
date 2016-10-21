using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

    public enum State
    {
        Idle,
        Moving,
        Pursuing,
        Attacking,
        Casting,
        Stunned,
        Flying,
        Controlled
    }

    public int team = 0;
    public int maxHP = 100;
    public int HP = 100;
    public float mass = 1;
    public float gravity = 5;
    public float moveSpeed = 1;
    public float currentMoveSpeed = 1;
    public float attackPeriod = 1;
    public float attackDelay = 0.5f;
    public float interruptDuration = 0.5f;
    public bool interruptableByNormalAttack = true;
    public float pursueDistance = 0;
    public float decayTime = 5;
    public bool isImmuneToControl = false;
    public bool isDead = false;
    public bool isInvincible = false;
    public State state = State.Idle;
    public Vector3 moveTargetPoint;
    public Unit moveTargetUnit;
    public Unit attackTarget;
    public Unit castTarget;
    public SpecialAttack currentSpecialAttack;

    public CharacterController controller;

    private Quaternion targetRotation;
    private float attackTimer = 0;
    private bool attacked = false;
    private float castTimer = 0;
    private bool cast = false;
    private float interruptTimer = 0;
    private float decayTimer = 0;
    private Vector3 flyingVelocity;


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

            // Attacking
            if (state == State.Attacking) {
                attackTimer -= Time.deltaTime;
                float attackMoment = attackPeriod - attackDelay;
                if (attackTimer <= attackMoment && !attacked) {
                    AttackAction(attackTarget);
                    attacked = true;
                }
                if (attackTimer <= 0) {
                    state = State.Idle;
                }
            }

            // Casting
            if (state == State.Casting) {
                castTimer -= Time.deltaTime;
                float castMoment = currentSpecialAttack.lasting ? 0 : currentSpecialAttack.attackPeriod - currentSpecialAttack.attackDelay;
                if (castTimer <= castMoment && !cast) {
                    currentSpecialAttack.AttackAction();
                    cast = true;
                }
                if (castTimer <= 0 && !currentSpecialAttack.lasting) {
                    state = State.Idle;
                }
            }

            // Stunned
            if (state == State.Stunned) {
                interruptTimer -= Time.deltaTime;
                if (interruptTimer <= 0) {
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
        if (state != State.Controlled) {
            if (!controller.isGrounded) {
                flyingVelocity += gravity * Time.deltaTime * Vector3.down;
                controller.Move(flyingVelocity * Time.deltaTime);
            } else {
                if (state == State.Flying) {
                    state = State.Idle;
                    LandAction(flyingVelocity);
                }
                flyingVelocity = Vector3.zero;
            }
        } else {
            flyingVelocity = Vector3.zero;
        }
    }

    public virtual void PreAttackAction(Unit target) {
        // To be overridden
    }
    public virtual void AttackAction(Unit target) {
        // To be overridden
    }
    public virtual int ReceiveDamageAction(int damage, Unit attacker) {
        // To be overridden, return actual damage received.
        return damage;
    }
    public virtual float InterruptAction(float duration) {
        // To be overridden, return actual duration stunned.
        return duration;
    }
    public virtual Vector3 SendFlyingAction(Vector3 acc) {
        // To be overridden, return actual velocity applied.
        return acc;
    }
    public virtual int LandAction(Vector3 velocity) {
        // To be overridden, return landing damage.
        return (int)velocity.sqrMagnitude;
    }
    public virtual void DieAction() {
        // To be overridden
    }



    public bool isBusy() {
        return state == State.Attacking || state == State.Casting || state == State.Stunned || state == State.Flying || state == State.Controlled;
    }

    public void Stop() {
        if (!isBusy()) {
            state = State.Idle;
        }
    }

    public void ForceStop() {
        if (state == State.Casting && cast && currentSpecialAttack.lasting) {
            currentSpecialAttack.Interrupt();
        }
        state = State.Idle;
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
            attacked = false;
        }
    }

    public void Cast(Unit target, SpecialAttack specialAttack) {
        specialAttack.target = target;
        specialAttack.self = this;
        if (!isBusy() && specialAttack.IsUsableNow()) {
            state = State.Casting;
            currentSpecialAttack = specialAttack;
            specialAttack.PreAttackAction();
            castTimer = specialAttack.lasting ? specialAttack.attackDelay : specialAttack.attackPeriod;
            castTarget = target;
            cast = false;
        }
    }

    // if time is 0, interrupt with default unit interrupt duration.
    public void Interrupt(float time = 0) {
        float duration = InterruptAction(time == 0 ? interruptDuration : time);
        if (duration >= 0) {
            ForceStop();
            state = State.Stunned;
            interruptTimer = interruptDuration;
        }
    }

    public void ReceiveDamage(int damage, Unit attacker, bool nonInterruptive = false) {
        if (!isInvincible) {
            HP -= ReceiveDamageAction(damage, attacker);
            if (HP <= 0) {
                Kill();
            } else if (!nonInterruptive && interruptableByNormalAttack) {
                Interrupt();
            }
        }
    }

    public void SendFlying(Vector3 impulse) {
        Vector3 acc = impulse / mass;
        acc = SendFlyingAction(acc);
        if (acc.sqrMagnitude == 0) return;

        ForceStop();
        state = State.Flying;
        flyingVelocity += acc;
        controller.Move(flyingVelocity * Time.deltaTime);
    }

    public void Control() {
        if (!isImmuneToControl) {
            ForceStop();
            state = State.Controlled;
        }
    }

    public void ReleaseControl() {
        if (state == State.Controlled) {
            state = State.Idle;
        }
    }

    public void Kill() {
        if (!isInvincible) {
            decayTimer = decayTime;
            isDead = true;
            DieAction();
        }
    }



    protected float HorizontalDistance(Vector3 p1, Vector3 p2) {
        return Mathf.Sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.z - p1.z) * (p2.z - p1.z));
    }

}
