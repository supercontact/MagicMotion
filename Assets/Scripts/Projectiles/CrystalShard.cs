using UnityEngine;
using System.Collections;

/// <summary>
/// The crystal shard is a shiny projectile that has a flare effect and a particle effect attached.
/// </summary>
public class CrystalShard : SimpleProjectile {

    public ParticleSystem particles;
    public LensFlare flare;
    public GameObject crystal;

    public override void Update() {
        base.Update();
        if (ended) {
            flare.brightness -= flare.fadeSpeed * Time.deltaTime;
        }
    }

    public override void EndAction(bool targetHit) {
        particles.transform.SetParent(null);
        particles.Stop();
        Destroy(particles.gameObject, 2);
        flare.transform.SetParent(null);
        Destroy(flare.gameObject, 1);
        Destroy(crystal);
        Destroy(gameObject, 1);
    }
}
