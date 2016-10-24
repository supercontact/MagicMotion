using UnityEngine;
using System.Collections;

public class BigAI : WanderingBasicAI {
	
	public float bulletRange = 15f;
	public float bulletSpeed = 8f;

	// Use this for initialization
	public override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update ();
	}

	public override void DieAction() {
		base.DieAction ();
		BasicAI smallAI1 = Unit.Instantiate(Links.links.SmallAI,transform.position + Vector3.left * 2,transform.rotation);
//		BasicAI smallAI2 = GameObject.Instantiate(Links.links.Bullet,transform.position + Vector3.right * 2).GetComponent<BasicAI>();
//		BasicAI smallAI3 = GameObject.Instantiate(Links.links.Bullet,transform.position + Vector3.forward * 2).GetComponent<BasicAI>();
//		BasicAI smallAI4 = GameObject.Instantiate(Links.links.Bullet,transform.position + Vector3.back * 2).GetComponent<BasicAI>();
	}

}
