﻿using UnityEngine;
using System.Collections;

public class LongDistanceAI : WanderingBasicAI {
	
	public float bulletRange = 15f;
	public float bulletSpeed = 8f;


	// Update is called once per frame
	public override void Update () {
		base.Update();
	}

	public override void AttackAction(Unit target) {
		ParabolicProjectile bullet = GameObject.Instantiate(Links.links.StoneProjectile).GetComponent<ParabolicProjectile>();
		bullet.attacker = this;
		bullet.team = team;
		bullet.damage = attackDamage;
		bullet.lifeTime = bulletRange / bulletSpeed;
		bullet.speed = bulletSpeed;

		int mode = Random.Range(0,4);
		switch (mode) {
		case 0:
			bullet.launchMode = ParabolicProjectile.LaunchMode.FixedHeight;
			break;
		case 1:
			bullet.launchMode = ParabolicProjectile.LaunchMode.FixedAngle;
			break;
		case 2:
			bullet.launchMode = ParabolicProjectile.LaunchMode.FixedSpeedHighTrajectory;
			break;
        case 3:
            bullet.launchMode = ParabolicProjectile.LaunchMode.FixedSpeedLowTrajectory;
            break;
        }

		bullet.LaunchAt(transform.position + centerHeight * Vector3.up, target.transform.position + target.centerHeight * Vector3.up);
	}
}
