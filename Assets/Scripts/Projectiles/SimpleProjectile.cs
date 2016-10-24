using UnityEngine;
using System.Collections;

public class SimpleProjectile : Projectile {

    public float speed = 10;
    public Vector3 acceleration;

    public override void Awake() {
        base.Awake();
        body.useGravity = false;
        body.drag = 0;
        body.angularDrag = 0;
    }

    public virtual void FixedUpdate() {
        body.AddForce(acceleration, ForceMode.Acceleration);
    }

    public virtual void Launch(Vector3 initialPosition, Vector3 direction) {
        transform.position = initialPosition;
        transform.rotation = Quaternion.LookRotation(direction);
        body.velocity = speed * direction.normalized;
    }
    public virtual void LaunchAt(Vector3 initialPosition, Vector3 targetPosition) {
        Launch(initialPosition, targetPosition - initialPosition);
    }

}
