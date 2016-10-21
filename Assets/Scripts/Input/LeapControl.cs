using UnityEngine;
using UnityEngine.UI;
using Leap;
using Leap.Unity;
using System.Collections;

public class LeapControl : MonoBehaviour {

    public static LeapControl control;

    public LeapServiceProvider provider;
    public RectTransform pointer, pointer2;
    public Text status;

    public enum HandState
    {
        Pointing,
        Fist,
        Palm,
        Other
    }

    public static Frame frame;
    public static Hand rightHand;
    public static Hand leftHand;
    public static Hand mainHand;
    public static Hand subHand;

    public static Vector3 fingerPoint;
    public static Vector3 fingerVector;
    public static Vector3 handPoint;
    public static Vector3 handSpeed;
    public static Vector3 smoothedHandSpeed;
    public static Vector3 palmDirection;
    public static HandState handState;
    public static bool isTracked;
    public static bool isPointing;

    public static Vector3 fingerPoint2;
    public static Vector3 fingerVector2;
    public static Vector3 handPoint2;
    public static Vector3 handSpeed2;
    public static Vector3 smoothedHandSpeed2;
    public static Vector3 palmDirection2;
    public static HandState handState2;
    public static bool isTracked2;
    public static bool isPointing2;

    public static bool rightHanded = true;
    

    public float fingerBendingAngleThreshold = 90;
    public float stateChangeDelay = 0.2f;

    private Vector3[] cornerPositions;
    private Vector3 center;
    private Vector3 axisX, axisY, axisZ;
    private float width, height;
    private float scaleFactor;
    private float stateChangeProgress = 0;
    private HandState nextState;
    private float stateChangeProgress2 = 0;
    private HandState nextState2;
    private int calibrationIndex = -1;
    private bool calibrated = false;


	// Use this for initialization
	void Start () {
        control = this;
        cornerPositions = new Vector3[] {
            new Vector3(-0.13f, 0.28f, -0.6f),
            new Vector3(0.13f, 0.28f, -0.6f),
            new Vector3(0.13f, 0.1f, -0.65f),
            new Vector3(-0.13f, 0.1f, -0.65f)
        };
        UpdateTrackedSpace();
    }
	
	// Update is called once per frame
	void Update () {
        isTracked = false;
        isTracked2 = false;
        isPointing = false;
        isPointing2 = false;

        UpdateHands();

        if (calibrationIndex >= 0) {
            // Calibrating
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (SetCalibrationPoint(calibrationIndex)) {
                    calibrationIndex++;
                }
            }
            if (calibrationIndex >= 4) {
                UpdateTrackedSpace();
                calibrationIndex = -1;
            }
        } else if (calibrated) {
            UpdateInfo(mainHand, ref fingerPoint, ref fingerVector, ref handPoint, ref handSpeed, ref smoothedHandSpeed, ref palmDirection, ref handState, ref isTracked, ref isPointing, ref nextState, ref stateChangeProgress);
            UpdateInfo(subHand, ref fingerPoint2, ref fingerVector2, ref handPoint2, ref handSpeed2, ref smoothedHandSpeed2, ref palmDirection2, ref handState2, ref isTracked2, ref isPointing2, ref nextState2, ref stateChangeProgress2);
        }

