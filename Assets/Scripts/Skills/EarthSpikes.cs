﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A spell that summons earth spikes in a small area, causing area damage. Useful to deal with tight groups of enemy.
/// After casting the spell, the target position can be further adjusted by finger movement.
/// </summary>
public class EarthSpikes : SpecialAttack {

    public int damage = 50;
    public float radius = 2;
    public float impulse = 3;
    public float initialDistance = 5;
    public float movingFactor = 0.005f;


    private bool aiming = false;
    private float startTime;
    private AimMarker marker;
    private Vector3 prevFingerPoint;
    private Vector3 relativeTargetPosition;

    public EarthSpikes() {
        attackPeriod = 2f;
        attackDelay = 1.5f;
        UpdateSender.OnUpdate += Update;
    }

    public override bool IsUsableNow() {
        return true;
    }
    public override void PreAttackAction() {
        relativeTargetPosition = Vector3.forward * initialDistance;
        aiming = true;
        startTime = Time.time;
        marker = GameObject.Instantiate(Links.links.aimMarker).GetComponent<AimMarker>();
        marker.transform.position = attacker.transform.TransformPoint(relativeTargetPosition);
        OverlayDisplay.Show(Links.links.spikeImage, 0, 0.5f);
    }
    public override void AttackAction() {
        Vector3 position = attacker.transform.TransformPoint(relativeTargetPosition);
        List<Unit> enemiesInRange = Unit.EnemiesInRange(position, radius, attacker.team);
        foreach (Unit enemie in enemiesInRange) {
            enemie.ReceiveDamage(damage, attacker);
            enemie.SendFlying(Vector3.up * impulse);
        }
        aiming = false;
        GameObject.Destroy(marker.gameObject);
        GameObject spikes = GameObject.Instantiate(Links.links.spikes);
        spikes.transform.position = position;
    }
    public override void Interrupt() {
        if (aiming) {
            aiming = false;
            GameObject.Destroy(marker.gameObject);
        }
    }

    void Update() {
        Vector3 fingerPoint = LeapControl.fingerPoint;
        if (aiming) {
            relativeTargetPosition += Vector3.ProjectOnPlane((fingerPoint - prevFingerPoint) * movingFactor, Vector3.up);
            marker.transform.position = attacker.transform.TransformPoint(relativeTargetPosition);
            marker.SetProgress((Time.time - startTime) / attackDelay);
        }
        prevFingerPoint = fingerPoint;
    }
}
