using UnityEngine;
using System.Collections;

public class HomingProjectile : Projectile  {
	private Vector3 direction;

	public Unit target;
	public float speed = 2;

	public override void Awake() {
		base.Awake();
		body.useGravity = false;
		body.drag = 0;
		body.angularDrag = 0;
	}
		
	// Use this for initialization
	public override void Start () {
	
	}
	
	// Update is called once per frame
	public override void Update () {
	
	}

	public virtual void FixedUpdate() {
		direction = target.transform.position - transform.position + Vector3.up * target.centerHeight;
		body.velocity = speed * direction.normalized;

	}
}
