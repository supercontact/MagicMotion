using UnityEngine;
using System.Collections.Generic;

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
        List<Unit> enemiesInRange = Unit.EnemiesInRange(attacker.transform.position, maxRadius, attacker.team);
        foreach (Unit enemie in enemiesInRange) {
            Vector3 vec = enemie.transform.position - attacker.transform.position;
            Vector3 radialVec = Vector3.ProjectOnPlane(vec, Vector3.up).normalized;
            float power = 1 - vec.magnitude / maxRadius;
            if (power > 0) {
                Vector3 impulse = maxUpImpulse * power * Vector3.up + maxRadialImpulse * power * radialVec;
                enemie.SendFlying(impulse);
            }
        }
        OverlayDisplay.ShowImage(Links.links.flashImage, 0, 0.5f);
        GameObject shockRing = GameObject.Instantiate(Links.links.shockRing);
        shockRing.transform.position = attacker.transform.position;
    }
}
