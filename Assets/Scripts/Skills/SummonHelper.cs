using UnityEngine;
using System.Collections.Generic;

public class SummonHelper : SpecialAttack {

    public Vector3 position = new Vector3(0, 0, 2.5f);

    private Vector3 summonPosition;

    public SummonHelper() {
        attackPeriod = 1f;
        attackDelay = 0.5f;
    }

    public override bool IsUsableNow() {
        return true;
    }

    public override void PreAttackAction() {
        GameObject ring = GameObject.Instantiate(Links.links.magicRing);
        summonPosition = attacker.transform.TransformPoint(position);
        ring.transform.position = summonPosition;
        OverlayDisplay.Show(Links.links.starImage, 0, 0.5f);
    }

    public override void AttackAction() {
        Helper helper = GameObject.Instantiate(Links.links.helper1).GetComponent<Helper>();
        helper.transform.position = summonPosition;
        helper.master = attacker;
        helper.team = attacker.team;
    }
}
