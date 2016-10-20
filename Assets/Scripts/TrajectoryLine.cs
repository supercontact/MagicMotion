using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrajectoryLine: MonoBehaviour
{
	public Unit target;
	public float distanceMax = 10.0f;
	public float distanceMin = 0.0f;
//	public float power = 5;
	public float speedInit = 5.0f;
	public float freezeTime = 1.0f;

	protected CharacterController controller;

	private bool targetReady = false;
	private bool isPressed = false;
	private bool isBallThrown = false;
	private float waitTime = 0.0f;
	private float currentMoveSpeed = 0;
	private Quaternion targetRotation;
	Vector3 targetPosition;

	void Start() {

		targetPosition = target.transform.position;
	}

	void Update() {
		
		if (isBallThrown) {
			return;
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			isPressed = true;
		}
		if(isPressed) {

			if (targetReady) {
				AcquireTargetPosition ();
				Launch ();
			}
		}
		waitTime += Time.deltaTime;
		if (waitTime >= freezeTime) {
			targetReady = true;
			waitTime = 0;
		}

	}
		

	private void Launch()
	{
		Debug.Log(" ball");
		float distanceToTarget = Vector3.Distance (targetPosition,transform.position);
		if (distanceToTarget <= distanceMax) {
			// Pursuing
			currentMoveSpeed = speedInit;
			float moveDistance = currentMoveSpeed * Time.deltaTime;
			if (moveDistance > distanceToTarget) {
				controller.Move (distanceToTarget * (targetPosition - transform.position).normalized);
				isPressed = false;
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