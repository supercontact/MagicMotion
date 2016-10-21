using UnityEngine;
using System.Collections;

public class GrabbingProjectile : MonoBehaviour {

    public Grabbing relatedAttack;
    public Vector3 velocity;
    public float triggerRadius;
    public float range;

    public SpriteRenderer sprite;

    private float distance = 0;
    private bool cancelled = false;
    private float cancelTimer = 0;
    private float cancelDuration = 0.3f;
    private LensFlare flare;
    private float flarePower;

	// Use this for initialization
	void Start () {
        flare = GetComponent<LensFlare>();
        flarePower = flare.brightness;
    }
	
	// Update is called once per frame
	void Update () {
        if (!cancelled) {
            transform.position += velocity * Time.deltaTime;
            distance += velocity.magnitude * Time.deltaTime;
            if (distance > range * 0.8f) {
                float alpha = (range - distance) / range / 0.2f;
                sprite.color = new Color(1, 1, 1, alpha);
                flare.brightness = flarePower * alpha;
            }
            if (distance > range) {
                relatedAttack.Miss();
                Cancel(false);
                return;
            }

            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, triggerRadius);
            foreach (Collider c in collidersInRange) {
                Unit unitInRange = c.GetComponent<Unit>();
                if (unitInRange != null && unitInRange != relatedAttack.self) {
                    relatedAttack.Catch(unitInRange);
                    Cancel(true);
                    return;
                }
            }
            if (collidersInRange.Length > 0) {
                Cancel(true);
                return;
            }
        } else {
            float alpha = 1 - cancelTimer / cancelDuration;
            float scale = 1 + 2 * cancelTimer / cancelDuration;
            sprite.color = new Color(1, 1, 1, alpha);
            flare.brightness = flarePower * alpha;
            transform.localScale = new Vector3(scale, scale, scale);
            cancelTimer += Time.deltaTime;
            if (cancelTimer > cancelDuration) {
                Destroy(gameObject);
            }
        }
    }

    public void Cancel(bool effectOn = true) {
        if (!effectOn) {
            Destroy(gameObject);
        } else {
            cancelled = true;
        }
    }
}
