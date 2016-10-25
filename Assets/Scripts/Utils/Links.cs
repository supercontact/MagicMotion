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
    public GameObject crystalBullet;
	public GameObject HomingBullet;
    public GameObject lockEffect;
    public GameObject aimMarker;
    public GameObject spikes;
    public GameObject aimLine;
    public GameObject beam;
    public GameObject shockRing;
    public GameObject fireBall;
    public GameObject explosion;
    public GameObject magicRing;
    public GameObject lightning;
    public GameObject lightningMark;
    public GameObject healthEffect;
    public GameObject speedEffect;
    public GameObject damageText;


    public Image ligntningImage, starImage, spikeImage, circleImage, crossImage, healImage, infinityImage, flashImage, handImage;
    public GameObject gameOver;

    void Awake() {
        links = this;
    }
}
