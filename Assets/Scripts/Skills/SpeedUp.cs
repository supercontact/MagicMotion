using UnityEngine;
using System.Collections;

/// <summary>
/// A spell that temporarily increase the movement speed of the target. Useful for running away from the enemy. (Did you say charging into the enemy?)
/// </summary>
public class SpeedUp : SpecialAttack
{
    public float duration = 5;
    public float speedBoost = 3;

    public SpeedUp() {
        attackPeriod = 0.5f;
        attackDelay = 0f;
    }

    public override bool IsUsableNow() {
        return true;
    }
    public override void AttackAction() {
        SpeedBuff buff = new GameObject().AddComponent<SpeedBuff>();
        buff.unit = target;
        buff.duration = duration;
        buff.speedBoost = speedBoost;
        OverlayDisplay.Show(Links.links.infinityImage, 0, 0.5f);
    }

}
