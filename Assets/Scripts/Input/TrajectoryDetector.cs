using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// This component detects the drawing performed by the user's index finger, and dispatches event whenever the user finished drawing the predefined path.
/// It uses a "balls running through tubes" model, which considers the drawing complete if any of the continuously spawned balls driven by user's finger movement reached the end of the path.
/// If a ball keeps crashing into the border of the tube, it will be removed when a defined threshold is surpassed. 
/// It has multiple scale level where the balls performs different amount of movement at the same times, which is useful for detecting various gesture size.
/// It also support visualization of the model.
/// </summary>
public class TrajectoryDetector : MonoBehaviour {

    public delegate void TriggerHandler(string type);
    public static event TriggerHandler OnTrigger;
    public static List<TrajectoryDetector> actionList = new List<TrajectoryDetector>();

    /// <summary>
    /// The path to follow, representing a specific gesture (or rune).
    /// </summary>
    public Vector3[] path;

    public string type = "action";
    public float maxDuration = 2;
    public float jointRadius = 100;
    public float pathRadius = 80;
    public float jointOvershootFactor = 0.5f; // Amount of excessive movement allowed when crashing into the border of the sphere joint. Number is the portion of the previous segment length. 
    public float pathOvershootFactor = 0.25f; // Amount of excessive movement allowed when crashing into the border of the cylinder tube. Number is the portion of the current segment length. 
    public float scaleFactor = 1.2f;
    public int maxScaleLevel = 3;
    public int minScaleLevel = -3;

    // Visualization related
    public bool initVisualization = false; // Visualization can only be actived at the start of the scene.
    public GameObject visualizationContainer;
    public GameObject sphere;
    public GameObject cylinder;
    public GameObject marker;
    public GameObject image;

    private Vector3 input;
    private Vector3 prevInput;

    private HashSet<SimulatedPointer> pointers;
    private bool visualizing = false;
    private Dictionary<SimulatedPointer, GameObject> visualMarkers;

    // Should be called when reloading scenes.
    public static void Reset() {
        actionList = new List<TrajectoryDetector>();
        OnTrigger = null;
    }

    void Start() {
        actionList.Add(this);
        pointers = new HashSet<SimulatedPointer>();
        visualMarkers = new Dictionary<SimulatedPointer, GameObject>();
        if (initVisualization) {
            Visualize();
        }
    }

	void Update() {
        if (LeapControl.handState == LeapControl.HandState.Pointing) {
            input = Vector3.ProjectOnPlane(LeapControl.fingerPoint, Vector3.forward);
            for (int s = minScaleLevel; s <= maxScaleLevel; s++) {
                SimulatedPointer newPointer = new SimulatedPointer(path[0], input);
                newPointer.multiplier = Mathf.Pow(scaleFactor, s);
                pointers.Add(newPointer);
            }
        } else {
            clear();
        }

        foreach (SimulatedPointer pointer in pointers) {
            if (Time.time - pointer.startTime > maxDuration) {
                pointer.progress = -1;
                continue;
            }
            Vector3 movement = input - prevInput;
            while (movement.sqrMagnitude > 0) {
                if (pointer.progress < 0) break;
                if (pointer.progress == (path.Length - 1) * 2) {
                    finish();
                    break;
                }
                if (pointer.progress % 2 == 0) {
                    movement = pointerMoveInSphere(pointer, movement * pointer.multiplier);
                } else {
                    movement = pointerMoveInCylinder(pointer, movement * pointer.multiplier);
                }
            }
        }

        if (visualizing) {
            foreach (SimulatedPointer pointer in pointers) {
                if (pointer.progress < 0) {
                    if (visualMarkers.ContainsKey(pointer)) {
                        Destroy(visualMarkers[pointer]);
                    }
                } else {
                    if (!visualMarkers.ContainsKey(pointer)) {
                        GameObject newMarker = Instantiate(marker);
                        newMarker.transform.SetParent(visualizationContainer.transform, false);
                        visualMarkers.Add(pointer, newMarker);
                    }
                    visualMarkers[pointer].transform.localPosition = pointer.position;
                }
            }
        }

        pointers.RemoveWhere(pointer => pointer.progress < 0);
        prevInput = input;
    }

