using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OverlayDisplay : MonoBehaviour {

    private static List<Image> images;
    private static List<float> fadeDurations;
    private static List<float> timers;

	// Use this for initialization
	void Start () {
        images = new List<Image>();
        fadeDurations = new List<float>();
        timers = new List<float>();
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = images.Count - 1; i >= 0; i--) {
            if (fadeDurations[i] < 0) continue;
            timers[i] -= Time.deltaTime;
            float newAlpha = Mathf.Min(timers[i] / fadeDurations[i], 1);
            if (newAlpha < 0) {
                images[i].gameObject.SetActive(false);
                images.RemoveAt(i);
                fadeDurations.RemoveAt(i);
                timers.RemoveAt(i);
            } else {
                images[i].color = new Color(1, 1, 1, newAlpha);
            }
        }
	}

    // duration = 0 means never disappear;
    public static void ShowImage(Image image, float duration, float fadeDuration) {
        if (!images.Contains(image)) {
            images.Add(image);
            fadeDurations.Add(fadeDuration);
            timers.Add(duration + fadeDuration);
        }
        image.gameObject.SetActive(true);
        image.color = Color.white;
    }

    public static void ShowImageIndefinately(Image image) {
        if (!images.Contains(image)) {
            images.Add(image);
            fadeDurations.Add(-1);
            timers.Add(-1);
        }
        image.gameObject.SetActive(true);
        image.color = Color.white;
    }

    public static void HideImage(Image image, float fadeDuration) {
        int index = images.IndexOf(image);
        if (index >= 0) {
            fadeDurations[index] = fadeDuration;
            timers[index] = fadeDuration;
        }
    }

}
