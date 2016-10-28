using UnityEngine;
using System.Collections;

public class RegenerateHealthBuff : MonoBehaviour {

    public Unit unit;
    public float duration = 10;
    public int regenerateAmountPerTick = 1;
    public float tickPerSecond = 5;

    private GameObject effect;
    private float timer;
    private float tickTimer;

	// Use this for initialization
	void Start () {
        effect = Instantiate(Links.links.healthEffect);
        effect.transform.SetParent(unit.transform, false);
        effect.transform.localPosition = Vector3.zero;
        tickTimer = 1 / tickPerSecond - 0.01f;
        timer = duration;
    }
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0) {
            unit.HP += regenerateAmountPerTick;
            unit.HP = Mathf.Min(unit.maxHP, unit.HP);
            tickTimer += 1 / tickPerSecond;
        }
        if (timer <= 0) {
            ParticleSystem particles = effect.GetComponentInChildren<ParticleSystem>();
            particles.transform.SetParent(unit.transform);
            particles.Stop();
            Destroy(particles.gameObject, 5f);
            Destroy(gameObject);
        }
    }
}
