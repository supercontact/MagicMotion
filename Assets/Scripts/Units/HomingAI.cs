using UnityEngine;
using System.Collections;

public class HomingAI : WanderingBasicAI {

	public float bulletRange = 10f;
	public float bulletSpeed = 8f;
	private Vector3[] initDirect = new Vector3[3];


	public override void AttackAction(Unit target) {
		initDirect [0] = Vector3.left;
		initDirect [1] = Vector3.right;
		initDirect [2] = Vector3.up;
		HomingProjectile[] bullets = new HomingProjectile[3];

		for (int i = 0; i < 3; i++) {
			bullets [i] = GameObject.Instantiate (Links.links.HomingBullet).GetComponent<HomingProjectile> ();
			bullets[i].initialDirection = initDirect[i];
			bullets[i].attacker = this;
			bullets[i].team = team;
			bullets[i].damage = attackDamage;
			bullets[i].lifeTime = bulletRange / bulletSpeed;
			bullets[i].speed = bulletSpeed;
			bullets[i].target = target;
			bullets[i].transform.position = this.transform.position + this.centerHeight * Vector3.up + initDirect[i];
		}

	}
}
