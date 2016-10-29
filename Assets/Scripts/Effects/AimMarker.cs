using UnityEngine;
using System.Collections;

/// <summary>
/// The effect of the aim marker used for earth spike spell.
/// </summary>
public class AimMarker : MonoBehaviour {

    public GameObject innerRing;
    public GameObject outerRing;

	// Use this for initialization
	void Start () {
        innerRing.transform.localScale = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
        outerRing.transform.localRotation = Quaternion.AngleAxis(90 * Time.deltaTime, Vector3.up) * outerRing.transform.localRotation;
    }

    public void SetProgress(float progress) {
        innerRing.transform.localScale = Vector3.one * progress;
    }
}
