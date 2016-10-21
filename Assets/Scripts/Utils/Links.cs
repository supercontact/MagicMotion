using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Links : MonoBehaviour {

    public static Links links;

    public GameObject grabbingProjectile;
    public GameObject enemy1;
    public GameObject lockEffect;
    public GameObject aimMarker;
    public GameObject spikes;

    public Image ligntningImage, starImage, spikeImage, circleImage, flashImage, handImage;


    void Awake() {
        links = this;
    }
}
