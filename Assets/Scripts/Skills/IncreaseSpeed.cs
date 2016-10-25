using UnityEngine;
using System.Collections;

public class IncreaseSpeed : SpecialAttack
{
    public float duration = 5;
    public float speedBoost = 3;

    public IncreaseSpeed() {
        attackPeriod = 0.5f;
        attackDelay = 0f;
    }

    public override bool IsUsableNow() {
        return true;
    }
    public override void AttackAction() {
        IncreaseSpeedBuff buff = new GameObject().AddComponent<IncreaseSpeedBuff>();
        buff.unit = target;
        buff.duration = duration;
        buff.speedBoost = speedBoost;
        OverlayDisplay.ShowImage(Links.links.infinityImage, 0, 0.5f);
    }

}
