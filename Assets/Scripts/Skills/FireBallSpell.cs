using UnityEngine;
using System.Collections;

/// <summary>
/// A spell that launches multiple fireballs in front of the caster.
/// It is powerful, but beware that the fire ball can hit friendly units.
/// </summary>
public class FireBallSpell : SpecialAttack {

    public float range = 20;
    public float speed = 10;
    public int damage = 30;
    public int number = 3;
    public float angle = 15;

    public FireBallSpell() {
        attackPeriod = 0.5f;
        attackDelay = 0f;
    }

    public override bool IsUsableNow() {
        return true;
    }

    public override void PreAttackAction() {
        OverlayDisplay.Show(Links.links.circleImage, 0, 0.5f);
    }

    public override void AttackAction() {
        for (int i = -number + 1; i <= number - 1; i += 2) {
            FireBall fireBall = GameObject.Instantiate(Links.links.fireBall).GetComponent<FireBall>();
            fireBall.Attacker = attacker;
            fireBall.damage = damage;
            fireBall.lifeTime = range / speed;
            fireBall.speed = speed;
            float ang = i * angle / 2;
            Vector3 direction = Quaternion.AngleAxis(ang, Vector3.up) * attacker.transform.forward;
            fireBall.Launch(attacker.transform.position + 0.5f * direction + attacker.centerHeight * Vector3.up, direction);
        }
    }
}
