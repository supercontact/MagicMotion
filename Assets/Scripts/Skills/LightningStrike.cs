using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Summon a quick lightning strike that hits a random place in front of the caster.
/// The lightning can be attracted to an enemy if the enemy is close enough to the striking position.
/// </summary>
public class LightningStrike : SpecialAttack {

    public float areaCenterDistance = 5;
    public float areaRadius = 4;
    public float snapDistance = 3.5f;
    public int damage = 40;
    public float stunDuration = 3;

    public LightningStrike() {
        attackPeriod = 0.5f;
        attackDelay = 0f;
    }

    public override bool IsUsableNow() {
        return true;
    }

    public override void PreAttackAction() {
        OverlayDisplay.Show(Links.links.ligntningImage, 0, 0.5f);
    }

    public override void AttackAction() {
        Vector3 center = attacker.transform.position + areaCenterDistance * attacker.transform.forward;
        Vector2 randomVector = Random.insideUnitCircle;
        Vector3 position = center + new Vector3(randomVector.x, 0, randomVector.y) * areaRadius;
        List<Unit> enemies = Unit.EnemiesInRange(position, snapDistance, attacker.team);

        float closestDistance = snapDistance;
        Unit closestEnemy = null;
        foreach (Unit enemy in enemies) {
            float distance = Vector3.Distance(enemy.transform.position, position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        GameObject ligntning = GameObject.Instantiate(Links.links.lightning);
        if (closestEnemy != null) {
            closestEnemy.ReceiveDamage(damage, attacker, true);
            closestEnemy.Interrupt(stunDuration);
            ligntning.transform.position = closestEnemy.transform.position;
        } else {
            ligntning.transform.position = position;
        }
    }
}
