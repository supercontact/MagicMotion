﻿using UnityEngine;
using System.Collections;

/// <summary>
/// A projectile that is pulled by gravity.
/// It provides 4 launch modes which behave differently when launched at targets at different distances.
/// </summary>
public class ParabolicProjectile : SimpleProjectile
{

    public enum LaunchMode
    {
        FixedHeight,
        FixedAngle,
        FixedSpeedHighTrajectory,
        FixedSpeedLowTrajectory
    }

    public Unit target;
    public float defaultSpeed = 10;
    public float defaultAngle = 45;
    public float defaultHeight = 3;
    public float gravity = 10f;
    public LaunchMode launchMode = LaunchMode.FixedHeight;

    private Vector3 speedVec;

    public override void Start() {
        base.Start();
        acceleration = Vector3.down * gravity;
    }

    public void LaunchWithFixedHeight(Vector3 initialPosition, Vector3 targetPosition) {
        float distanceY = initialPosition.y + defaultHeight - targetPosition.y;
        Vector3 directionX = Vector3.ProjectOnPlane(targetPosition - initialPosition, Vector3.up);
        float distanceX = directionX.magnitude;

        float t1 = Mathf.Sqrt((2 * defaultHeight / gravity));
        float t2 = Mathf.Sqrt((2 * distanceY / gravity));
        float speedX = distanceX / (t1 + t2);
        float speedY = gravity * t1;
        speedVec = directionX.normalized * speedX + speedY * Vector3.up;
        speed = speedVec.magnitude;
        Launch(initialPosition, speedVec);
    }

    public void LaunchWithFixedAngle(Vector3 initialPosition, Vector3 targetPosition) {
        float cosalphe = Mathf.Cos(defaultAngle * Mathf.Deg2Rad);
        float sinalphe = Mathf.Sin(defaultAngle * Mathf.Deg2Rad);
        Vector3 directionX = Vector3.ProjectOnPlane(targetPosition - initialPosition, Vector3.up);
        float distanceX = directionX.magnitude;

        float param1 = (gravity * distanceX * distanceX) / (2 * cosalphe * cosalphe);
        float param2 = (distanceX * sinalphe / cosalphe - initialPosition.y);
        speed = Mathf.Sqrt(param1 / param2);
        speedVec = directionX.normalized * speed * cosalphe + speed * sinalphe * Vector3.up;
        Launch(initialPosition, speedVec);
    }

    public void LaunchWithFixedSpeed(Vector3 initialPosition, Vector3 targetPosition, bool lowerTrajectory = true) {
        Vector3 directionX = Vector3.ProjectOnPlane(targetPosition - initialPosition, Vector3.up);
        float distanceX = directionX.magnitude;

        float param1 = gravity * distanceX * distanceX / (defaultSpeed * defaultSpeed) + initialPosition.y;
        float param2 = Mathf.Sqrt(distanceX * distanceX + initialPosition.y * initialPosition.y);
        float angleTotal = lowerTrajectory ? Mathf.Asin(param1 / param2) : Mathf.PI - Mathf.Asin(param1 / param2);
        float beta = Mathf.Atan2(-initialPosition.y, distanceX);
        float angle = (angleTotal - beta) / 2;
        float cosAlpha = Mathf.Cos(angle);
        float sinAlpha = Mathf.Sin(angle);
        speed = defaultSpeed;
        speedVec = directionX.normalized * speed * cosAlpha + speed * sinAlpha * Vector3.up;
        Launch(initialPosition, speedVec);
    }

    public override void LaunchAt(Vector3 initialPosition, Vector3 targetPosition) {
        switch (launchMode) {
        case LaunchMode.FixedHeight:
            LaunchWithFixedHeight(initialPosition, targetPosition);
            break;
        case LaunchMode.FixedAngle:
            LaunchWithFixedAngle(initialPosition, targetPosition);
            break;
        case LaunchMode.FixedSpeedHighTrajectory:
            LaunchWithFixedSpeed(initialPosition, targetPosition, false);
            break;
        case LaunchMode.FixedSpeedLowTrajectory:
            LaunchWithFixedSpeed(initialPosition, targetPosition, true);
            break;
        default:
            Launch(initialPosition, targetPosition);
            break;
        }

    }

}
