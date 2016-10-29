using UnityEngine;
using System.Collections;

/// <summary>
/// The fire ball can push back enemies with its explosive impact.
/// Should use a non-trigger collider.
/// </summary>
public class FireBall : SimpleProjectile {

    public override bool HitAction(Unit target, Collision collision) {
        if (target != null) {
            target.SendFlying(collision.impulse);
        }
        return true;
    }

}
