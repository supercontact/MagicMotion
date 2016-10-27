using UnityEngine;
using System.Collections;

public class DisappearingEffect : MonoBehaviour {

    public float duration = 1;
    public MeshRenderer particleQuad;
    public LensFlare flare;
    public ParticleSystem particles;
    public float particlesPersistDuration = 5f;

    private float timer = 0;
    private Color quadInitialColor;
    private float flareInitialBrightness;

	// Use this for initialization
	void Start () {
        timer = 0;
        if (particleQuad != null) {
            quadInitialColor = particleQuad.material.GetColor("_TintColor");
        }
        if (flare != null) {
            flareInitialBrightness = flare.brightness;
        }
        if (particles != null) {
            particles.transform.SetParent(null);
            particles.Stop();
            Destroy(particles.gameObject, particlesPersistDuration);
        }
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer >= duration) {
            Destroy(gameObject);
            return;
        }
        if (particleQuad != null) {
            particleQuad.material.SetColor("_TintColor", new Color(quadInitialColor.r, quadInitialColor.g, quadInitialColor.b, (1 - timer / duration) * quadInitialColor.a));
        }
        if (flare != null) {
            flare.brightness = (1 - timer / duration) * flareInitialBrightness;
        }
	}
}
