using UnityEngine;
using System.Collections;

/// <summary>
/// An enemy that throw rapid small projectiles at the player. Very fast but not very strong.
/// </summary>
public class MediumRangeEnemy : WanderingBasicAI {

	public float bulletRange = 15f;
	public float bulletSpeed = 8f;

	// Update is called once per frame
	public override void Update () {
		base.Update();
	}

	public override void AttackAction(Unit target) {
        SimpleProjectile bullet = Instantiate(Links.links.smallBullet).GetComponent<SimpleProjectile>();
		bullet.Attacker = this;
		bullet.team = team;
		bullet.damage = attackDamage;
		bullet.lifeTime = bulletRange / bulletSpeed;
		bullet.speed = bulletSpeed;

		bullet.LaunchAt(transform.position + centerHeight * Vector3.up, target.transform.position + target.centerHeight * Vector3.up);
	}
}
