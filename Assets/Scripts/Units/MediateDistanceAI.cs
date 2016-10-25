using UnityEngine;
using System.Collections;

public class MediateDistanceAI : WanderingBasicAI {

	public float bulletRange = 15f;
	public float bulletSpeed = 8f;


	// Update is called once per frame
	public override void Update () {
		base.Update();
	}

	public override void AttackAction(Unit target) {
        SimpleProjectile bullet = Instantiate(Links.links.SmallBullet).GetComponent<SimpleProjectile>();
		bullet.attacker = this;
		bullet.team = team;
		bullet.damage = attackDamage;
		bullet.lifeTime = bulletRange / bulletSpeed;
		bullet.speed = bulletSpeed;

		bullet.LaunchAt(transform.position + centerHeight * Vector3.up, target.transform.position + target.centerHeight * Vector3.up);
	}
}
