using UnityEngine;
using System.Collections;

public class ShockRingEffect : MonoBehaviour {

    public MeshRenderer ring;
    public float duration = 0.1f;
    public float radius = 5;

    private float timer = 0;

	// Use this for initialization
	void Start () {
        timer = 0;
        ring.transform.localScale = new Vector3(0, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer >= duration) {
            Destroy(gameObject);
            return;
        } else {
            float progress = timer / duration;
            float size = 2 * radius * progress;
            ring.transform.localScale = new Vector3(size, size, size);
            ring.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, (1 - progress) / 2));
        }
        
	}
}
