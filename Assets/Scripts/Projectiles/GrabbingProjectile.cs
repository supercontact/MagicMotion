using UnityEngine;
using System.Collections;

public class GrabbingProjectile : SimpleProjectile {

    public Grabbing relatedAttack;

    public SpriteRenderer sprite;

    private float endTimer = 0;
    private float endDuration = 0.3f;
    private LensFlare flare;
    private float flarePower;

	// Use this for initialization
	public override void Start () {
        base.Start();
        flare = GetComponent<LensFlare>();
        flarePower = flare.brightness;
        damage = 0;
        team = -1;
    }

    // Update is called once per frame
    public override void Update () {
        base.Update();
        if (!ended) {
            if (lifeTimer > lifeTime * 0.8f) {
                float alpha = (lifeTime - lifeTimer) / lifeTime / 0.2f;
                sprite.color = new Color(1, 1, 1, alpha);
                flare.brightness = flarePower * alpha;
            }
        } else {
            float alpha = 1 - endTimer / endDuration;
            float scale = 1 + 2 * endTimer / endDuration;
            sprite.color = new Color(1, 1, 1, alpha);
            flare.brightness = flarePower * alpha;
            transform.localScale = new Vector3(scale, scale, scale);
            endTimer += Time.deltaTime;
            if (endTimer > endDuration) {
                Destroy(gameObject);
            }
        }
    }

    public override bool HitAction(Unit target, Collision collision) {
        if (target != null) {
            relatedAttack.Catch(target);
        }
        return true;
    }

    public override void EndAction(bool targetHit) {
        if (targetHit) {
            body.detectCollisions = false;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        } else {
            Destroy(gameObject);
        }
    }
}
