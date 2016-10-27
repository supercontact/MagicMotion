using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneControl : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponentInChildren<Player>().isDead) {
            Links.links.gameOver.SetActive(true);
        }
        Unit[] units = GetComponentsInChildren<Unit>();
        bool win = true;
        for (int i = 0; i < units.Length; i++) {
            if (units[i].team != 1 && !units[i].isDead) {
                win = false;
                break;
            }
        }
        if (win) {
            Links.links.gameWon.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            Reload();
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            Time.timeScale = Time.timeScale > 0 ? 0 : 1;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public static void EnterSlowMotion() {
        Time.timeScale = 0.1f;
    }

    public static void Play() {
        Time.timeScale = 1;
    }

    public static void Pause() {
        Time.timeScale = 0;
    }

    public static void Reload() {
        TrajectoryDetector.Reset();
        HandUpDetector.Reset();
        GrabDetector.Reset();
        UpdateSender.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
