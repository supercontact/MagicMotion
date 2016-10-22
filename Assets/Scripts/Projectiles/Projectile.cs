using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public Unit attacker;
    public int team = 0;
    public int damage = 1;
    public bool nonInterruptive = false;
    public bool multiHit = false;
    public float lifeTime = 1;

    public Rigidbody body;
    public Collider[] colliders;

    protected float lifeTimer = 0;
    protected bool targetHit = false;
    protected bool ended = false;

    public virtual void Awake() {
        body = GetComponent<Rigidbody>();
        if (body == null) {
            body = gameObject.AddComponent<Rigidbody>();
        }
        colliders = GetComponentsInChildren<Collider>();
    }

    public virtual void Start() {
        if (attacker != null) {
            foreach (Collider c in colliders) {
                Physics.IgnoreCollision(c, attacker.controller);
            }
        }
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
            unit.ReceiveDamage(damage, attacker, nonInterruptive);
        }
        if (!multiHit) {
            ended = true;
            EndAction(true);
        }
    }

    public virtual bool HitAction(Unit target, Collision collision) {
        // To be overridden, if object hit is not a unit then target = null. Return true to inflict damage.
        return target.team != team;
    }
    public virtual void EndAction(bool targetHit) {
        // To be overridden, called when running out of time and not hitting anything. Return true to destroy immediately.
        Destroy(gameObject);
    }

    public void Cancel(bool pretendTargetHit = true) {
        ended = true;
        targetHit = pretendTargetHit;
        if (targetHit) {
            HitAction(null, null);
        }
        EndAction(targetHit);
    }
}
