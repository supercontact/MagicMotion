using UnityEngine;
using System.Collections;

public class TransformRandomizer : MonoBehaviour {

    public float scaleRange = 0.2f;
    public float displaceXRange = 0.5f;
    public float displaceYRange = 0;
    public float displaceZRange = 0.5f;
    public float rotationRange = 360;
    public float tiltRange = 30;

    // Use this for initialization
    void Awake() {
        transform.localPosition = transform.localPosition + new Vector3(
            Random.Range(-displaceXRange / 2, displaceXRange / 2),
            Random.Range(-displaceYRange / 2, displaceYRange / 2),
            Random.Range(-displaceZRange / 2, displaceZRange / 2));
        transform.localScale = transform.localScale * Random.Range(1 - scaleRange / 2, 1 + scaleRange / 2);
        float angle = Random.Range(0, tiltRange / 2);
        Vector3 tiltAxis = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Vector3.right;
        transform.localRotation = Quaternion.AngleAxis(angle, tiltAxis) * Quaternion.AngleAxis(Random.Range(-rotationRange / 2, rotationRange / 2), Vector3.up) * transform.localRotation;
    }
	
}
