using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This component detects the "grab" gesture (the user's dominant hand in palm state, facing forward, moving forward, and grip).
/// A OnGrab event is dispatched if the gesture is detected. And OnRelease event is dispatched is the user opened up the hand after the gesture.
/// </summary>
public class GrabDetector : MonoBehaviour {

    public delegate void TriggerHandler();
    public static event TriggerHandler OnGrab;
    public static event TriggerHandler OnRelease;
    public static bool grabbing;
    public static GrabDetector detector;

    public float distanceRequired = 300;
    public float distanceDropRate = 1500;
    public float palmAngle = 30; // The angle of the palm normal leaning downward
    public float palmDirectionAngleThreshold = 30;
    public float palmVelocityAngleThreshold = 30;

    public KeyCode simulateKey = KeyCode.Alpha1;

    private float currentDistance = 0;

    // Should be called when reloading scenes.
    public static void Reset() {
        OnGrab = null;
        OnRelease = null;
    }

    // Use this for initialization
    void Start () {
        detector = this;
	}
	
	// Update is called once per frame
	void Update () {
        if (!LeapControl.control.simulateWithMouse) {
            if (LeapControl.handState == LeapControl.HandState.Fist) {
                if (currentDistance >= distanceRequired) {
                    grab();
                }
            } else {
                if (grabbing) {
                    release();
                }
            }

            if (handIsRightOpen() && Vector3.Angle(LeapControl.handSpeed, Vector3.forward) <= palmVelocityAngleThreshold) {
                currentDistance += Vector3.Dot(LeapControl.handSpeed, Vector3.forward) * Time.deltaTime;
            } else {
                currentDistance -= distanceDropRate * Time.deltaTime;
                currentDistance = Mathf.Max(currentDistance, 0);
            }
        } else {
            if (Input.GetKeyDown(simulateKey)) {
                grab();
            }
            if (Input.GetKeyUp(simulateKey)) {
                release();
            }
        }
    }

    private bool handIsRightOpen() {
        if (LeapControl.mainHand == null) return false;
        if (LeapControl.handState != LeapControl.HandState.Palm) return false;
        Vector3 palmDir = new Vector3(0, -Mathf.Sin(Mathf.Deg2Rad * palmAngle), Mathf.Cos(Mathf.Deg2Rad * palmAngle));
        return Vector3.Angle(LeapControl.palmDirection, palmDir) <= palmDirectionAngleThreshold;
    }

    private void grab() {
        grabbing = true;
        if (OnGrab != null) {
            OnGrab();
        }
        currentDistance = 0;
        OverlayDisplay.ShowIndefinately(Links.links.handImage);
    }

    private void release() {
        grabbing = false;
        if (OnRelease != null) {
            OnRelease();
        }
        currentDistance = 0;
        OverlayDisplay.Hide(Links.links.handImage, 0.5f);
    }
}
