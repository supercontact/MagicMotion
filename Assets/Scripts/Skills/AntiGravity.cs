using UnityEngine;
using System.Collections;

public class AntiGravity : SpecialAttack {

    public float maxRadius = 5;
    public float maxUpImpulse = 10;
    public float maxRadialImpulse = 5;

    public AntiGravity() {
        attackPeriod = 0.5f;
        attackDelay = 0f;
    }

    public override bool IsUsableNow() {
        return true;
    }
    public override void AttackAction() {
        Collider[] collidersInRange = Physics.OverlapSphere(self.transform.position, maxRadius);
        foreach (Collider c in collidersInRange) {
            Unit unitInRange = c.GetComponent<Unit>();
            if (unitInRange != null && unitInRange.team != self.team) {
                Vector3 vec = unitInRange.transform.position - self.transform.position;
                Vector3 radialVec = Vector3.ProjectOnPlane(vec, Vector3.up).normalized;
                float power = 1 - vec.magnitude / maxRadius;
                if (power > 0) {
                    Vector3 impulse = maxUpImpulse * power * Vector3.up + maxRadialImpulse * power * radialVec;
                    unitInRange.SendFlying(impulse);
                }
            }
        }
    }
}
