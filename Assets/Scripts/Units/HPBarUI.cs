using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPBarUI : MonoBehaviour {


	public static ColorMixer colorMix;

	public Unit unit;
	public Image barImage;
    public RectTransform bar;
    public Text hpText;

    private float initialOffset;

	// Use this for initialization
	void Start () {
        if (colorMix == null) {
            colorMix = new ColorMixer();
            colorMix.InsertColorNode(Color.red, 0.2f);
            colorMix.InsertColorNode(new Color(1, 1, 0), 0.6f);
            colorMix.InsertColorNode(Color.green, 1);
        }
        initialOffset = bar.offsetMax.x;
    }
	
	// Update is called once per frame
	void Update () {
		float hpRatio = (float)unit.HP / unit.maxHP;
		hpRatio = Mathf.Clamp01 (hpRatio);
        bar.offsetMax = new Vector2(hpRatio * initialOffset, bar.offsetMax.y);
        bar.offsetMin = new Vector2(-hpRatio * initialOffset, bar.offsetMin.y);
        barImage.color = colorMix.GetColor (hpRatio);
        hpText.text = Mathf.Max(unit.HP, 0) + "/" + unit.maxHP;
	}
}


