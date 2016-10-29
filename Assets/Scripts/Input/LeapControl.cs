using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Leap;
using Leap.Unity;
using System.Collections;

/// <summary>
/// This component wraps the Leap Motion library and extract useful hand input information, including index finger positions, palm directions, hand states, etc.
/// It transform the positions into screen space (with depth) which can be calibrated by the user.
/// The dominant hand can be set to either right hand or left hand.
/// You can simulate the hand movement using mouse if simulateWithMouse is set to true.
/// </summary>
public class LeapControl : MonoBehaviour {

    public static LeapControl control;

    public LeapServiceProvider provider;
    public RectTransform pointer, pointer2;
    public Text status;
    public bool simulateWithMouse = false;
    public KeyCode subHandSimulateKey = KeyCode.Alpha3;

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

    public static bool rightHanded = true;

    // Information about the dominant hand
    public static Vector3 fingerPoint;
    public static Vector3 fingerVector;
    public static Vector3 handPoint;
    public static Vector3 handSpeed;
    public static Vector3 smoothedHandSpeed;
    public static Vector3 palmDirection;
    public static HandState handState;
    public static bool isTracked;
    public static bool isPointing;

    // Information about the secondary hand
    public static Vector3 fingerPoint2;
    public static Vector3 fingerVector2;
    public static Vector3 handPoint2;
    public static Vector3 handSpeed2;
    public static Vector3 smoothedHandSpeed2;
    public static Vector3 palmDirection2;
    public static HandState handState2;
    public static bool isTracked2;
    public static bool isPointing2;
    

    public float fingerBendingAngleThreshold = 90;
    public float stateChangeDelay = 0.2f;

    private static Vector3[] cornerPositions = new Vector3[] {
            new Vector3(-0.13f, 0.28f, 0.4f),
            new Vector3(0.13f, 0.28f, 0.4f),
            new Vector3(0.13f, 0.1f, 0.35f),
            new Vector3(-0.13f, 0.1f, 0.35f)};
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

    private Vector3 lastMousePosition;
    private Vector3 lastMouseSmoothedSpeed;


    public void SetRightHanded(bool value) {
        rightHanded = value;
    }

    public void SetMouseSimulation(bool value) {
        simulateWithMouse = value;
    }

    // Use this for initialization
    void Start () {
        control = this;
        UpdateTrackedSpace();
    }
	
	// Update is called once per frame
	void Update () {
        if (!simulateWithMouse) {
            isTracked = false;
            isTracked2 = false;
            isPointing = false;
            isPointing2 = false;

            UpdateHands();

            if (calibrationIndex >= 0) {
                // Calibrating
                if (Input.GetKeyDown(KeyCode.Space)) {
                    if (SetCalibrationPoint(calibrationIndex)) {
                        Links.links.calibrationInstructions[calibrationIndex].SetActive(false);
                        calibrationIndex++;
                    }
                }
                if (calibrationIndex >= 4) {
                    UpdateTrackedSpace();
                    calibrationIndex = -1;
                    SceneControl.Unpause();
                } else {
                    Links.links.calibrationInstructions[calibrationIndex].SetActive(true);
                }
            } else if (calibrated) {
                UpdateInfo(mainHand, ref fingerPoint, ref fingerVector, ref handPoint, ref handSpeed, ref smoothedHandSpeed, ref palmDirection, ref handState, ref isTracked, ref isPointing, ref nextState, ref stateChangeProgress);
                UpdateInfo(subHand, ref fingerPoint2, ref fingerVector2, ref handPoint2, ref handSpeed2, ref smoothedHandSpeed2, ref palmDirection2, ref handState2, ref isTracked2, ref isPointing2, ref nextState2, ref stateChangeProgress2);
            }
        } else {
            // Simulating hands with mouse
            isTracked = true;
            isTracked2 = Input.GetKey(subHandSimulateKey);

            Vector3 mousePos = new Vector3(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2, 0);

            isPointing = true;
            isPointing2 = true;
            handState = HandState.Pointing;
            handState2 = HandState.Palm;
            fingerPoint = mousePos;
            fingerPoint2 = fingerPoint;
            fingerVector = Vector3.forward;
            fingerVector2 = fingerVector;
            handPoint = fingerPoint;
            handPoint2 = handPoint;
            handSpeed = (mousePos - lastMousePosition) / Time.deltaTime;
            handSpeed2 = handSpeed;
            smoothedHandSpeed = lastMouseSmoothedSpeed * 0.8f + handSpeed * 0.2f;
            smoothedHandSpeed2 = smoothedHandSpeed;
            palmDirection = Vector3.down;
            palmDirection2 = palmDirection;

            lastMousePosition = mousePos;
            lastMouseSmoothedSpeed = smoothedHandSpeed;
        }

        pointer.anchoredPosition = fingerPoint;
        pointer.gameObject.SetActive(isPointing);
        pointer2.anchoredPosition = handPoint;
        status.text = handState.ToString();
    }

    private void UpdateHands() {
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

    private void UpdateInfo(Hand hand, ref Vector3 fingerPoint, ref Vector3 fingerVector, ref Vector3 handPoint, ref Vector3 handSpeed, ref Vector3 smoothedHandSpeed
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

            HandState detectedState = GetState(hand);
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

    private bool SetCalibrationPoint(int i) {
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
        OverlayDisplay.Show(Links.links.calibrationWarning, 2, 1);
        return false;
    }

    private void UpdateTrackedSpace() {
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

        //scaleFactor = (Screen.width / width + Screen.height / height) / 2;
        scaleFactor = (1920 / width + 1080 / height) / 2;
        calibrated = true;
    }

    public void StartCalibration() {
        cornerPositions = new Vector3[4];
        calibrationIndex = 0;
        calibrated = false;
        Links.links.calibrationInstructions[0].SetActive(true);
        Links.links.calibrationInstructions[1].SetActive(false);
        Links.links.calibrationInstructions[2].SetActive(false);
        Links.links.calibrationInstructions[3].SetActive(false);
        SceneControl.EnterSlowMotion();
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

    public HandState GetState(Hand hand) {
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

    /// <summary>
    /// Returns the finger position if it is present, otherwise returns the hand position
    /// </summary>
    public static Vector3 GetTargetingPositionOnScreen() {
        if (isPointing) {
            return new Vector3(fingerPoint.x + Screen.width / 2, fingerPoint.y + Screen.height / 2, fingerPoint.z);
        } else {
            return new Vector3(handPoint.x + Screen.width / 2, handPoint.y + Screen.height / 2, handPoint.z);
        }
    }

}
