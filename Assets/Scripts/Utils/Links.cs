using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This class maintains static links for prefabs and scenes elements.
/// Links should be set in the unity inspector.
/// </summary>
public class Links : MonoBehaviour {

    public static Links links;

    public Canvas canvas;

    // Unit prefabs
    public GameObject enemy1;
    public GameObject helper1;
    public GameObject smallEnemy;
    public GameObject bigEnemy;
    public GameObject throwingEnemy;
    public GameObject shootingEnemy;
	public GameObject homingEnemy;

    // Projectile prefabs
    public GameObject grabbingProjectile;
    public GameObject stoneProjectile;
    public GameObject smallBullet;
    public GameObject crystalBullet;
	public GameObject homingBullet;
    public GameObject fireBall;

    // Effect prefabs
    public GameObject explosion;
    public GameObject explosionMissile;
    public GameObject lockEffect;
    public GameObject aimMarker;
    public GameObject spikes;
    public GameObject aimLine;
    public GameObject beam;
    public GameObject shockRing;
    public GameObject magicRing;
    public GameObject lightning;
    public GameObject lightningMark;
    public GameObject healthEffect;
    public GameObject speedEffect;
    public GameObject damageText;

    // UI elements
    public GameObject ligntningImage, starImage, spikeImage, circleImage, crossImage, healImage, infinityImage, flashImage, handImage;
    public GameObject[] calibrationInstructions;
    public GameObject calibrationWarning;
    public GameObject gameOver, gameWon;

    void Awake() {
        links = this;
    }
}
