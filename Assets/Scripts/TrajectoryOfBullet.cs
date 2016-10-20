using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrajectoryOfBullet: MonoBehaviour
{
	public Unit target;
	public Unit attacker;
	public int team = 0;
	public float distanceMax = 10.0f;
	public float distanceMin = 0.0f;
	public float speedInit = 1.0f;
	public float damage = 10.0f;
	public float decayTimer = 2.0f;

	protected CharacterController controller;

	private Vector3 targetPosition;
	private Vector3 positionInit;
	private bool isAttacked = false;
	private float distanceToTarget;
	private float remainingDistance;

	void Start() {
		controller = GetComponent<CharacterController>();
		targetPosition = target.transform.position;
		positionInit = attacker.transform.position;
		distanceToTarget = Vector3.Distance (targetPosition,positionInit);
		remainingDistance = distanceToTarget < distanceMax ? distanceToTarget : distanceMax;
	}

	void Update() {
		if (team != target.team) {
			Launch ();
		}
		if (isAttacked) {			
			decayTimer -= Time.deltaTime;
			if (decayTimer <= 0) {
				Destroy (gameObject);
				return;
			}
		}

	}
		

	private void Launch()
	{
		
		if (distanceToTarget <= distanceMax) {
			// Pursuing
			float moveDistance = speedInit * Time.deltaTime;
			if (remainingDistance <= moveDistance) {
				controller.Move (remainingDistance * (targetPosition - positionInit).normalized);
				remainingDistance = 0;
				transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
				isAttacked = true;
			} else {
				controller.Move (moveDistance * (targetPosition - positionInit).normalized);
				remainingDistance -= moveDistance;
			}
			if (remainingDistance == 0) {
				controller.Move(5 * Vector3.down * Time.deltaTime * Time.deltaTime);
			}

		} else {
			float moveDistance = speedInit * Time.deltaTime;
			if (remainingDistance <= moveDistance) {
				controller.Move (remainingDistance * (targetPosition - positionInit).normalized);
				remainingDistance = 0;
				transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
				isAttacked = true;
			} else {
				controller.Move (moveDistance * (targetPosition - positionInit).normalized);
				remainingDistance -= moveDistance;
			}
			if (remainingDistance == 0) {
				controller.Move(5 * Vector3.down * Time.deltaTime * Time.deltaTime);
			}
		}

		
	}

	private void AcquireTargetPosition()
	{
		targetPosition = target.transform.position;
	}


}