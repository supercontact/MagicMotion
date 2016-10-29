using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This component detects the "hand up" gesture (the user's dominant hand facing up and moving up). Events are dispatched if the gesture is detected.
/// </summary>
public class HandUpDetector : MonoBehaviour {

    public delegate void TriggerHandler();
    public static event TriggerHandler OnTrigger;
    public static HandUpDetector detector;

    public float distanceRequired = 300;
    public float palmAngle = 15; // The angle of the palm normal leaning backwards
    public float palmDirectionAngleThreshold = 30;
    public float palmVelocityAngleThreshold = 30;

    public KeyCode simulateKey = KeyCode.Alpha2;

    private float currentDistance = 0;

    // Should be called when reloading scenes.
    public static void Reset() {
        OnTrigger = null;
    }

    // Use this for initialization
    void Start () {
        detector = this;
	}
	
	// Update is called once per frame
	void Update () {
        if (!LeapControl.control.simulateWithMouse) {
            if (handIsUp() && Vector3.Angle(LeapControl.handSpeed, Vector3.up) <= palmVelocityAngleThreshold) {
                currentDistance += Vector3.Dot(LeapControl.handSpeed, Vector3.up) * Time.deltaTime;
                if (currentDistance >= distanceRequired) {
                    finish();
                }
            } else {
                currentDistance = 0;
            }
        } else if (Input.GetKeyDown(simulateKey)) {
            finish();
        }
    }

    private bool handIsUp() {
        if (LeapControl.mainHand == null) return false;
        return Vector3.Angle(LeapControl.palmDirection, new Vector3(0, Mathf.Cos(palmAngle * Mathf.Deg2Rad), -Mathf.Sin(palmAngle * Mathf.Deg2Rad))) <= palmDirectionAngleThreshold;
    }

    private void finish() {
        if (OnTrigger != null) {
            OnTrigger();
        }
        currentDistance = 0;
    }
}
