using UnityEngine;
using System.Collections;

public class LockEffect : MonoBehaviour {

    public GameObject sprite1;
    public GameObject sprite2;
    public float rotationSpeed = 60;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        sprite1.transform.localRotation = sprite1.transform.localRotation * Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.forward);
        sprite2.transform.localRotation = sprite2.transform.localRotation * Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.forward);
    }
}
