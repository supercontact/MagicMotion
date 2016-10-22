using UnityEngine;
using System.Collections;

public class SimpleProjectile : Projectile {

    public float speed = 10;
    public Vector3 acceleration;

    public override void Start() {
        base.Start();
        body.useGravity = false;
        body.drag = 0;
        body.angularDrag = 0;
    }

    public virtual void FixedUpdate() {
        body.AddForce(acceleration, ForceMode.Acceleration);
    }

    public void LaunchAt(Vector3 initialPosition, Vector3 targetPosition) {
        transform.position = initialPosition;
        body.velocity = speed * (targetPosition - initialPosition).normalized;
    }

}