    // Returns remaining movement
    private Vector3 pointerMoveInSphere(SimulatedPointer pointer, Vector3 movement) {
        Vector3 center = path[pointer.progress / 2];
        Vector3 right = Vector3.Cross(pointer.position - center, movement);
        Vector3 up = Vector3.Cross(movement, right).normalized;
        Vector3 forward = movement.normalized;
        Vector3 outDirection = pointer.progress < (path.Length - 1) * 2 ? (path[pointer.progress / 2 + 1] - center).normalized : Vector3.zero;
        float outThreshold = Mathf.Sqrt(jointRadius * jointRadius - pathRadius * pathRadius);
        float height = Vector3.Dot(pointer.position - center, up);
        float halfLength = Mathf.Sqrt(jointRadius * jointRadius - height * height);
        float distance = movement.magnitude;
        float distanceToBorder = halfLength - Vector3.Dot(pointer.position - center, forward);

        if (distance < distanceToBorder) {
            pointer.position += distance * forward;
            return Vector3.zero;
        } else {
            Vector3 contactPosition = pointer.position + distanceToBorder * forward;
            if (Vector3.Dot(contactPosition - center, outDirection) > outThreshold) {
                // Progress to the next segment
                pointer.position = contactPosition;
                pointer.progress++;
                pointer.overshoot = 0;
                return (distance - distanceToBorder) * forward;
            } else {
                Vector3 finalPoint = pointer.position + distance * forward;
                finalPoint = center + (finalPoint - center).normalized * jointRadius;
                pointer.position = finalPoint;
                pointer.overshoot += distance - distanceToBorder;
                if (Vector3.Dot(finalPoint - center, outDirection) > outThreshold) {
                    pointer.progress++;
                    pointer.overshoot = 0;
                } else if (pointer.progress >= 2 && pointer.overshoot > (center - path[pointer.progress / 2 - 1]).magnitude * jointOvershootFactor) {
                    pointer.progress = -1;
                }
                return Vector3.zero;
            }
        }
    }

    // Returns remaining movement
    private Vector3 pointerMoveInCylinder(SimulatedPointer pointer, Vector3 movement) {
        Vector3 a = path[pointer.progress / 2];
        Vector3 b = path[(pointer.progress + 1) / 2];
        Vector3 forward = (b - a).normalized;

        float distance = movement.magnitude;
        float distanceToExit = distance * Vector3.Dot(b - pointer.position, forward) / Vector3.Dot(movement, forward);
        if (float.IsInfinity(distanceToExit) || distanceToExit < 0) {
            distanceToExit = float.PositiveInfinity;
        }
        float distanceToStart = distance * Vector3.Dot(a - pointer.position, forward) / Vector3.Dot(movement, forward);
        if (float.IsInfinity(distanceToStart) || distanceToStart < 0) {
            distanceToStart = float.PositiveInfinity;
        }

        if (distance < distanceToExit && distance < distanceToStart) {
            pointer.position += movement;
            Vector3 outVec = Vector3.ProjectOnPlane(pointer.position - a, forward);
            if (outVec.sqrMagnitude > pathRadius * pathRadius) {
                pointer.position = a + Vector3.Project(pointer.position - a, forward) + outVec.normalized * pathRadius;
                pointer.overshoot += outVec.magnitude - pathRadius;
                if (pointer.overshoot > (b - a).magnitude * pathOvershootFactor) {
                    pointer.progress = -1;
                }
            }
            return Vector3.zero;
        } else if (distance >= distanceToExit) {
            pointer.position += forward * distanceToExit;
            Vector3 outVec = Vector3.ProjectOnPlane(pointer.position - a, forward);
            if (outVec.sqrMagnitude > pathRadius * pathRadius) {
                pointer.position = a + Vector3.Project(pointer.position - a, forward) + outVec.normalized * pathRadius;
            }
            pointer.progress++;
            pointer.overshoot = 0;
            return forward * (distance - distanceToExit);
        } else {
            pointer.progress = -1;
            return Vector3.zero;
        }
    }

    private void finish() {
        clearAll();
        if (OnTrigger != null) {
            OnTrigger(type);
        }
        if (image != null) {
            OverlayDisplay.Show(image, 0, 0.5f);
        }
    }

    public void clear() {
        foreach (SimulatedPointer pointer in pointers) {
            pointer.progress = -1;
        }
    }

    public static void clearAll() {
        foreach (TrajectoryDetector action in actionList) {
            action.clear();
        }
    }

    public void Visualize() {
        visualizing = true;
        for (int i = 0; i < path.Length; i++) {
            GameObject newSphere = Instantiate(sphere);
            newSphere.transform.SetParent(visualizationContainer.transform, false);
            newSphere.transform.localPosition = path[i];
            newSphere.transform.localScale = Vector3.one * (jointRadius / 0.5f);
        }
        for (int i = 0; i < path.Length - 1; i++) {
            GameObject newCylinder = Instantiate(cylinder);
            Vector3 dir = path[i + 1] - path[i];
            newCylinder.transform.SetParent(visualizationContainer.transform, false);
            newCylinder.transform.localPosition = path[i];
            newCylinder.transform.localRotation = Quaternion.LookRotation(dir);
            newCylinder.transform.localScale = new Vector3(pathRadius / 0.5f, pathRadius / 0.5f, dir.magnitude / 2);
        }
    }

    private class SimulatedPointer {
        public float startTime;
        public Vector3 position;
        public Vector3 inputDelta;
        public float multiplier = 1;
        public int progress = 0;
        public float overshoot = 0;

        public SimulatedPointer(Vector3 pos, Vector3 currentInputPosition) {
            startTime = Time.time;
            position = pos;
            inputDelta = pos - currentInputPosition;
        }
    }
}
