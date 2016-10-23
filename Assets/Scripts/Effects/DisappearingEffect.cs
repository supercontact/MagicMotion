using UnityEngine;
using System.Collections;

public class DisappearingEffect : MonoBehaviour {

    public float duration = 1;

    private float timer = 0;

	// Use this for initialization
	void Start () {
        timer = 0;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer >= duration) {
            Destroy(gameObject);
        }
	}
}
