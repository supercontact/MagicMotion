using UnityEngine;
using System.Collections;

public class Player : Unit {

    public enum PlayerState
    {
        Normal,
        Slowed,
        Busy
    }

    public Animator anim;
    public float rotateAreaWidth = 300;
    public float maxRotationSpeed = 90;
    public PlayerState playerState = PlayerState.Normal;

    private AntiGravity skillAntiGravity;
    private Grabbing skillGrabbing;
    private EarthSpikes skillSpike;
    private LightBeam skillBeam;
    private FireBallSpell skillFireBall;
    private LightningStrike skillLightning;
    private SummonHelper skillSummonHelper;
    private Heal skillHeal;
    private IncreaseSpeed skillIncreaseSpeed;

    // Use this for initialization
    public override void Start () {
        base.Start();
        interruptableByNormalAttack = false;

        TrajectoryDetector.OnTrigger += TrajectoryGestureTriggered;
        HandUpDetector.OnTrigger += HandUpGestureTriggered;
        GrabDetector.OnGrab += GrabGestureTriggered;

        skillAntiGravity = new AntiGravity();
        skillGrabbing = new Grabbing();
        skillSpike = new EarthSpikes();
        skillBeam = new LightBeam();
        skillFireBall = new FireBallSpell();
        skillLightning = new LightningStrike();
        skillSummonHelper = new SummonHelper();
        skillHeal = new Heal();
        skillIncreaseSpeed = new IncreaseSpeed();
    }

    // Update is called once per frame
    public override void Update() {
        if (state == State.Casting) {
            playerState = PlayerState.Slowed;
        } else if (isBusy()) {
            playerState = PlayerState.Busy;
        } else {
            playerState = PlayerState.Normal;
        }

        if (playerState != PlayerState.Busy) {
            Vector3 v = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            v *= (Input.GetKey(KeyCode.LeftShift) || playerState == PlayerState.Slowed) ? moveSpeed / 2 : moveSpeed;

            float lerp = 1 - Mathf.Exp(-Time.deltaTime * 10);
            if (v != Vector3.zero) {
                controller.Move(transform.TransformVector(v) * Time.deltaTime);
                anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), (v.magnitude / moveSpeed) / 2 + 0.5f, lerp));
            } else {
                anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), 0.5f, lerp));
            }

            if (LeapControl.isTracked) {
                Vector3 handPosition = LeapControl.GetTargetingPositionOnScreen();
                if (handPosition.x < rotateAreaWidth) {
                    float r = 1 - handPosition.x / rotateAreaWidth;
                    transform.rotation = Quaternion.AngleAxis(-r * maxRotationSpeed * Time.deltaTime, Vector3.up) * transform.rotation;
                } else if (handPosition.x > Screen.width - rotateAreaWidth) {
                    float r = 1 - (Screen.width - handPosition.x) / rotateAreaWidth;
                    transform.rotation = Quaternion.AngleAxis(r * maxRotationSpeed * Time.deltaTime, Vector3.up) * transform.rotation;
                }
            }
        }

        base.Update();
    }

    public override void DieAction() {
        anim.SetTrigger("Die");
        Links.links.gameOver.SetActive(true);
    }

    private void TrajectoryGestureTriggered(string type) {
        if (!isBusy()) {
            if (type == "Spike") {
                Cast(null, skillSpike);
            } else if (type == "Cross") {
                Cast(null, skillBeam);
            } else if (type == "Circle") {
                Cast(null, skillFireBall);
            } else if (type == "Lightning") {
                Cast(null, skillLightning);
            } else if (type == "Star") {
                Cast(null, skillSummonHelper);
            } else if (type == "Heal") {
                Cast(this, skillHeal);
            } else if (type == "Infinity") {
                Cast(this, skillIncreaseSpeed);
            }
        }
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
