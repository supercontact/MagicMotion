using UnityEngine;
using System.Collections;
using System;

public class Parabola : MonoBehaviour {
	public Unit target;
	public Unit attacker;
	public int team = 0;
//	public float distanceMax = 10.0f;
//	public float distanceMin = 0.0f;
	public double speedInit = 1.0f;
	public float damage = 10.0f;
	public float decayTimer = 2.0f;
	public double angleDeg = 30;



	protected CharacterController controller;

	private Vector3 targetPosition;
	private Vector3 positionInit;
	private bool isAttacked = false;
	private float distanceToTarget;
	private float remainingDistance;
	private float distanceMaxX;
	private float distanceMin = 0.0f;

	private double angle; 

	private double speedX;
	private double speedY;
	private double g = 10;
	private float distanceMaxY;
	private float flyTime;
	private float currentTime;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		targetPosition = target.transform.position;
		positionInit = new Vector3(transform.position.x,0,transform.position.z);
		distanceToTarget = Vector3.Distance (targetPosition,positionInit);
		angle    = Math.PI * angleDeg / 180.0;
		speedX = Math.Cos (angle) * speedInit;
		speedY = Math.Sin (angle) * speedInit;
		distanceMaxY = attacker.transform.localScale.y + 0.5f * (float)(speedY * speedY / g);
		flyTime = (float)(speedY / g + Math.Sqrt (2 * distanceMaxY / g));
		distanceMaxX = (float)speedX * flyTime;
		remainingDistance = distanceToTarget < distanceMaxX ? distanceToTarget : distanceMaxX;
		currentTime = 0;

		Debug.Log ("distanceMAx "+distanceMaxX + "distanceToTarget"+distanceToTarget);

	}
	
	// Update is called once per frame
	void Update () {
		
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
		currentTime += Time.deltaTime;
	}


	private void Launch()
	{

		if (distanceToTarget <= distanceMaxX) {
			// Pursuing

			float moveDistanceX = (float)speedX * Time.deltaTime;
			float moveDistanceY = (float)( -g * currentTime * Time.deltaTime - 0.5 * g * Time.deltaTime * Time.deltaTime + speedX * Time.deltaTime);
			if (remainingDistance > 0) {
				if (remainingDistance <= moveDistanceX) {
					controller.Move (moveDistanceY * Vector3.down + remainingDistance * (targetPosition - positionInit).normalized);
					remainingDistance = 0;
					transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
					isAttacked = true;
					Debug.Log (remainingDistance);
					Debug.Log ("movedistanceY" + moveDistanceY);
				} else {
					controller.Move (moveDistanceY * Vector3.down + moveDistanceX * (targetPosition - positionInit).normalized);
					remainingDistance -= moveDistanceX;
					Debug.Log ("movedistanceY" + moveDistanceY);
				}
			}
			else  {
				controller.Move (  5 * Time.deltaTime * Time.deltaTime * Vector3.down);
				return;
			}


		} else {
			if (remainingDistance == 0) {
				controller.Move(5 * Vector3.down * Time.deltaTime * Time.deltaTime);
				return;
			}
			float moveDistanceX = (float)speedX * Time.deltaTime;
			float moveDistanceY = (float)( -g * currentTime * Time.deltaTime - 0.5 * g * Time.deltaTime * Time.deltaTime + speedX * Time.deltaTime);
			if (remainingDistance <= moveDistanceX) {
				controller.Move (moveDistanceY * Vector3.down + remainingDistance * (targetPosition - positionInit).normalized);
				remainingDistance = 0;
				transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
				isAttacked = true;
			} else {
				controller.Move (moveDistanceY * Vector3.down + moveDistanceX * (targetPosition - positionInit).normalized);
				remainingDistance -= moveDistanceX;
			}

		}


	}
}
