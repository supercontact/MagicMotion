using UnityEngine;
using System.Collections;

public class HomingEnemy : WanderingBasicAI {

	public float bulletRange = 10f;
	public float bulletSpeed = 8f;

	private Vector3[] bulletDirections = new Vector3[] { Vector3.left, Vector3.up, Vector3.right };


	public override void AttackAction(Unit target) {
		for (int i = 0; i < bulletDirections.Length; i++) {
			HomingProjectile bullet = GameObject.Instantiate (Links.links.homingBullet).GetComponent<HomingProjectile> ();
			bullet.Attacker = this;
			bullet.team = team;
			bullet.damage = attackDamage;
			bullet.lifeTime = bulletRange / bulletSpeed;
			bullet.speed = bulletSpeed;
            bullet.Launch(target, transform.position + Vector3.up * centerHeight + transform.TransformVector(bulletDirections[i]) * 0.5f, transform.TransformVector(bulletDirections[i]));
		}

	}
}
