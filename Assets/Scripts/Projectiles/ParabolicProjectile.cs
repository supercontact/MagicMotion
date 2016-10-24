using UnityEngine;
using System.Collections;

public class ParabolicProjectile : SimpleProjectile {
	public float defaultSpeed = 10;
	public float defaultAngle = 45;
	public float defaultHeight = 3;
	public float g = 9.8f;
	public int launchMode = 0;

	public void LaunchWithFixedHeight(Vector3 initialPosition ,Vector3 targetPosition){
		float distanceY = initialPosition.y + defaultHeight - targetPosition.y;
		Vector3 directionX = Vector3.ProjectOnPlane (targetPosition - initialPosition, Vector3.up);
		float distanceX = Vector3.ProjectOnPlane (targetPosition - initialPosition, Vector3.up).magnitude;
		float t1 = Mathf.Sqrt ((2 * defaultHeight / g));
		float t2 = Mathf.Sqrt ((2 * distanceY / g));
		float speedX = distanceX / (t1 + t2);
		float speedY = g * t1;
		Vector3 speedVec = directionX.normalized * speedX + speedY * Vector3.up;
		speed = speedVec.magnitude;
		Launch (initialPosition, speedVec);
			
	}

	public void LaunchWithFixedAngle(Vector3 initialPosition ,Vector3 targetPosition){
		float cosalphe = Mathf.Cos(defaultAngle * Mathf.Deg2Rad);
		float sinalphe = Mathf.Sin(defaultAngle * Mathf.Deg2Rad);
		Vector3 directionX = Vector3.ProjectOnPlane (targetPosition - initialPosition, Vector3.up);
		float distanceX = Vector3.ProjectOnPlane (targetPosition - initialPosition, Vector3.up).magnitude;

		float param1 = (g * distanceX * distanceX) / (2 * cosalphe * cosalphe);
		float param2 = (distanceX * sinalphe / cosalphe - initialPosition.y);
		speed = Mathf.Sqrt(param1 / param2);
		Vector3 speedVec = directionX.normalized * speed * cosalphe + speed * sinalphe * Vector3.up;
		Launch (initialPosition, speedVec);
	}

	public void LaunchWithFixedSpeed(Vector3 initialPosition ,Vector3 targetPosition){
		Vector3 directionX = Vector3.ProjectOnPlane (targetPosition - initialPosition, Vector3.up);
		float distanceX = Vector3.ProjectOnPlane (targetPosition - initialPosition, Vector3.up).magnitude;

		float param1 = g * distanceX * distanceX / (defaultSpeed * defaultSpeed) + initialPosition.y;
		float param2 = Mathf.Sqrt (distanceX * distanceX + initialPosition.y * initialPosition.y);
		float angleTotal = Mathf.Asin (param1 / param2);
		float belta = Mathf.Asin (-initialPosition.y / param2);
		float angle = (angleTotal - belta);
		float cosalphe = Mathf.Cos(angle);
		float sinalphe = Mathf.Sin(angle);
		speed = defaultSpeed;
		Vector3 speedVec = directionX.normalized * speed * cosalphe + speed * sinalphe * Vector3.up;
		Launch (initialPosition, speedVec);
	}

	public override void LaunchAt (Vector3 initialPosition, Vector3 targetPosition)
	{
		base.LaunchAt (initialPosition, targetPosition);
		switch (launchMode) {
		case 0:
			LaunchWithFixedHeight (initialPosition, targetPosition);
			break;
		case 1:
			LaunchWithFixedAngle (initialPosition, targetPosition);
			break;
		case 2:
			LaunchWithFixedSpeed (initialPosition, targetPosition);
			break;
		default:
			Launch (initialPosition, targetPosition);
			break;		
		}

	}
}
