using UnityEngine;
using System.Collections;

public class Grabbing : SpecialAttack {

    public float projectileSpeed = 20;
    public float projectileRange = 10;
    public float projectileRadius = 0.5f;
    public float movingFactor = 0.005f;
    public float flyingSpeedLimit = 10;

    public Unit grabbedTarget = null;
    public bool isActive;

    private GrabbingProjectile projectile;
    private GameObject lockEffect;
    private Vector3 prevHandPoint;
    private Vector3 relativeTargetPosition;

	public Grabbing() {
        attackPeriod = 0;
        attackDelay = 0;
        lasting = true;
        UpdateSender.OnUpdate += Update;
        GrabDetector.OnRelease += ReleaseGestureTriggered;
        isActive = false;
    }

    public override bool IsUsableNow() {
        return true;
    }

    public override void AttackAction() {
        projectile = GameObject.Instantiate(Links.links.grabbingProjectile).GetComponent<GrabbingProjectile>();
        projectile.relatedAttack = this;
        projectile.attacker = attacker;
        projectile.lifeTime = projectileRange / projectileSpeed;
        projectile.speed = projectileSpeed;
        projectile.Launch(attacker.transform.position + 0.5f * attacker.transform.forward + 0.5f * Vector3.up, attacker.transform.forward);
        isActive = true;
    }

    public override void Interrupt() {
        if (isActive) {
            if (projectile != null) {
                projectile.Cancel(true);
                projectile = null;
            }
            if (grabbedTarget != null) {
                Vector3 flyingVelocity = attacker.transform.TransformVector(LeapControl.smoothedHandSpeed * movingFactor);
                if (flyingVelocity.magnitude > flyingSpeedLimit) {
                    flyingVelocity = flyingVelocity.normalized * flyingSpeedLimit;
                }
                grabbedTarget.SendFlying(flyingVelocity);
                grabbedTarget = null;
                GameObject.Destroy(lockEffect);
                lockEffect = null;
            }
            isActive = false;
        }
    }

    public void Catch(Unit target) {
        projectile = null;
        if (target.isImmuneToControl) {
            attacker.ForceStop();
            Interrupt();
            return;
        }
        grabbedTarget = target;
        relativeTargetPosition = attacker.transform.InverseTransformPoint(target.transform.position);
        lockEffect = GameObject.Instantiate(Links.links.lockEffect);
        lockEffect.transform.SetParent(grabbedTarget.transform, false);
    }

    public void Miss() {
        projectile = null;
        attacker.ForceStop();
        Interrupt();
    }

    private void Update() {
        Vector3 handPoint = LeapControl.handPoint;
        if (isActive && grabbedTarget != null) {
            if (grabbedTarget.state != Unit.State.Controlled) {
                grabbedTarget.Control();
            }
            relativeTargetPosition += (handPoint - prevHandPoint) * movingFactor;
            grabbedTarget.controller.Move(attacker.transform.TransformPoint(relativeTargetPosition) - grabbedTarget.transform.position);
            relativeTargetPosition = attacker.transform.InverseTransformPoint(grabbedTarget.transform.position);
        }
        prevHandPoint = handPoint;
    }

    private void ReleaseGestureTriggered() {
        if (isActive) {
            attacker.ForceStop();
            Interrupt();
        }
    }

}
