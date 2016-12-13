using UnityEngine;

public class EnemyController : MonoBehaviour {
	public float speed;
	public GameObject dropOnDeathObj;

	private PlayerController follow;
	private Rigidbody rb;

	private int health = 10;
	private float stunUntil; 
	private const float stunDuration = .75f;
	private Vector3 knockbackDirection;
	private const float knockbackSpeed = 1f;

	public float targetDistance;
	public GameObject attackObj;
	public float attackWarmUp;
	public float attackDepth;
	public float attackWidth;
	private bool isAttacking;
	private float attackEndTime;

	void Start() {
		rb = GetComponent<Rigidbody>();
		// accomodates enemies spawned after player destroyed
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		if (player != null) {
			follow = player.GetComponent<PlayerController>();
		}
	}

	void FixedUpdate() {
		if (Time.time < stunUntil) {
			rb.velocity = knockbackDirection * knockbackSpeed;
			return;
		} 
 
		if (!follow) {
			return;
		}

		Vector3 movement = follow.target - transform.position;
		
		if (isAttacking) {
			if (Time.time > attackEndTime) {
				float attackDistance = attackDepth / 2;
				Vector3 position = transform.position + (transform.forward * attackDistance);
				GameObject newObj = Instantiate(attackObj, position, transform.rotation) as GameObject;

				newObj.transform.localScale = new Vector3(
					attackWidth,
					newObj.transform.localScale.y,
					attackDepth
				);

				EndAttack();
			}
		} else if (movement.magnitude <= targetDistance) {
			StartAttack();
		}

		if (!isAttacking) {
			rb.velocity = movement.normalized * speed;
		} else {
			rb.velocity = Vector3.zero;
		}

		if (movement.normalized != Vector3.zero) {
			rb.rotation = Quaternion.LookRotation(movement.normalized);
		}
	}

	void StartAttack() {
		isAttacking = true;
		attackEndTime = Time.time + attackWarmUp;
		transform.localScale = new Vector3(1, .5f, 1);
	}

	void EndAttack() {
		isAttacking = false;
		transform.localScale = new Vector3(1, 1, 1);
	}

	public void OnHit(int damage) {
		health -= damage;
		EndAttack();

		if (health <= 0) {
			GameController.KillEnemy(gameObject);
		} else {
			GetComponent<Wiggler>().Enable();
			knockbackDirection = (transform.position - follow.target).normalized;
			transform.position += knockbackDirection * knockbackSpeed;
			stunUntil = Time.time + stunDuration;
		}
	}
}
