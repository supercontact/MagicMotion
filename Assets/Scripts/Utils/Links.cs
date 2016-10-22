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
    public GameObject aimLine;
    public GameObject beam;
    public GameObject shockRing;

    public Image ligntningImage, starImage, spikeImage, circleImage, crossImage, healImage, flashImage, handImage;


    void Awake() {
        links = this;
    }
}
