using UnityEngine;
using System.Collections;

/// <summary>
/// The effect of the lightning.
/// </summary>
public class LightningEffect : MonoBehaviour {

    public MeshRenderer imageRenderer;
    public LensFlare flare;
    public float fadeInDuration = 0.1f;
    public float holdDuration = 0.1f;
    public float fadeOutDuration = 0.2f;
    public bool faceCamera = true;

    private float timer = 0;
    private Transform cam;

	// Use this for initialization
	void Start () {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        GameObject mark = Instantiate(Links.links.lightningMark);
        mark.transform.position = transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(cam.position - transform.position, Vector3.up));
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        float brightness;
        if (timer < fadeInDuration) {
            brightness = timer / fadeInDuration;
        } else if (timer < fadeInDuration + holdDuration) {
            brightness = 1;
        } else if (timer < fadeInDuration + holdDuration + fadeOutDuration) {
            brightness = (fadeInDuration + holdDuration + fadeOutDuration - timer) / fadeOutDuration;
        } else {
            Destroy(gameObject);
            return;
        }
        imageRenderer.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, brightness));
        flare.brightness = brightness;
        if (faceCamera) {
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(cam.position - transform.position, Vector3.up));
        }
    }
}
