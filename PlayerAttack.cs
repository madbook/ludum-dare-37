using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {
	public float lifetime;
	public int damage = 1;
	public bool didHit;
	private AudioSource audioSource;

	void Start() {
		StartCoroutine(Countdown());
	}

	void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

	IEnumerator Countdown() {
		yield return new WaitForSeconds(lifetime);
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other) {
		bool didHitNow = false;
		
		if (other.CompareTag("Enemy")) {
			GameController.HitEnemy(other.gameObject, damage);
			didHitNow = true;
		}
		if (other.CompareTag("Spawner")) {
			GameController.HitSpawner(other.gameObject, damage);
			didHitNow = true;
		}

		if (!didHit && didHitNow) {
			didHit = true;
			audioSource.Play();
		}
	}
}
