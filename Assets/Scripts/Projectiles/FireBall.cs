using UnityEngine;
using System.Collections;

public class FireBall : SimpleProjectile {

    public override bool HitAction(Unit target, Collision collision) {
        if (target != null) {
            target.SendFlying(collision.impulse);
        }
        return true;
    }

}
