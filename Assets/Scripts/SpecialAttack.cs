using UnityEngine;
using System.Collections;

public class SpecialAttack {

    public float attackPeriod;
    public float attackDelay;

    public virtual bool IsUsableNow(Unit target, Unit self) {
        // To be overridden
        return false;
    }
	public virtual void PreAttackAction(Unit target, Unit self) {
        // To be overridden
    }
    public virtual void AttackAction(Unit target, Unit self) {
        // To be overridden
    }
}
