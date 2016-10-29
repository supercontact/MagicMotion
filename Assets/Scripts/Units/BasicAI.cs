using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// BasicAI chases enemies within its detection range and attacks them if they are within attack range. 
/// </summary>
public class BasicAI : Unit {

    public enum AIState
    {
        Idle,
        Attacking,
        Busy
    }

    public float detectionRange = 10f;

    public AIState aiState = AIState.Idle;
    public Unit hostileTarget;

    public override void Awake() {
        base.Awake();
    }

    public override void Start() {
        base.Start();
    }

    public override void Update() {

        if (isBusy() && state != State.Attacking) {
            aiState = AIState.Busy;
        } else if (aiState == AIState.Busy) {
            aiState = AIState.Idle;
        }

        if (!isDead) {
            if (aiState != AIState.Busy) {
                if (hostileTarget == null) {
                    List<Unit> enemies = Unit.EnemiesInRange(transform.position, detectionRange, team);
                    if (enemies.Count > 0) {
                        float closestDistance = float.MaxValue;
                        Unit closestEnemy = null;
                        foreach (Unit enemy in enemies) {
                            float distance = Vector3.Distance(enemy.transform.position, transform.position);
                            if (distance < closestDistance) {
                                closestDistance = distance;
                                closestEnemy = enemy;
                            }
                        }
                        hostileTarget = closestEnemy;
                        aiState = AIState.Attacking;
                    }
                } else {
                    if (hostileTarget.isDead || Vector3.Distance(hostileTarget.transform.position, transform.position) > detectionRange) {
                        hostileTarget = null;
                        aiState = AIState.Idle;
                    } else if (Vector3.Distance(hostileTarget.transform.position, transform.position) > attackRange) {
                        pursueDistance = 0;
                        Pursue(hostileTarget);
                        aiState = AIState.Attacking;
                    } else {
                        Attack(hostileTarget);
                        aiState = AIState.Attacking;
                    }
                }
            }
        }
        base.Update();
    }
}
