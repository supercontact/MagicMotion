using UnityEngine;
using System.Collections;

/// <summary>
/// This component randomizes the initial transformation of the GameObject.
/// </summary>
public class TransformRandomizer : MonoBehaviour {

    public float scaleRange = 0;
    public float displaceXRange = 0;
    public float displaceYRange = 0;
    public float displaceZRange = 0;
    public float rotationRange = 0; // Horizontal rotation (rotation around y-axis)
    public float tiltRange = 0; // Tilting the y axis by random angle in random direction

    // Apply in Awake to correctly transform static objects of the scene.
    void Awake() {
        Apply();
    }

    public void Apply() {
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
