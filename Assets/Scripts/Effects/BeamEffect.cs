using UnityEngine;
using System.Collections;

public class BeamEffect : MonoBehaviour {

    public GameObject innerBeam;
    public LensFlare flare1;
    public LensFlare flare2;
    public float fadeDuration = 0.2f;

    private bool isOn = true;
    private float progress = 0;

	// Use this for initialization
	void Start () {
        innerBeam.transform.localScale = new Vector3(0, 0, 1);
        isOn = true;
        progress = 0;

    }
	
	// Update is called once per frame
	void Update () {
	    if (isOn && progress < 1f) {
            progress += Time.deltaTime / fadeDuration;
            progress = Mathf.Min(progress, 1f);
        } else if (!isOn) {
            progress -= Time.deltaTime / fadeDuration;
            if (progress <= 0) {
                Destroy(gameObject);
            }
        }
        innerBeam.transform.localScale = new Vector3(progress, progress, 1);
        flare1.brightness = progress;
        flare2.brightness = progress;
    }

    public void Off() {
        isOn = false;
    }
}
