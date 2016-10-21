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
        projectile = GameObject.Instantiate(PrefabLinks.links.grabbingProjectile).GetComponent<GrabbingProjectile>();
        projectile.relatedAttack = this;
        projectile.range = projectileRange;
        projectile.triggerRadius = projectileRadius;
        projectile.velocity = self.transform.forward * projectileSpeed;
        projectile.transform.position = self.transform.position + 0.5f * self.transform.forward + 0.5f * Vector3.up;
        projectile.transform.rotation = self.transform.rotation;
        isActive = true;
    }

    public override void Interrupt() {
        if (isActive) {
            if (projectile != null) {
                projectile.Cancel();
                projectile = null;
            }
            if (grabbedTarget != null) {
                Vector3 flyingVelocity = self.transform.TransformVector(LeapControl.smoothedHandSpeed * movingFactor);
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
            self.ForceStop();
            Interrupt();
            return;
        }
        grabbedTarget = target;
        lockEffect = GameObject.Instantiate(PrefabLinks.links.lockEffect);
        lockEffect.transform.SetParent(grabbedTarget.transform, false);
    }

    public void Miss() {
        projectile = null;
        self.ForceStop();
        Interrupt();
    }

    private void Update() {
        Vector3 handPoint = LeapControl.handPoint;
        if (isActive && grabbedTarget != null) {
            if (grabbedTarget.state != Unit.State.Controlled) {
                grabbedTarget.Control();
            }
            grabbedTarget.controller.Move(self.transform.TransformVector(handPoint - prevHandPoint) * movingFactor);
        }
        prevHandPoint = handPoint;
    }

    private void ReleaseGestureTriggered() {
        if (isActive) {
            self.ForceStop();
            Interrupt();
        }
    }

}
