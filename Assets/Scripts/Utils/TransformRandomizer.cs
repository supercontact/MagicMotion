using UnityEngine;
using System.Collections;

public class TransformRandomizer : MonoBehaviour {

    public float scaleRange = 0.2f;

    // Use this for initialization
    void Awake () {
        transform.localScale = transform.localScale * Random.Range(1 - scaleRange / 2, 1 + scaleRange / 2);
        transform.localRotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * transform.localRotation;
	}
	
}
