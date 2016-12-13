using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {
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
		if (other.CompareTag("Player")) {
			if (!didHit) {
				didHit = true;
				audioSource.Play();
			}
			GameController.instance.TakeDamage(damage);
			other.GetComponent<Wiggler>().Enable();
		}
	}
}
