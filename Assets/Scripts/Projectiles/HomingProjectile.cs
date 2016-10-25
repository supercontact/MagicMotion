using UnityEngine;
using System.Collections;

public class HomingProjectile : Projectile  {
	private Vector3 direction;

	public Unit target;
	public float speed = 2;
	public float timer = 2f;
	public Vector3 initialDirection;

	public override void Awake() {
		base.Awake();
		body.useGravity = false;
		body.drag = 0;
		body.angularDrag = 0;
	}
		
	// Use this for initialization
	public override void Start () {
		base.Start ();
		timer = 2f;
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
		if (timer >  Time.deltaTime) {
			timer -= Time.deltaTime;
		} else {
			timer = 0;
		}
//		Debug.Log ("direction"+initialDirection*timer+"time"+timer);
		FixedUpdate ();
	}

	public virtual void FixedUpdate() {
		
		direction = target.transform.position - transform.position + Vector3.up * target.centerHeight + initialDirection * timer;
		body.velocity = speed * direction.normalized;

	}

}
