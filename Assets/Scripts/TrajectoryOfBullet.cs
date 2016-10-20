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
	public float speedInit = 5.0f;
	public float damage = 10.0f;

	protected CharacterController controller;

	private Quaternion targetRotation;
	private Vector3 targetPosition;
	private Vector3 attackerPosition;

	void Start() {
		controller = GetComponent<CharacterController>();
		targetPosition = target.transform.position;
		attackerPosition = attacker.transform.position;
	}

	void Update() {

		Launch ();

	}
		

	private void Launch()
	{
		float distanceToTarget = Vector3.Distance (targetPosition,transform.position);
		if (distanceToTarget <= distanceMax) {
			// Pursuing
			float moveDistance = speedInit * Time.deltaTime;
			if (moveDistance > distanceToTarget) {
				controller.Move (distanceToTarget * (targetPosition - transform.position).normalized);
			} else {
				controller.Move (moveDistance * (targetPosition - transform.position).normalized);
			}

			targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(targetPosition - transform.position, Vector3.up));

		}
	}

	private void AcquireTargetPosition()
	{
		targetPosition = target.transform.position;
	}


}