using UnityEngine;
using System.Collections;

/// <summary>
/// Register to the OnUpdate/OnFixedUpdate event to make a non-MonoBehaviour class also receive updates.
/// </summary>
public class UpdateSender : MonoBehaviour {

    public delegate void UpdateListener();
    public static event UpdateListener OnUpdate;
    public static event UpdateListener OnFixedUpdate;

    // Should be called when reloading scenes.
    public static void Reset() {
        OnUpdate = null;
        OnFixedUpdate = null;
    }

	void Update () {
	    if (OnUpdate != null) {
            OnUpdate();
        }
	}

    void FixedUpdate() {
        if (OnFixedUpdate != null) {
            OnFixedUpdate();
        }
    }

}
