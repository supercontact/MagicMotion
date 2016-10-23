using UnityEngine;
using System.Collections.Generic;

public class Helper : Unit {

    public enum HelperState
    {
        Attacking,
        Returning,
        Following,
        Busy
    }

    public Unit master;
    public int attackDamage = 10;
    public float bulletRange = 8f;
    public float bulletSpeed = 8f;
    public float detectionRange = 10f;
    public float attackRange = 6f;
    public float attackMovingRange = 6f;
    public float attackReturnRange = 5f;
    public float startFollowRange = 3f;
    public float followRange = 2.5f;
    public float pushBackRange = 1.5f;
    public HelperState helperState = HelperState.Following;
    public Unit hostileTarget;

    public GameObject Crystal;


    private float lastThinkTime;

    public override void Awake() {
        base.Awake();
    }

    public override void Start() {
        base.Start();
    }

    public override void Update() {

        if (isBusy() && state != State.Attacking) {
            helperState = HelperState.Busy;
        } else if (helperState == HelperState.Busy) {
            helperState = HelperState.Following;
        }

        if (!isDead) {
            if (helperState == HelperState.Attacking || helperState == HelperState.Following) {
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
                        if (Vector3.Distance(closestEnemy.transform.position, master.transform.position) <= attackMovingRange + attackRange) {
                            hostileTarget = closestEnemy;
                            helperState = HelperState.Attacking;
                        }
                    }
                } else {
                    if (hostileTarget.isDead ||
                        Vector3.Distance(hostileTarget.transform.position, transform.position) > detectionRange ||
                        Vector3.Distance(hostileTarget.transform.position, master.transform.position) > attackMovingRange + attackRange) {
                        hostileTarget = null;
                        helperState = HelperState.Following;
                    } else if (Vector3.Distance(transform.position, master.transform.position) > attackMovingRange) {
                        hostileTarget = null;
                        helperState = HelperState.Returning;
                    } else if (Vector3.Distance(hostileTarget.transform.position, transform.position) > attackRange) {
                        pursueDistance = 0;
                        Pursue(hostileTarget);
                        helperState = HelperState.Attacking;
                    } else {
                        Attack(hostileTarget);
                        helperState = HelperState.Attacking;
                    }
                }

                if (helperState == HelperState.Following) {
                    // Following
                    if (Vector3.Distance(transform.position, master.transform.position) > startFollowRange) {
                        pursueDistance = followRange;
                        Pursue(master);
                    } else if (Vector3.Distance(transform.position, master.transform.position) < pushBackRange) {
                        Vector3 pos = master.transform.position + (transform.position - master.transform.position).normalized * followRange;
                        MoveTo(pos);
                    }
                }
            } else if (helperState == HelperState.Returning) {
                // Returning
                pursueDistance = followRange;
                Pursue(master);
                if (Vector3.Distance(transform.position, master.transform.position) <= attackReturnRange) {
                    helperState = HelperState.Following;
                }
            }
            Crystal.transform.localRotation = Quaternion.AngleAxis(90 * Time.deltaTime, Vector3.up) * Crystal.transform.localRotation;
        } else {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        base.Update();
    }

    public override void PreAttackAction(Unit target) {

    }

    public override void AttackAction(Unit target) {
        SimpleProjectile bullet = GameObject.Instantiate(Links.links.crystalBullet).GetComponent<SimpleProjectile>();
        bullet.attacker = this;
        bullet.team = team;
        bullet.damage = attackDamage;
        bullet.lifeTime = bulletRange / bulletSpeed;
        bullet.speed = bulletSpeed;
        Vector3 direction = (target.transform.position - transform.position).normalized;
        bullet.Launch(transform.position + 0.5f * direction + 1f * Vector3.up, direction);
    }

    public override void DieAction() {

    }
}
