﻿using UnityEngine;
using System.Collections;

/// <summary>
/// The effect of the light beam.
/// </summary>
public class BeamEffect : MonoBehaviour {

    public GameObject innerBeam;
    public LensFlare flare1;
    public LensFlare flare2;
    public ParticleSystem particles;
    public float fadeDuration = 0.2f;

    private bool isOn = true;
    private bool particlesIsOn = false;
    private float progress = 0;

	// Use this for initialization
	void Start () {
        innerBeam.transform.localScale = new Vector3(0, 0, 1);
        isOn = true;
        progress = 0;
        particlesIsOn = false;
        particles.Stop();
    }
	
	// Update is called once per frame
	void Update () {
	    if (isOn && progress < 1f) {
            progress += Time.deltaTime / fadeDuration;
            progress = Mathf.Min(progress, 1f);
        } else if (!isOn) {
            progress -= Time.deltaTime / fadeDuration;
            if (progress <= 0) {
                particles.transform.SetParent(null);
                particles.transform.localScale = Vector3.one;
                particles.Stop();
                Destroy(particles.gameObject, 2);
                Destroy(gameObject);
            }
        }
        innerBeam.transform.localScale = new Vector3(progress, progress, 1);
        flare1.brightness = progress;
        flare2.brightness = progress;
    }

    public void ParticlesOn() {
        if (!particlesIsOn) {
            particles.Play();
            particlesIsOn = true;
        }
    }

    public void ParticlesOff() {
        if (particlesIsOn) {
            particles.Stop();
            particlesIsOn = false;
        }
    }

    public void Off() {
        isOn = false;
    }
}
