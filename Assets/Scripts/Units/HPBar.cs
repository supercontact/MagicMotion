using UnityEngine;
using System.Collections;

public class HPBar : MonoBehaviour {


	public static ColorMixer colorMix;

	//camera position
	public Transform cameraMain;
	public Unit unit;
	public MeshRenderer barRenderer;
    public Transform bar;
	 

	// Use this for initialization
	void Start () {
        cameraMain = GameObject.FindGameObjectWithTag("MainCamera").transform;
        unit = GetComponentInParent<Unit>();

        bar.LookAt(cameraMain.transform);
        bar.localScale = new Vector3(1,1,1);
        if (colorMix == null) {
            colorMix = new ColorMixer();
            colorMix.InsertColorNode(Color.red, 0.2f);
            colorMix.InsertColorNode(new Color(1, 1, 0), 0.6f);
            colorMix.InsertColorNode(Color.green, 1);
        }

	}
	
	// Update is called once per frame
	void Update () {
		float hpRatio = (float)unit.HP / unit.maxHP;
		hpRatio = Mathf.Clamp01 (hpRatio);
        bar.LookAt(cameraMain.transform);
        bar.localScale = new Vector3 (hpRatio, 1, 1);
        barRenderer.material.color = colorMix.GetColor (hpRatio);
	}
}