        pointer.anchoredPosition = fingerPoint;
        pointer.gameObject.SetActive(isPointing);
        pointer2.anchoredPosition = handPoint;
        status.text = handState.ToString();
    }

    public void UpdateHands() {
        if (provider.IsConnected()) {
            frame = new Frame();
            frame.CopyFrom(provider.CurrentFrame);
            // right
            if (rightHand != null) {
                rightHand = frame.Hand(rightHand.Id);
            }
            if (rightHand == null) {
                foreach (Hand hand in frame.Hands) {
                    if (hand.IsRight) {
                        rightHand = hand;
                        break;
                    }
                }
            }

            // left
            if (leftHand != null) {
                leftHand = frame.Hand(leftHand.Id);
            }
            if (leftHand == null) {
                foreach (Hand hand in frame.Hands) {
                    if (hand.IsLeft) {
                        leftHand = hand;
                        break;
                    }
                }
            }

            mainHand = rightHanded ? rightHand : leftHand;
            subHand = rightHanded ? leftHand : rightHand;
        }
    }

    public void UpdateInfo(Hand hand, ref Vector3 fingerPoint, ref Vector3 fingerVector, ref Vector3 handPoint, ref Vector3 handSpeed, ref Vector3 smoothedHandSpeed
        , ref Vector3 palmDirection, ref HandState handState, ref bool isTracked, ref bool isPointing, ref HandState nextState, ref float stateChangeProgress) {
        if (hand != null) {
            Finger finger = GetIndexFinger(hand);
            if (finger != null) {
                fingerPoint = GetTransformedFingerPosition(finger);
                fingerVector = GetTransformedFingerDirection(finger);
                isPointing = IsFingerStraight(finger);
            }
            handPoint = TransformPoint(hand.PalmPosition.ToVector3());
            handSpeed = TransformVector(hand.PalmVelocity.ToVector3());
            smoothedHandSpeed = smoothedHandSpeed * 0.8f + handSpeed * 0.2f;
            palmDirection = TransformVector(hand.PalmNormal.ToVector3()).normalized;

            HandState detectedState = getState(hand);
            if (detectedState != handState) {
                if (nextState == detectedState) {
                    stateChangeProgress += Time.deltaTime;
                    if (stateChangeProgress > stateChangeDelay) {
                        handState = detectedState;
                        stateChangeProgress = 0;
                    }
                } else {
                    nextState = detectedState;
                    stateChangeDelay = 0;
                }
            } else {
                nextState = detectedState;
            }
            isTracked = true;
        }
    }

    public bool SetCalibrationPoint(int i) {
        if (rightHand == null) {
            Debug.Log("Right hand not found!");
        } else {
            Finger index = GetIndexFinger(rightHand);
            if (index == null) {
                Debug.Log("Index finger not found!");
            } else {
                cornerPositions[i] = index.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
                Debug.Log("Position get! " + cornerPositions[i].x + " " + cornerPositions[i].y + " " + cornerPositions[i].z);
                return true;
            }
        }
        return false;
    }

    public void UpdateTrackedSpace() {
        center = (cornerPositions[0] + cornerPositions[1] + cornerPositions[2] + cornerPositions[3]) / 4;
        axisZ = Vector3.zero;
        for (int i = 0; i < 4; i++) {
            axisZ -= Vector3.Cross(cornerPositions[i] - center, cornerPositions[(i + 1) % 4] - center);
        }
        axisZ.Normalize();
        for (int i = 0; i < 4; i++) {
            cornerPositions[i] = center + Vector3.ProjectOnPlane(cornerPositions[i] - center, axisZ);
        }

        axisX = cornerPositions[1] + cornerPositions[2] - cornerPositions[0] - cornerPositions[3];
        width = axisX.magnitude / 2;
        axisX.Normalize();

        height = (cornerPositions[0] + cornerPositions[1] - cornerPositions[2] - cornerPositions[3]).magnitude / 2;
        axisY = Vector3.Cross(axisZ, axisX);

        scaleFactor = (Screen.width / width + Screen.height / height) / 2;
        calibrated = true;
    }

    public void StartCalibration() {
        cornerPositions = new Vector3[4];
        calibrationIndex = 0;
        calibrated = false;
    }

    public Vector3 TransformPoint(Vector3 p) {
        if (!calibrated) {
            return new Vector3();
        } else {
            return TransformVector(p - center);
        }
    }

    public Vector3 TransformVector(Vector3 v) {
        if (!calibrated) {
            return new Vector3();
        } else {
            return new Vector3(Vector3.Dot(v, axisX) * scaleFactor, Vector3.Dot(v, axisY) * scaleFactor, Vector3.Dot(v, axisZ) * scaleFactor);
        }
    }

    public Finger GetIndexFinger(Hand hand) {
        Finger index = null;
        foreach (Finger f in hand.Fingers) {
            if (f.Type == Finger.FingerType.TYPE_INDEX) {
                index = f;
            }
        }
        return index;
    }

    public Finger GetFinger(Hand hand, Finger.FingerType type) {
        Finger finger = null;
        foreach (Finger f in hand.Fingers) {
            if (f.Type == type) {
                finger = f;
            }
        }
        return finger;
    }

    public Vector3 GetFingerPosition(Finger finger) {
        return finger.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
    }

    public Vector3 GetFingerDirection(Finger finger) {
        Vector3 p1 = finger.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
        Vector3 p2 = finger.Bone(Bone.BoneType.TYPE_INTERMEDIATE).PrevJoint.ToVector3();
        return (p1 - p2).normalized;
    }

    public Vector3 GetTransformedFingerPosition(Finger finger) {
        return TransformPoint(GetFingerPosition(finger));
    }

    public Vector3 GetTransformedFingerDirection(Finger finger) {
        return TransformVector(GetFingerDirection(finger)).normalized;
    }

    public bool IsFingerStraight(Finger finger, float threshold = 90) {
        if (finger == null) return false;

        float totalAngle = 0;
        Vector3[] vs = new Vector3[5];
        vs[0] = finger.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint.ToVector3();
        vs[1] = finger.Bone(Bone.BoneType.TYPE_METACARPAL).NextJoint.ToVector3();
        vs[2] = finger.Bone(Bone.BoneType.TYPE_PROXIMAL).NextJoint.ToVector3();
        vs[3] = finger.Bone(Bone.BoneType.TYPE_INTERMEDIATE).NextJoint.ToVector3();
        vs[4] = finger.Bone(Bone.BoneType.TYPE_DISTAL).NextJoint.ToVector3();
        for (int i = 1; i <= 3; i++) {
            totalAngle += Vector3.Angle(vs[i] - vs[i - 1], vs[i + 1] - vs[i]);
        }
        return totalAngle < threshold;
    }

    public HandState getState(Hand hand) {
        Finger index = GetFinger(hand, Finger.FingerType.TYPE_INDEX);
        Finger middle = GetFinger(hand, Finger.FingerType.TYPE_MIDDLE);
        Finger ring = GetFinger(hand, Finger.FingerType.TYPE_RING);
        Finger pinky = GetFinger(hand, Finger.FingerType.TYPE_PINKY);
        Finger thumb = GetFinger(hand, Finger.FingerType.TYPE_THUMB);
        
        if (IsFingerStraight(index) &&
            (!IsFingerStraight(middle, 120) || (GetTransformedFingerPosition(index) - GetTransformedFingerPosition(middle)).magnitude < 300) &&
            !IsFingerStraight(ring) &&
            !IsFingerStraight(pinky)) {
            return HandState.Pointing;
        }
        if (!IsFingerStraight(index) &&
            !IsFingerStraight(middle) &&   
            !IsFingerStraight(ring) &&
            !IsFingerStraight(pinky)) {
            return HandState.Fist;
        }
        int straightFingerCount = 0;
        if (IsFingerStraight(index)) straightFingerCount++;
        if (IsFingerStraight(middle)) straightFingerCount++;
        if (IsFingerStraight(ring)) straightFingerCount++;
        if (IsFingerStraight(pinky)) straightFingerCount++;
        if (IsFingerStraight(thumb)) straightFingerCount++;
        if (straightFingerCount >= 4) return HandState.Palm;

        return HandState.Other;
    }

}
