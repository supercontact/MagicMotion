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
		WanderingBasicAI smallAI1 = Instantiate(Links.links.SmallAI).GetComponent<WanderingBasicAI>();
		smallAI1.transform.position = transform.position + Vector3.left * 1;
		smallAI1.transform.rotation = transform.rotation;
		WanderingBasicAI smallAI2 = GameObject.Instantiate(Links.links.SmallAI).GetComponent<WanderingBasicAI>();
		smallAI2.transform.position = transform.position + Vector3.right * 1;
		smallAI2.transform.rotation = transform.rotation;
		WanderingBasicAI smallAI3 = GameObject.Instantiate(Links.links.SmallAI).GetComponent<WanderingBasicAI>();
		smallAI3.transform.position = transform.position + Vector3.forward * 1;
		smallAI3.transform.rotation = transform.rotation;
		WanderingBasicAI smallAI4 = GameObject.Instantiate(Links.links.SmallAI).GetComponent<WanderingBasicAI>();
		smallAI4.transform.position = transform.position + Vector3.back * 1;
		smallAI4.transform.rotation = transform.rotation;
	}

}
