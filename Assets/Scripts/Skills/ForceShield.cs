using UnityEngine;
using System.Collections;

public class ForceShield : MonoBehaviour {

    public Unit unit;
    public GameObject shield;
    public float anglePerPixel = 0.1f;
    public float angleOffset = 40;
    public float openAnimationDuration = 0.2f;

    public bool isActive = false;

    private float prepareProgress = 0;

	// Use this for initialization
	void Start () {
        shield.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        isActive = (LeapControl.isTracked2 && LeapControl.handState2 == LeapControl.HandState.Palm);
        
        if (isActive) {
            shield.SetActive(true);
            transform.localPosition = Vector3.zero;
            prepareProgress += Time.deltaTime / openAnimationDuration;
            prepareProgress = Mathf.Min(prepareProgress, 1);
            transform.localScale = new Vector3(1, prepareProgress, 1);
            transform.localRotation = Quaternion.AngleAxis(LeapControl.handPoint2.x * anglePerPixel + (LeapControl.rightHanded ? angleOffset : -angleOffset), Vector3.up);
        } else {
            prepareProgress -= Time.deltaTime / openAnimationDuration;
            if (prepareProgress > 0) {
                transform.localScale = new Vector3(1, prepareProgress, 1);
            } else {
                shield.SetActive(false);
                transform.localPosition = Vector3.down * 100;
            }
            prepareProgress = Mathf.Max(prepareProgress, 0);
        }
	}
}
