using UnityEngine;
using System.Collections;

public class SpeedBuff : MonoBehaviour {

    public Unit unit;
    public float duration = 5;
    public float speedBoost = 3;

    private GameObject effect;
    private float timer;

	// Use this for initialization
	void Start () {
        effect = Instantiate(Links.links.speedEffect);
        effect.transform.SetParent(unit.transform, false);
        effect.transform.localPosition = Vector3.zero;
        timer = duration;
        unit.moveSpeed += speedBoost;
    }
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            unit.moveSpeed -= speedBoost;
            ParticleSystem particles = effect.GetComponentInChildren<ParticleSystem>();
            particles.transform.SetParent(unit.transform);
            particles.Stop();
            Destroy(particles.gameObject, 5f);
            Destroy(gameObject);
        }
    }
}
