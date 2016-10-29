using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This component controls the HP bar (UI version) of a unit, which updates automatically.
/// It shows different color with different amount of HP.
/// </summary>
public class HPBarUI : MonoBehaviour {

	public static ColorMixer colorGradient;

	public Unit unit;
	public Image barImage;
    public RectTransform bar;
    public Text hpText;

    private float initialOffset;

	// Use this for initialization
	void Start () {
        if (colorGradient == null) {
            colorGradient = new ColorMixer();
            colorGradient.InsertColorNode(Color.red, 0.2f);
            colorGradient.InsertColorNode(new Color(1, 1, 0), 0.6f);
            colorGradient.InsertColorNode(Color.green, 1);
        }
        initialOffset = bar.offsetMax.x;
    }
	
	// Update is called once per frame
	void Update () {
		float hpRatio = (float)unit.HP / unit.maxHP;
		hpRatio = Mathf.Clamp01 (hpRatio);
        bar.offsetMax = new Vector2(hpRatio * initialOffset, bar.offsetMax.y);
        bar.offsetMin = new Vector2(-hpRatio * initialOffset, bar.offsetMin.y);
        barImage.color = colorGradient.GetColor (hpRatio);
        hpText.text = Mathf.Max(unit.HP, 0) + "/" + unit.maxHP;
	}
}


