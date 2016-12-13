using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour {
	public float spawnOverSeconds;
	public int spawnCount;

	private int health = 15;
	private int armor = 1;

	public Material material;

	private float spawnIntervalSeconds;

	void Start() {
		spawnIntervalSeconds = spawnOverSeconds / spawnCount;
		StartCoroutine(Spawn());
	}
	
	IEnumerator Spawn() {
		int i = spawnCount;
		while (i > 0) {
			i = i - 1;
			yield return new WaitForSeconds(spawnIntervalSeconds);
			Vector2 position = new Vector2(
				transform.position.x,
				transform.position.z
			);
			GameController.SpawnEnemy(position);
		}
		GameController.KillSpawner(gameObject);
	}

	public void OnHit(int damage) {
		damage -= armor;
		if (damage <= 0) {
			return;
		}

		health -= damage;

		if (health <= 0) {
			GameController.KillSpawner(gameObject, true);
		} else {
			GetComponent<Wiggler>().Enable();
		}
	}
}
