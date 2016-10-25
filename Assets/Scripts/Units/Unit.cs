using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

    public enum State
    {
        Idle,
        Moving,
        Pursuing,
        Attacking,
        Casting,
        Stunned,
        Controlled
    }

    public int team = 0;
    public int maxHP = 100;
    public int HP = 100;
    public float mass = 1;
    public float height = 1;
    public float centerHeight = 0.5f;
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
    public GameObject damageTextPrefab;

    public State state = State.Idle;
    public Vector3 moveTargetPoint;
    public Unit moveTargetUnit;
    public Unit attackTarget;
    public Unit castTarget;
    public SpecialAttack currentSpecialAttack;

    public CharacterController controller;
    public Animator animator;

    private Quaternion targetRotation;
    private float attackTimer = 0;
    private bool attacked = false;
    private float castTimer = 0;
    private bool cast = false;
    private float interruptTimer = 0;
    private float decayTimer = 0;
    private Vector3 flyingVelocity;
    private bool isFlying;


    public virtual void Awake() {
        controller = GetComponent<CharacterController>();
    }

    public virtual void Start() {
        if (damageTextPrefab == null) {
            damageTextPrefab = Links.links.damageText;
        }
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
                    currentSpecialAttack.EndAction();
                    state = State.Idle;
                }
            }

            // Stunned
            if (state == State.Stunned) {
                interruptTimer -= Time.deltaTime;
                if (interruptTimer <= 0 && !isFlying) {
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
                if (isFlying) {
                    LandAction(flyingVelocity);
                    isFlying = false;
                }
                flyingVelocity = Vector3.zero;
            }
        } else {
            flyingVelocity = Vector3.zero;
        }
    }

    public virtual void PreAttackAction(Unit target) {
        // To be overridden
        if (animator != null) {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                animator.SetTrigger("Attack");
                animator.SetFloat("AttackSpeed", (animator.GetFloat("AttackDuration") + 0.1f) / attackPeriod);
            }
        }
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
        if (animator != null) {
            float animationDuration = Mathf.Min(duration, interruptDuration);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                animator.SetTrigger("Stun");
                animator.SetFloat("StunSpeed", (animator.GetFloat("StunDuration") + 0.1f) / animationDuration);
            }
        }
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
        if (animator != null) {
            animator.SetTrigger("Die");
        }
    }



    public bool isBusy() {
        return isDead || state == State.Attacking || state == State.Casting || state == State.Stunned || state == State.Controlled;
    }

    public void Stop() {
        if (!isBusy()) {
            state = State.Idle;
        }
    }

    public void ForceStop() {
        if (state == State.Casting) {
            state = State.Idle;
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
        specialAttack.attacker = this;
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
        if (!isInvincible && !isDead) {
            int realDamage = ReceiveDamageAction(damage, attacker);
            HP -= realDamage;
            if (HP <= 0) {
                Kill();
            } else if (!nonInterruptive && interruptableByNormalAttack) {
                Interrupt();
            }
            DamageTextEffect text = GameObject.Instantiate(damageTextPrefab).GetComponent<DamageTextEffect>();
            text.SetPosition(transform.position + Vector3.up * 1);
            text.SetText(realDamage.ToString());
        }
    }

    public void SendFlying(Vector3 impulse) {
        Vector3 acc = impulse / mass;
        acc = SendFlyingAction(acc);
        if (acc.sqrMagnitude == 0) return;

        ForceStop();
        state = State.Stunned;
        flyingVelocity += acc;
        controller.Move(flyingVelocity * Time.deltaTime);
        isFlying = true;
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



    public static List<Unit> UnitsInRange(Vector3 position, float range, bool aliveOnly = true) {
        Collider[] colliders = Physics.OverlapSphere(position, range);
        List<Unit> units = new List<Unit>();
        foreach (Collider c in colliders) {
            Unit unit = c.GetComponent<Unit>();
            if (unit != null && (!unit.isDead || !aliveOnly) && Vector3.Distance(unit.transform.position, position) <= range) {
                units.Add(unit);
            }
        }
        return units;
    }
    public static List<Unit> EnemiesInRange(Vector3 position, float range, int team, bool aliveOnly = true) {
        List<Unit> units = UnitsInRange(position, range, aliveOnly);
        List<Unit> enemies = units.FindAll(unit => unit.team != team);
        return enemies;
    }
    public static List<Unit> FriendsInRange(Vector3 position, float range, int team, bool aliveOnly = true) {
        List<Unit> units = UnitsInRange(position, range, aliveOnly);
        List<Unit> friends = units.FindAll(unit => unit.team == team);
        return friends;
    }


    protected float HorizontalDistance(Vector3 p1, Vector3 p2) {
        return Mathf.Sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.z - p1.z) * (p2.z - p1.z));
    }

}
