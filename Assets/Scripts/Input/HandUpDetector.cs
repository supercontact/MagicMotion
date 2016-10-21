using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HandUpDetector : MonoBehaviour {

    public delegate void TriggerHandler();
    public static event TriggerHandler OnTrigger;
    public static HandUpDetector detector;

    public float distanceRequired = 300;
    public float palmAngle = 15;
    public float palmDirectionAngleThreshold = 30;
    public float palmVelocityAngleThreshold = 30;

    public Image image;

    private float currentDistance = 0;

    // Use this for initialization
    void Start () {
        detector = this;
	}
	
	// Update is called once per frame
	void Update () {
	    if (handIsUp() && Vector3.Angle(LeapControl.handSpeed, Vector3.up) <= palmVelocityAngleThreshold) {
            currentDistance += Vector3.Dot(LeapControl.handSpeed, Vector3.up) * Time.deltaTime;
            if (currentDistance >= distanceRequired) {
                finish();
            }
        } else {
            currentDistance = 0;
        }

        if (image.gameObject.activeSelf) {
            float newAlpha = image.color.a - 2 * Time.deltaTime;
            if (newAlpha < 0) {
                image.gameObject.SetActive(false);
            } else {
                image.color = new Color(1, 1, 1, newAlpha);
            }
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
        image.gameObject.SetActive(true);
        image.color = Color.white;
    }
}
