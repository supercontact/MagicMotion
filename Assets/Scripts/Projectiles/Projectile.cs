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

    public virtual void Start() {
        body = GetComponent<Rigidbody>();
        if (body == null) {
            body = gameObject.AddComponent<Rigidbody>();
        }
        colliders = GetComponentsInChildren<Collider>();
        if (attacker != null) {
            foreach (Collider c in colliders) {
                Physics.IgnoreCollision(c, attacker.controller);
            }
        }
    }

    public virtual void Update() {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime) {
            if (EndAction()) Destroy(gameObject);
        }
    }

    public virtual void OnCollisionEnter(Collision collision) {
        Unit unit = collision.gameObject.GetComponent<Unit>();
        bool hit = HitAction(unit, collision);
        if (hit) {
            Hit(unit);
        }
    }

    public virtual void OnTriggerEnter(Collider collider) {
        Unit unit = collider.GetComponent<Unit>();
        bool hit = HitAction(unit, null);
        if (hit) {
            Hit(unit);
        }
    }

    private void Hit(Unit unit) {
        if (unit != null && unit.team != team) {
            unit.ReceiveDamage(damage, attacker, nonInterruptive);
        }
        if (!multiHit) {
            Destroy(gameObject);
        }
    }

    public virtual bool HitAction(Unit target, Collision collision) {
        // To be overridden, if object hit is not a unit then target = null. Return true to inflict damage.
        return true;
    }
    public virtual bool EndAction() {
        // To be overridden, called when running out of time and not hitting anything. Return true to destroy immediately.
        return true;
    }
}
