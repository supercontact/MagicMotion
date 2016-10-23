using UnityEngine;
using System.Collections;

public class LightBeam : SpecialAttack {

    public int damagePerTick = 5;
    public float tickPerSecond = 6;
    public float radius = 0.1f;
    public float range = 15;
    public float anglePerPixel = 0.1f;


    private bool aiming = false;
    private bool firing = false;
    private float startTime;
    private GameObject aimLine;
    private BeamEffect beam;
    private Vector3 prevFingerPoint;
    private float angle;
    private float damageTimer;

    public LightBeam() {
        attackPeriod = 3f;
        attackDelay = 1f;
        UpdateSender.OnUpdate += Update;
    }

    public override bool IsUsableNow() {
        return true;
    }

    public override void PreAttackAction() {
        angle = 0;
        aiming = true;
        startTime = Time.time;
        aimLine = GameObject.Instantiate(Links.links.aimLine);
        aimLine.transform.position = attacker.transform.position + 0.5f * Vector3.up;
        aimLine.transform.localScale = new Vector3(1, 1, range);
        OverlayDisplay.ShowImage(Links.links.crossImage, 0, 0.5f);
    }

    public override void AttackAction() {
        firing = true;
        damageTimer = 0;
        beam = GameObject.Instantiate(Links.links.beam).GetComponent<BeamEffect>();
        beam.transform.position = attacker.transform.position + attacker.centerHeight * Vector3.up;
        beam.transform.rotation = attacker.transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
        beam.transform.localScale = new Vector3(1, 1, range);
        aiming = false;
        GameObject.Destroy(aimLine);
    }

    public override void EndAction() {
        if (firing) {
            firing = false;
            beam.Off();
        }
    }

    public override void Interrupt() {
        if (aiming) {
            aiming = false;
            GameObject.Destroy(aimLine);
        } else if (firing) {
            firing = false;
            beam.Off();
        }
    }

    void Update() {
        Vector3 fingerPoint = LeapControl.fingerPoint;
        angle += (fingerPoint.x - prevFingerPoint.x) * anglePerPixel;
        if (aiming) {
            aimLine.transform.position = attacker.transform.position + attacker.centerHeight * Vector3.up;
            aimLine.transform.rotation = attacker.transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
        } else if (firing) {
            beam.transform.position = attacker.transform.position + attacker.centerHeight * Vector3.up;
            beam.transform.rotation = attacker.transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0) {
                RayCastLine(true);
                damageTimer += 1 / tickPerSecond;
            } else {
                RayCastLine(false);
            }
        }
            
        prevFingerPoint = fingerPoint;
    }

    private void RayCastLine(bool inflictDamage) {
        Vector3 startPosition = beam.transform.position;
        Vector3 direction = beam.transform.forward;
        RaycastHit[] hits = Physics.SphereCastAll(startPosition, radius, direction, range);

        Unit firstEnemy = null;
        float minDistance = range;
        foreach (RaycastHit hit in hits) {
            Unit unit = hit.collider.GetComponent<Unit>();
            if ((unit == null || unit.team != attacker.team) && hit.distance < minDistance) {
                firstEnemy = unit;
                minDistance = hit.distance;
            }
        }
        beam.transform.localScale = new Vector3(1, 1, minDistance);
        if (minDistance < range) {
            beam.ParticlesOn();
        } else {
            beam.ParticlesOff();
        }
        if (inflictDamage && firstEnemy != null) {
            firstEnemy.ReceiveDamage(damagePerTick, attacker);
        }
    }
}
