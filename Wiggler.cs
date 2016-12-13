using UnityEngine;
using System.Collections;

public class Wiggler : MonoBehaviour {
    private float lifetime = .2f;
    private float duration = .05f;
    private Vector3 minVector = new Vector3(1f, .6f, 1f);
    private Vector3 maxVector = new Vector3(1f, 1.4f, 1f);

    void Start() {
        enabled = false;
    }

    public void Enable() {
        if (enabled) {
            return;
        }
        enabled = true;
		StartCoroutine(Countdown());
    }

	IEnumerator Countdown() {
		yield return new WaitForSeconds(lifetime);
        transform.localScale = new Vector3(1f, 1f, 1f);
        enabled = false;
	}

    void Update() {
        float lerp = Mathf.PingPong(Time.time, duration) / duration;        
        transform.localScale = Vector3.Lerp(minVector, maxVector, lerp);
    }
}