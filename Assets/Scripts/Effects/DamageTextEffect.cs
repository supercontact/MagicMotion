using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// The effect of the damage text UI.
/// </summary>
public class DamageTextEffect : MonoBehaviour {

    public Vector2 initVelocity = Vector2.up * 300;
    public float velocityRandomRange = 200;
    public float opaqueDuration = 0.5f;
    public float fadingDuration = 0.2f;
    public float gravity = 1000;
    public Text text;

    private Vector2 velocity;
    private float timer;
    private RectTransform trans;

    void Awake() {
        trans = GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start () {
        timer = opaqueDuration + fadingDuration;
        velocity = initVelocity;
        velocity += Random.insideUnitCircle * velocityRandomRange;
        trans.SetParent(Links.links.canvas.transform);
    }
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            Destroy(gameObject);
            return;
        }
        velocity += Vector2.down * gravity * Time.deltaTime;
        trans.anchoredPosition = trans.anchoredPosition + velocity * Time.deltaTime;
        if (timer < fadingDuration) {
            float alpha = timer / fadingDuration;
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }
    }

    public void SetPosition(Vector3 worldPosition) {
        Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Vector3 cameraPoint = cam.WorldToScreenPoint(worldPosition);
        trans.anchoredPosition = cameraPoint;
        trans.localScale = Vector3.one * (5 / cameraPoint.z);
    }

    public void SetText(string str) {
        text.text = str;
    }
}
