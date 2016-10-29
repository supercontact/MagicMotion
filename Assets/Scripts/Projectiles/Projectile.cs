using UnityEngine;
using System.Collections;

/// <summary>
/// The base class of all projectiles. It supports collision detection and basic damage dealing.
/// It supports both colliders and triggers.
/// </summary>
public class Projectile : MonoBehaviour {

    private Unit attacker;
    public int team = 0;
    public int damage = 1;
    public bool nonInterruptive = false; // Non-interruptive pojectile will not interrupt the target hit even if the target is interruptable by normal attacks.
    public bool multiHit = false; // If multiHit is set to true, EndAction will only be called when the lift time of the projectile runs out.
    public float lifeTime = 1;

    public GameObject explosionPrefab;

    public Rigidbody body;
    public Collider[] colliders;

    protected float lifeTimer = 0;
    protected bool targetHit = false;
    protected bool ended = false;

    public Unit Attacker {
        get {return attacker;}
        set {
            attacker = value;
            if (attacker != null) {
                foreach (Collider c in colliders) {
                    Physics.IgnoreCollision(c, attacker.controller);
                }
            }
        }
    }

    public virtual void Awake() {
        body = GetComponent<Rigidbody>();
        if (body == null) {
            body = gameObject.AddComponent<Rigidbody>();
        }
        colliders = GetComponentsInChildren<Collider>();
    }

    public virtual void Start() {

    }

    public virtual void Update() {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime) {
            ended = true;
            EndAction(targetHit);
        }
    }

    public virtual void OnCollisionEnter(Collision collision) {
        if (!ended) {
            Unit unit = collision.gameObject.GetComponent<Unit>();
            bool doHit = HitAction(unit, collision);
            if (doHit) {
                Hit(unit);
            }
        }
    }

    public virtual void OnTriggerEnter(Collider collider) {
        if (!ended) {
            Unit unit = collider.GetComponent<Unit>();
            bool doHit = HitAction(unit, null);
            if (doHit) {
                Hit(unit);
            }
        }
    }

    private void Hit(Unit unit) {
        targetHit = true;
        if (unit != null && damage > 0) {
            unit.ReceiveDamage(damage, Attacker, nonInterruptive);
        }
        if (!multiHit) {
            ended = true;
            EndAction(true);
        }
    }

    /// <summary>
    /// Called when the bullet hit something.
    /// If object hit is not a unit then target is null.
    /// If the projectile only has triggers then collision is null.
    /// Should return true if it is considered a hit (will inflict damage, and if multiHit is false, call EndAction).
    /// </summary>
    public virtual bool HitAction(Unit target, Collision collision) {
        // To be overridden
        return target == null || target.team != team;
    }

    /// <summary>
    /// Called when the bullet finishes its role.
    /// targetHit indicates whether the bullet hit something or the time is up.
    /// </summary>
    public virtual void EndAction(bool targetHit) {
        // To be overridden
        if (explosionPrefab != null) {
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
        }
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < particles.Length; i++) {
            particles[i].transform.SetParent(null);
            particles[i].Stop();
            Destroy(particles[i].gameObject, 5f);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Cancel the projectile.
    /// </summary>
    public void Cancel(bool pretendTargetHit = true) {
        ended = true;
        targetHit = pretendTargetHit;
        if (targetHit) {
            HitAction(null, null);
        }
        EndAction(targetHit);
    }
}
