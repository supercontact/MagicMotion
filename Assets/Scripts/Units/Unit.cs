using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The base class of all units. Can preform basic tasks
/// </summary>
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
    public int attackDamage = 10;
    public float attackPeriod = 1;
    public float attackDelay = 0.5f;
    public float attackRange = 1.5f;
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
    public Animator animator;
    public GameObject damageTextPrefab;

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
            if (attackTimer > 0) {
                attackTimer -= Time.deltaTime;
            }
            if (state == State.Attacking) { 
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
            if (castTimer > 0) {
                castTimer -= Time.deltaTime;
            }
            if (state == State.Casting) {
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


    public bool isBusy() {
        return isDead || state == State.Attacking || state == State.Casting || state == State.Stunned || state == State.Controlled;
    }


    // The following methods are basic actions that a unit can perform:

    /// <summary>
    /// Stops moving.
    /// </summary>
    public void Stop() {
        if (!isBusy()) {
            state = State.Idle;
        }
    }

    /// <summary>
    /// Stops any activity.
    /// </summary>
    public void ForceStop() {
        if (state == State.Attacking && !attacked) {
            attackTimer = 0;
        }
        if (state == State.Casting) {
            state = State.Idle;
            currentSpecialAttack.Interrupt();
            if (!cast) {
                castTimer = 0;
            }
        }
        state = State.Idle;
    }

    /// <summary>
    /// Move to a target position with certain speed.
    /// If speed is 0, move with default unit move speed.
    /// </summary>
    public void MoveTo(Vector3 target, float speed = 0) {
        if (!isBusy()) {
            currentMoveSpeed = speed == 0 ? moveSpeed : speed;
            moveTargetPoint = target;
            state = State.Moving;
        }
    }

    /// <summary>
    /// Move towards a direction with certain speed until Stop or ForceStop is called.
    /// If speed is 0, move with default unit move speed.
    /// </summary>
    public void Move(Vector3 direction, float speed = 0) {
        MoveTo(transform.position + direction * 99999, speed);
    }

    /// <summary>
    /// Following a target unit with certain speed until Stop or ForceStop is called.
    /// A minimum distance of pursueDistance will be kept.s
    /// If speed is 0, move with default unit move speed.
    /// </summary>
    public void Pursue(Unit target, float speed = 0) {
        if (!isBusy()) {
            currentMoveSpeed = speed == 0 ? moveSpeed : speed;
            moveTargetUnit = target;
            state = State.Pursuing;
        }
    }

    /// <summary>
    /// Directly performs an attack towards the target (does not check if target is in attack range).
    /// </summary>
    public void Attack(Unit target) {
        if (!isBusy() && attackTimer <= 0) {
            state = State.Attacking;
            PreAttackAction(target);
            attackTimer = attackPeriod;
            attackTarget = target;
            attacked = false;
            targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(target.transform.position - transform.position, Vector3.up));
        }
    }

    /// <summary>
    /// Directly performs a given special attack towards the target (does not check if target is in attack range, but checks the IsUsableNow condition of the special attack).
    /// </summary>
    public void Cast(Unit target, SpecialAttack specialAttack) {
        specialAttack.target = target;
        specialAttack.attacker = this;
        if (!isBusy() && castTimer <= 0 && specialAttack.IsUsableNow()) {
            state = State.Casting;
            currentSpecialAttack = specialAttack;
            specialAttack.PreAttackAction();
            castTimer = specialAttack.lasting ? specialAttack.attackDelay : specialAttack.attackPeriod;
            castTarget = target;
            cast = false;
        }
    }

    /// <summary>
    /// Stun the unit for a certain duration and interrupt any of its action.
    /// The final duration of the stun is decided by the Unit's InterruptAction method.
    /// If time is 0, interrupt with default unit interrupt duration.
    /// </summary>
    public void Interrupt(float time = 0) {
        float duration = InterruptAction(time == 0 ? interruptDuration : time);
        if (duration >= 0) {
            ForceStop();
            state = State.Stunned;
            interruptTimer = interruptDuration;
        }
    }

    /// <summary>
    /// The unit receives damage. Final damage amount done to the unit is decided by the Unit's ReceiveDamageAction method.
    /// The unit will get stunned if interruptableByNormalAttack is set to true.
    /// </summary>
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

    /// <summary>
    /// Send the unit to the sky. Also stuns the unit when it's flying.
    /// Initial speed can be altered by the Unit's SendFlyingAction method.
    /// </summary>
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

    /// <summary>
    /// Control the unit. Checks isImmuneToControl condition.
    /// Unit controled cannot perform actions until the control is released.
    /// </summary>
    public void Control() {
        if (!isImmuneToControl) {
            ForceStop();
            state = State.Controlled;
        }
    }

    /// <summary>
    /// Release the control of this unit.
    /// </summary>
    public void ReleaseControl() {
        if (state == State.Controlled) {
            state = State.Idle;
        }
    }

    /// <summary>
    /// Instantly kill this unit. Checks isInvincible condition.
    /// </summary>
    public void Kill() {
        if (!isInvincible) {
            decayTimer = decayTime;
            isDead = true;
            DieAction();
        }
    }



    // The following methods are unit specific actions that can be overriden by subclasses:

    /// <summary>
    /// Called when the unit starts an attack (i.e. start attack animation)
    /// </summary>
    public virtual void PreAttackAction(Unit target) {
        // To be overridden
        if (animator != null) {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                animator.SetTrigger("Attack");
                animator.SetFloat("AttackSpeed", (animator.GetFloat("AttackDuration") + 0.1f) / attackPeriod);
            }
        }
    }

    /// <summary>
    /// Called when the unit actually do the attack (i.e. cause damage), which happened attackDelay seconds after initiating the attack.
    /// </summary>
    public virtual void AttackAction(Unit target) {
        // To be overridden
        target.ReceiveDamage(attackDamage, this);
    }

    /// <summary>
    /// Called when the unit receives damage.
    /// Should return actual damage received.
    /// </summary>
    public virtual int ReceiveDamageAction(int damage, Unit attacker) {
        // To be overridden
        return damage;
    }

    /// <summary>
    /// Called when the unit gets stunned.
    /// Should return actual duration stunned. If the duration is negative, the unit is not interrupted.
    /// </summary>
    public virtual float InterruptAction(float duration) {
        // To be overridden
        if (animator != null) {
            float animationDuration = Mathf.Min(duration, interruptDuration);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                animator.SetTrigger("Stun");
                animator.SetFloat("StunSpeed", (animator.GetFloat("StunDuration") + 0.1f) / animationDuration);
            }
        }
        return duration;
    }

    /// <summary>
    /// Called when the unit is sent flying.
    /// Should return actual speed changed applied.
    /// </summary>
    public virtual Vector3 SendFlyingAction(Vector3 acc) {
        // To be overridden
        return acc;
    }

    /// <summary>
    /// Called when the unit hits the land after being sent flying.
    /// Should return damage caused by the crash (currently no damage).
    /// </summary>
    public virtual int LandAction(Vector3 velocity) {
        // To be overridden
        return (int)velocity.sqrMagnitude;
    }

    /// <summary>
    /// Called when the unit dies.
    /// </summary>
    public virtual void DieAction() {
        // To be overridden
        if (animator != null) {
            animator.SetTrigger("Die");
        }
    }



    // Static helper methods:

    /// <summary>
    /// Finds any units within certain range of a given position
    /// </summary>
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

    /// <summary>
    /// Finds any enemies within certain range of a given position
    /// </summary>
    public static List<Unit> EnemiesInRange(Vector3 position, float range, int team, bool aliveOnly = true) {
        List<Unit> units = UnitsInRange(position, range, aliveOnly);
        List<Unit> enemies = units.FindAll(unit => unit.team != team);
        return enemies;
    }

    /// <summary>
    /// Finds any friendly units within certain range of a given position
    /// </summary>
    public static List<Unit> FriendsInRange(Vector3 position, float range, int team, bool aliveOnly = true) {
        List<Unit> units = UnitsInRange(position, range, aliveOnly);
        List<Unit> friends = units.FindAll(unit => unit.team == team);
        return friends;
    }

    public static float HorizontalDistance(Vector3 p1, Vector3 p2) {
        return Mathf.Sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.z - p1.z) * (p2.z - p1.z));
    }

}
