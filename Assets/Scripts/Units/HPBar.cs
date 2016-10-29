using UnityEngine;
using System.Collections;

/// <summary>
/// This component controls the HP bar (3D version) of a unit, which updates automatically.
/// It can always rotate towards the camera, and it shows different color with different amount of HP.
/// </summary>
public class HPBar : MonoBehaviour {

	public static ColorMixer colorGradient;

	public Transform cameraMain;
	public Unit unit;
	public MeshRenderer barRenderer;
    public Transform bar;
    public bool autoRotate = true;
	 

	// Use this for initialization
	void Start () {
        cameraMain = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Unit parentUnit = GetComponentInParent<Unit>();
        if (parentUnit != null) unit = parentUnit;

        if (autoRotate) {
            bar.LookAt(cameraMain.transform);
        }
        bar.localScale = new Vector3(1,1,1);
        if (colorGradient == null) {
            colorGradient = new ColorMixer();
            colorGradient.InsertColorNode(Color.red, 0.2f);
            colorGradient.InsertColorNode(new Color(1, 1, 0), 0.6f);
            colorGradient.InsertColorNode(Color.green, 1);
        }

	}
	
	// Update is called once per frame
	void Update () {
		float hpRatio = (float)unit.HP / unit.maxHP;
		hpRatio = Mathf.Clamp01 (hpRatio);
        if (autoRotate) {
            bar.LookAt(cameraMain.transform);
        }
        bar.localScale = new Vector3 (hpRatio, 1, 1);
        barRenderer.material.color = colorGradient.GetColor (hpRatio);
	}
}


