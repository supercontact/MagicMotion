using UnityEngine;
using System.Collections;

public class BigEnemy : WanderingBasicAI {
	
	public float bulletRange = 15f;
	public float bulletSpeed = 8f;

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
