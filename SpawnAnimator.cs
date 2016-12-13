using UnityEngine;

public class SpawnAnimator : MonoBehaviour {
	private float doneSpawningAfter;
	private Vector3 spawnPosition;

    public MonoBehaviour controller;

	void Start() {
        controller.enabled = false;
		doneSpawningAfter = Time.time + 1f;
		spawnPosition = transform.position;
		transform.position = new Vector3(
			transform.position.x,
			-1f,
			transform.position.z
		);
		transform.localScale = new Vector3(.5f, .5f, .5f);
    }

    void Update() {
        if (Time.time > doneSpawningAfter) {
            transform.position = spawnPosition;
            transform.localScale = Vector3.one;
            controller.enabled = true;
            enabled = false;
        } else {
            transform.position = Vector3.Lerp(transform.position, spawnPosition, .1f);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, .1f);
        }
    }
}
