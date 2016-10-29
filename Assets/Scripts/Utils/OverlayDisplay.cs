using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// This component manages the display of the overlay UI images.
/// It supports fade out effects. (I planned to add fade in if it is needed in the future)
/// </summary>
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


    /// <summary>
    /// Show a UI component for certain duration, and then fade out during certain duraion.
    /// duration = 0 means never disappear.
    /// </summary>
    public static void Show(GameObject uiComponent, float duration, float fadeDuration) {
        if (!uiElements.Contains(uiComponent)) {
            uiElements.Add(uiComponent);
            fadeDurations.Add(fadeDuration);
            timers.Add(duration + fadeDuration);
        }
        uiComponent.SetActive(true);
        uiComponent.GetComponent<CanvasRenderer>().SetAlpha(1);
    }

    /// <summary>
    /// Show a UI component indefinately until Hide is called.
    /// </summary>
    public static void ShowIndefinately(GameObject uiComponent) {
        if (!uiElements.Contains(uiComponent)) {
            uiElements.Add(uiComponent);
            fadeDurations.Add(-1);
            timers.Add(-1);
        }
        uiComponent.SetActive(true);
        uiComponent.GetComponent<CanvasRenderer>().SetAlpha(1);
    }

    /// <summary>
    /// Hide an already-shown UI component with certain fade out time.
    /// </summary>
    public static void Hide(GameObject uiComponent, float fadeDuration) {
        int index = uiElements.IndexOf(uiComponent);
        if (index >= 0) {
            fadeDurations[index] = fadeDuration;
            timers[index] = fadeDuration;
        }
    }

}
