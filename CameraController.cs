using UnityEngine;

public class CameraController : MonoBehaviour {
	public PlayerController follow;
	
	private const float speed = 5f;
	private Vector3 offset;
	private bool offsetCalculated = false;
	private Camera c;

	private const float lerpSpeed = 1f;
	private const float restingSize = 6f;
	private const float hitZoomAmount = .1f;
	private const float minSize = 3.5f;

	private bool isZooming;
	private float resetZoomAfter;
	public float targetSize = restingSize;
	private float zoomDelay = .5f;
	
	void Start() {
		c = GetComponent<Camera>();
		UpdateFollow();
	}

	public void UpdateFollow() {
		GameObject obj = GameObject.FindGameObjectWithTag ("Player");
		if (obj != null) {
			follow = obj.GetComponent<PlayerController>();
			if (!offsetCalculated) {
				offset = transform.position - follow.transform.position;
				offsetCalculated = true;
			}
		} 
	}

	void Update () {
		Vector3 target = (follow.target + offset);
		float t = speed * Time.deltaTime;
		transform.position = new Vector3(
			Mathf.Lerp(transform.position.x, target.x, t), 
			transform.position.y,
			Mathf.Lerp(transform.position.z, target.z, t)
		);

		if (GameController.inputManager.attackButton.wasPressed) {
			isZooming = true;
			resetZoomAfter = Time.time + zoomDelay;
			targetSize = Mathf.Max(targetSize - hitZoomAmount, minSize);
		} else if (isZooming && Time.time > resetZoomAfter) {
			isZooming = false;
			targetSize = restingSize;
		}

		c.orthographicSize = Mathf.Lerp(c.orthographicSize, targetSize, Time.deltaTime * lerpSpeed);
	}
}
