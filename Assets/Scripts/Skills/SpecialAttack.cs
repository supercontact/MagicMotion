using UnityEngine;
using System.Collections;

/// <summary>
/// The base class for all special attacks (spells or skills).
/// </summary>
public class SpecialAttack {

    public float attackPeriod;
    public float attackDelay;
    public bool lasting = false;

    public Unit target;
    public Unit attacker;

    /// <summary>
    /// Return if the unit can use this special attack at this moment.
    /// </summary>
    public virtual bool IsUsableNow() {
        // To be overridden
        return false;
    }

    /// <summary>
    /// Called when the unit starts a special attack.
    /// </summary>
	public virtual void PreAttackAction() {
        // To be overridden
    }

    /// <summary>
    /// Called when the unit actually do the attack (i.e. cause damage), which happened attackDelay seconds after initiating the attack.
    /// </summary>
    public virtual void AttackAction() {
        // To be overridden
    }

    /// <summary>
    /// Called when the attack is over, which happened attackPeriod seconds after initiating the attack.
    /// </summary>
    public virtual void EndAction() {
        // To be overridden
    }

    /// <summary>
    /// Called if the attack is interrupted before ending.
    /// </summary>
    public virtual void Interrupt() {
        // To be overridden 
    }
}
