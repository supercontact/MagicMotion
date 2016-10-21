using UnityEngine;
using System.Collections;

public class Player : Unit {

    public enum PlayerState
    {
        Normal,
        Preparing,
        Busy
    }

    public float speed = 3f;
    public Animator anim;
    public float rotateAreaWidth = 300;
    public float maxRotationSpeed = 90;
    public PlayerState playerState = PlayerState.Normal;

    private AntiGravity skillAntiGravity;
    private Grabbing skillGrabbing;

    // Use this for initialization
    public override void Start () {
        base.Start();
        anim = GetComponent<Animator>();
        TrajectoryDetector.OnTrigger += TrajectoryGestureTriggered;
        HandUpDetector.OnTrigger += HandUpGestureTriggered;
        GrabDetector.OnGrab += GrabGestureTriggered;

        skillAntiGravity = new AntiGravity();
        skillGrabbing = new Grabbing();
    }

    // Update is called once per frame
    public override void Update() {

        if (isBusy()) {
            playerState = PlayerState.Busy;
        } else if (playerState == PlayerState.Busy) {
            playerState = PlayerState.Normal;
        }

        if (playerState != PlayerState.Busy) {
            Vector3 v = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            v *= (Input.GetKey(KeyCode.LeftShift) || playerState == PlayerState.Preparing) ? speed / 2 : speed;

            float lerp = 1 - Mathf.Exp(-Time.deltaTime * 10);
            if (v != Vector3.zero) {
                controller.Move(transform.TransformVector(v) * Time.deltaTime);
                anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), (Vector3.Dot(v, transform.forward) / speed) / 2 + 0.5f, lerp));
            } else {
                anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), 0.5f, lerp));
            }

            Vector3 handPosition = InputManager.GetHandPositionOnScreen();
            if (handPosition.x < rotateAreaWidth) {
                float r = 1 - handPosition.x / rotateAreaWidth;
                transform.rotation = Quaternion.AngleAxis(-r * maxRotationSpeed * Time.deltaTime, Vector3.up) * transform.rotation;
            } else if (handPosition.x > Screen.width - rotateAreaWidth) {
                float r = 1 - (Screen.width - handPosition.x) / rotateAreaWidth;
                transform.rotation = Quaternion.AngleAxis(r * maxRotationSpeed * Time.deltaTime, Vector3.up) * transform.rotation;
            }
        }

        base.Update();
    }

    private void TrajectoryGestureTriggered(string type) {

    }
    private void HandUpGestureTriggered() {
        if (!isBusy()) {
            Cast(null, skillAntiGravity);
        }
    }
    private void GrabGestureTriggered() {
        if (!isBusy()) {
            Cast(null, skillGrabbing);
        }
    }
}
