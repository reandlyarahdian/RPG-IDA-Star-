using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SpawnEnemy : MonoBehaviour {

	[SerializeField]
	private GameObject enemyEncounterPrefab;

	private bool spawning = false;

	void Start() {
		DontDestroyOnLoad (this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		if (scene.name == "Battle") {
			if (this.spawning) {
				Instantiate (enemyEncounterPrefab);
			}
			SceneManager.sceneLoaded -= OnSceneLoaded;
			Destroy (this.gameObject);
		}
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.spawning = true;
            AudioManager.instance.Stop("Forest");
            SceneManager.LoadScene("Battle");
        }
    }
}
