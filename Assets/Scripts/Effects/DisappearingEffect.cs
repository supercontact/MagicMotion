using UnityEngine;
using System.Collections;

public class DisappearingEffect : MonoBehaviour {

    public float duration = 1;
    public MeshRenderer particleQuad;

    private float timer = 0;

	// Use this for initialization
	void Start () {
        timer = 0;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (particleQuad != null) {
            particleQuad.material.SetColor("_TintColor", new Color(1, 1, 1, 1 - timer / duration));
        }
        if (timer >= duration) {
            Destroy(gameObject);
        }
	}
}
