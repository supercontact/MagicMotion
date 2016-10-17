using UnityEngine;  
using System.Collections;

public class BasicAI : Unit
{

    public Unit hero;
    public float wanderSpeed = 2;
    public float thinkPeriod = 2;
    public bool isAggressive = true;
    public float detectionRange = 10;
    public float attackRange = 1.5f;

    private float lastThinkTime;


    public override void Start() {
        base.Start();
    }

    public override void Update() {
        if (!isDead) {
            if (Vector3.Distance(transform.position, hero.transform.position) < detectionRange && isAggressive) {
                if (Vector3.Distance(transform.position, hero.transform.position) > attackRange) {
                    Pursue(hero);
                } else {
                    Attack(hero);
                }
            } else {
                if (Time.time - lastThinkTime > thinkPeriod) {
                    lastThinkTime = Time.time;

                    int rnd = Random.Range(0, 2);
                    switch (rnd) {
                    case 0:
                        Stop();
                        break;

                    case 1:
                        Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                        Move(rot * Vector3.forward, wanderSpeed);
                        break;
                    }
                }
            }
        } else {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        base.Update();
    }

    public override void PreAttackAction(Unit target) {
        
    }

    public override void AttackAction(Unit target) {
        ReceiveDamage(5, target);
    }

    public override void DieAction() {
        
    }

}