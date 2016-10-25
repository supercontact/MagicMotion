using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Links : MonoBehaviour {

    public static Links links;

    public Canvas canvas;

    public GameObject enemy1;
    public GameObject helper1;
    public GameObject SmallEnemy;
    public GameObject BigEnemy;
    public GameObject ThrowingEnemy;
    public GameObject ShootingEnemy;
	public GameObject HomingEnemy;
    public GameObject grabbingProjectile;
    public GameObject StoneProjectile;
    public GameObject SmallBullet;
	public GameObject HomingBullet;
    public GameObject lockEffect;
    public GameObject aimMarker;
    public GameObject spikes;
    public GameObject aimLine;
    public GameObject beam;
    public GameObject shockRing;
    public GameObject fireBall;
    public GameObject explosion;
    public GameObject crystalBullet;
    public GameObject magicRing;
    public GameObject damageText;
    public GameObject healthEffect;
    public GameObject lightning;
    public GameObject lightningMark;


    public Image ligntningImage, starImage, spikeImage, circleImage, crossImage, healImage, flashImage, handImage;

    void Awake() {
        links = this;
    }
}
