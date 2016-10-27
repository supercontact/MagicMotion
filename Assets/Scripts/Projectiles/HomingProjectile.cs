using UnityEngine;
using System.Collections;

public class HomingProjectile : Projectile  {

	public Unit target;
	public float speed = 2;
    public float angleSpeed = 90;

    private Vector3 direction;

    public override void Awake() {
		base.Awake();
		body.useGravity = false;
		body.drag = 0;
		body.angularDrag = 0;
	}

	public virtual void FixedUpdate() {
        Vector3 realDir = (target.transform.position + Vector3.up * target.centerHeight - transform.position).normalized;
        float angle = Vector3.Angle(direction, realDir);
        if (angle < angleSpeed * Time.deltaTime) {
            direction = realDir;
        } else {
            direction = Vector3.Slerp(direction, realDir, angleSpeed * Time.deltaTime / angle);
        }
		body.velocity = speed * direction.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
	}

    public virtual void Launch(Unit target, Vector3 initialPosition, Vector3 initialDirection) {
        this.target = target;
        transform.position = initialPosition;
        direction = initialDirection.normalized;
    }

}
