using UnityEngine;
using System.Collections;

public class InputManager {
    public static Vector3 GetHandPositionOnScreen() {
        if (LeapControl.handState == LeapControl.HandState.Pointing) {
            return new Vector3(LeapControl.fingerPoint.x + Screen.width / 2, LeapControl.fingerPoint.y + Screen.height / 2, LeapControl.fingerPoint.z);
        } else {
            return new Vector3(LeapControl.handPoint.x + Screen.width / 2, LeapControl.handPoint.y + Screen.height / 2, LeapControl.handPoint.z);
        }
    }
}
