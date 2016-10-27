using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OverlayDisplay : MonoBehaviour {
    
    private static List<GameObject> uiElements;
    private static List<float> fadeDurations;
    private static List<float> timers;

	// Use this for initialization
	void Start () {
        uiElements = new List<GameObject>();
        fadeDurations = new List<float>();
        timers = new List<float>();
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = uiElements.Count - 1; i >= 0; i--) {
            if (fadeDurations[i] < 0) continue;
            timers[i] -= Time.unscaledDeltaTime;
            float newAlpha = Mathf.Min(timers[i] / fadeDurations[i], 1);
            if (newAlpha < 0) {
                uiElements[i].gameObject.SetActive(false);
                uiElements.RemoveAt(i);
                fadeDurations.RemoveAt(i);
                timers.RemoveAt(i);
            } else {
                uiElements[i].GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
            }
        }
	}

    // duration = 0 means never disappear;
    public static void Show(GameObject uiComponent, float duration, float fadeDuration) {
        if (!uiElements.Contains(uiComponent)) {
            uiElements.Add(uiComponent);
            fadeDurations.Add(fadeDuration);
            timers.Add(duration + fadeDuration);
        }
        uiComponent.SetActive(true);
        uiComponent.GetComponent<CanvasRenderer>().SetAlpha(1);
    }

    public static void ShowIndefinately(GameObject uiComponent) {
        if (!uiElements.Contains(uiComponent)) {
            uiElements.Add(uiComponent);
            fadeDurations.Add(-1);
            timers.Add(-1);
        }
        uiComponent.SetActive(true);
        uiComponent.GetComponent<CanvasRenderer>().SetAlpha(1);
    }

    public static void Hide(GameObject uiComponent, float fadeDuration) {
        int index = uiElements.IndexOf(uiComponent);
        if (index >= 0) {
            fadeDurations[index] = fadeDuration;
            timers[index] = fadeDuration;
        }
    }

}
