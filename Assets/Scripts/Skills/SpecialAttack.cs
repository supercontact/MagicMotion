using UnityEngine;
using System.Collections;

public class SpecialAttack {

    public float attackPeriod;
    public float attackDelay;
    public bool lasting = false;

    public Unit target;
    public Unit attacker;

    public virtual bool IsUsableNow() {
        // To be overridden
        return false;
    }
	public virtual void PreAttackAction() {
        // To be overridden
    }
    public virtual void AttackAction() {
        // To be overridden
    }
    public virtual void EndAction() {
        // To be overridden
    }
    public virtual void Interrupt() {
        // To be overridden, called if interrupted. 
    }
}
