using UnityEngine;
using System.Collections.Generic;

public class GrabbingProjectile : Projectile {

    public Grabbing relatedAttack;

    public SpriteRenderer sprite;
    public float speed = 20;
    public float attractMaxAngle = 90;
    public float attractRadiusOfCurvature = 5;

    private Unit target;
    private float endTimer = 0;
    private float endDuration = 0.3f;
    private LensFlare flare;
    private float flarePower;

	// Use this for initialization
	public override void Awake () {
        base.Awake();
        body.useGravity = false;
        body.drag = 0;
        body.angularDrag = 0;
        flare = GetComponent<LensFlare>();
        flarePower = flare.brightness;
        damage = 0;
        team = -1;
    }

    public virtual void FixedUpdate() {
        if (!ended && target != null) {
            float distance;
            float radiusOfCurvature;
            Vector3 rotationAxis;
            Trajectory(target.transform.position + target.centerHeight * Vector3.up, out distance, out radiusOfCurvature, out rotationAxis);
            transform.rotation = Quaternion.AngleAxis(speed * Time.fixedDeltaTime / radiusOfCurvature * Mathf.Rad2Deg, rotationAxis) * transform.rotation;
            body.velocity = transform.forward * speed;
        }
    }

    // Update is called once per frame
    public override void Update () {
        base.Update();
        if (!ended) {
            if (target == null) {
                Unit unit = FindTarget();
                if (unit != null) {
                    target = unit;
                }
            }
            if (lifeTimer > lifeTime * 0.6f) {
                float alpha = (lifeTime - lifeTimer) / lifeTime / 0.4f;
                Debug.Log(alpha);
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

    public Unit FindTarget() {
        float maxRange = attractMaxAngle * Mathf.Deg2Rad * attractRadiusOfCurvature;
        List<Unit> unitsInRange = Unit.UnitsInRange(transform.position, maxRange, false);
        Unit closestUnit = null;
        float closestDistance = float.MaxValue;
        foreach (Unit unit in unitsInRange) {
            if (IsReachable(unit.transform.position + unit.centerHeight * Vector3.up)) {
                float distance = Vector3.Distance(unit.transform.position + unit.centerHeight * Vector3.up, transform.position);
                if (distance < closestDistance) {
                    closestUnit = unit;
                    closestDistance = distance;
                }
            }
        }
        return closestUnit;
    }

    public bool IsReachable(Vector3 position) {
        float distance;
        float radiusOfCurvature;
        Vector3 rotationAxis;
        Trajectory(position, out distance, out radiusOfCurvature, out rotationAxis);
        return Vector3.Dot(position - transform.position, transform.forward) > 0 && 
            distance <= attractMaxAngle * Mathf.Deg2Rad * attractRadiusOfCurvature && 
            radiusOfCurvature <= attractRadiusOfCurvature;
    }

    public void Trajectory(Vector3 position, out float distance, out float radiusOfCurvature, out Vector3 rotationAxis) {
        Vector3 relativePosition = transform.InverseTransformDirection(position - transform.position);
        Vector2 targetPosition = new Vector2(Mathf.Sqrt(relativePosition.x * relativePosition.x + relativePosition.y * relativePosition.y), relativePosition.z);
        float angle = Vector2.Angle(Vector2.up, targetPosition) * Mathf.Deg2Rad;
        distance = angle == 0 ? targetPosition.y : targetPosition.magnitude * angle / Mathf.Sin(angle);
        radiusOfCurvature = angle == 0 ? float.PositiveInfinity : targetPosition.magnitude / (2 * Mathf.Sin(angle));
        rotationAxis = Vector3.Cross(transform.forward, position - transform.position).normalized;
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

    public virtual void Launch(Vector3 initialPosition, Vector3 direction) {
        transform.position = initialPosition;
        transform.rotation = Quaternion.LookRotation(direction);
        body.velocity = speed * direction.normalized;
    }
}
