using UnityEngine;
using System.Collections;

public class PrefabLinks : MonoBehaviour {

    public static PrefabLinks links;

    public GameObject grabbingProjectile;
    public GameObject enemy1;
    public GameObject lockEffect;


    void Awake() {
        links = this;
    }
}
