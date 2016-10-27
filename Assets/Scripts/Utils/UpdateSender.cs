using UnityEngine;
using System.Collections;

public class UpdateSender : MonoBehaviour {

    public delegate void UpdateListener();
    public static event UpdateListener OnUpdate;
	
    public static void Reset() {
        OnUpdate = null;
    }

	// Update is called once per frame
	void Update () {
	    if (OnUpdate != null) {
            OnUpdate();
        }
	}
}
