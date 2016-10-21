using UnityEngine;
using System.Collections;

public class UpdateSender : MonoBehaviour {

    public delegate void UpdateListener();
    public static event UpdateListener OnUpdate;
	
	// Update is called once per frame
	void Update () {
	    if (OnUpdate != null) {
            OnUpdate();
        }
	}
}
