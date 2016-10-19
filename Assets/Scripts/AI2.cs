using UnityEngine;
using System.Collections;

public class AI2 : BasicAI {
	private float lastThinkTime;

	// Use this for initialization
	public override void Start () {
		base.Start ();
	}

	// Update is called once per frame
	public override void Update () {
		base.Update();
	}

	public override void PreAttackAction(Unit target) {

	}
	public override void AttackAction(Unit target) {
		ReceiveDamage(30, target);
	}

	public override void DieAction() {

	}

}
