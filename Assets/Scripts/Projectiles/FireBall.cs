using UnityEngine;
using System.Collections;

public class FireBall : SimpleProjectile {

    public override void Awake() {
        base.Awake();
        damage = 30;
    }

    public override bool HitAction(Unit target, Collision collision) {
        if (target != null) {
            target.SendFlying(collision.impulse);
        }
        return true;
    }

    public override void EndAction(bool targetHit) {
        Destroy(gameObject);
    }

}
