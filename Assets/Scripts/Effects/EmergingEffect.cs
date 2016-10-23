using UnityEngine;
using System.Collections;

public class EmergingEffect : MonoBehaviour {

    public float riseHeight = 2.5f;
    public float riseDuration = 0.1f;
    public float holdDuration = 0.4f;
    public float dropDuration = 0.5f;

    private Vector3 origin;
    private float timer = 0;

	// Use this for initialization
	void Start () {
        transform.position = transform.position + Vector3.down * riseHeight;
        origin = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        float height;
        if (timer < riseDuration) {
            height = timer / riseDuration;
        } else if (timer < riseDuration + holdDuration) {
            height = 1;
        } else if (timer < riseDuration + holdDuration + dropDuration) {
            height = (riseDuration + holdDuration + dropDuration - timer) / dropDuration;
        } else {
            Destroy(gameObject);
            return;
        }
        transform.position = origin + Vector3.up * height * riseHeight;
	}
}
