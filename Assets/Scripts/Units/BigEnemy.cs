using UnityEngine;
using System.Collections;

/// <summary>
/// A big enemy that can split into multiple small enemies when die.
/// </summary>
public class BigEnemy : WanderingBasicAI {

    private Vector3[] spawnPosition = new Vector3[] {
        Vector3.left + Vector3.forward,
        Vector3.right + Vector3.forward,
        Vector3.left + Vector3.back,
        Vector3.right + Vector3.back
    };

	public override void DieAction() {
		base.DieAction ();
        for (int i = 0; i < spawnPosition.Length; i++) {
            WanderingBasicAI smallGuy = Instantiate(Links.links.smallEnemy).GetComponent<WanderingBasicAI>();
            smallGuy.transform.position = transform.position + transform.TransformDirection(spawnPosition[i]);
            smallGuy.transform.rotation = transform.rotation;
        }
	}
}
