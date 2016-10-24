using UnityEngine;
using System.Collections;

public class StraightLine :  SimpleProjectile {
	public ParticleSystem particles;
	public LensFlare flare;
	public Unit target;
	public GameObject ball;
	private Vector3 targetPosition;
	private Vector3 initialPosition;	

	// Use this for initialization
	public override void Start () {
		base.Start ();
		targetPosition = target.transform.position;
		initialPosition = attacker.transform.position;
		LaunchAt (initialPosition, targetPosition);
	}
	
	// Update is called once per frame
	public override void Update () {
		
		Debug.Log (Vector3.Distance (transform.position, targetPosition));
		if (Vector3.Distance (transform.position, targetPosition) == 0) {
			particles.transform.SetParent (null);
			particles.Stop ();
			Destroy (particles.gameObject, 2);
			flare.transform.SetParent (null);
			Destroy (flare.gameObject, 1);
			Destroy (ball);
			Destroy (gameObject, 1);
		}
	}

	public void updateSpeed(){
		
	}


}
