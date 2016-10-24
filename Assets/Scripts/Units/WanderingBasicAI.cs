using UnityEngine;
using System.Collections;

public class WanderingBasicAI : BasicAI {

    public float wanderSpeed = 2f;
    public float wanderDurationMin = 0.5f;
    public float wanderDurationMax = 2f;
    public float pauseDurationMin = 0.5f;
    public float pauseDurationMax = 4f;

    private bool isWandering = false;
    private float timer;
    private float duration;

    public override void Start() {
        base.Start();
        timer = 0;
        duration = Random.Range(pauseDurationMin, pauseDurationMax);
    }

    // Update is called once per frame
    public override void Update () {
        base.Update();
        if (aiState == AIState.Idle) {
            timer += Time.deltaTime;
            if (timer >= duration) {
                timer = 0;
                if (isWandering) {
                    Stop();
                    duration = Random.Range(pauseDurationMin, pauseDurationMax);
                    isWandering = false;
                } else {
                    Move(Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Vector3.right, wanderSpeed);
                    duration = Random.Range(pauseDurationMin, pauseDurationMax);
                    isWandering = true;
                }
            }
        } else {
            timer = 0;
            isWandering = false;
        }
	}
}
