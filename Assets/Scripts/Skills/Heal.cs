using UnityEngine;
using System.Collections;

/// <summary>
/// A spell that gradually recover health of the target.
/// The spell does not seem too easy to cast, isn't it? I recommand not to cast it when your are in tight battle...
/// </summary>
public class Heal : SpecialAttack
{
    public float duration = 10;
    public int regenerateAmountPerTick = 1;
    public float tickPerSecond = 5;

    public Heal() {
        attackPeriod = 0.5f;
        attackDelay = 0f;
    }

    public override bool IsUsableNow() {
        return true;
    }
    public override void AttackAction() {
        RegenerateHealthBuff buff = new GameObject().AddComponent<RegenerateHealthBuff>();
        buff.unit = target;
        buff.duration = duration;
        buff.regenerateAmountPerTick = regenerateAmountPerTick;
        buff.tickPerSecond = tickPerSecond;
        OverlayDisplay.Show(Links.links.healImage, 0, 0.5f);
    }

}
