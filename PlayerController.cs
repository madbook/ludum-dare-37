using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float speed;

	public float attackWidth;
	public float attackDepth;
	public GameObject attackObj; 
	
	public float dashRate;
	public float dashDuration;
	public float dashSpeed;
	private float nextDashAvailable = 0f;
	private float dashActiveUntil = 0f;
	private bool isDashing = false;
	private Vector3 dashStartPosition;
	private int defaultLayerIndex;
	private int dashLayerIndex;
	
	private float attackActiveUntil = 0f; 
	private bool attackSideAlternater;

	private Rigidbody rb;
	public Vector3 target = Vector3.zero;

	private bool dashAttack;
	private float canSwirlAfter;

	public Material slashMat;
	public Material dashAttackMat;
	public Material dashMat;

	public Material slashComboMat;
	public Material dashAttackComboMat;
	public Material dashComboMat;

	private AudioSource[] audioPlayers;
	private int audioPlayerIndex = 0;

	private float canComboAfter;
	private float canComboBefore;
	private int comboNumber = -1;
	private const int maxCombo = 3;
	private float lastAttack;

	void Start() {
		rb = GetComponent<Rigidbody>();
		defaultLayerIndex = gameObject.layer;
		dashLayerIndex = LayerMask.NameToLayer("Dash");

		// animator = GetComponent<Animator>();
	}

	void Awake() {
		audioPlayers = GetComponents<AudioSource>();
	}

	AudioSource GetAudioSource() {
		audioPlayerIndex += 1;
		if (audioPlayerIndex == audioPlayers.Length) {
			audioPlayerIndex = 0;
		}
		return audioPlayers[audioPlayerIndex];
	}

	void Update() {
		bool wasAttackPressed = GameController.inputManager.attackButton.wasPressed;
		bool wasSwirlPressed = GameController.inputManager.swirlButton.wasPressed;

		if (wasAttackPressed) {
			Quaternion rotation = transform.rotation;
			
			float actualAttackDepth = attackDepth;
			float actualAttackWidth = attackWidth;
			Vector3 attackPosition = transform.position;

			AudioSource audioPlayer = GetAudioSource();
			if (isDashing) {
				actualAttackDepth += dashDuration * dashSpeed;
				attackPosition = dashStartPosition;
				actualAttackWidth *= .5f;
				dashAttack = true; 
				audioPlayer.pitch = Random.Range(1f, 1.2f);
				audioPlayer.Play();
			} else {
				if (attackSideAlternater) {
					rotation *= Quaternion.Euler(0, 30f, 0);
				} else {
					rotation *= Quaternion.Euler(0, -30f, 0);
				}
				audioPlayer.pitch = Random.Range(.8f, .9f);
				audioPlayer.Play();
			}

			UpdateCombo(true);

			attackSideAlternater = !attackSideAlternater;
			
			if (comboNumber > 0) {
				actualAttackDepth += .5f;
				actualAttackDepth += .5f;
			}

			float attackDistance = actualAttackDepth / 2;

			Vector3 position = attackPosition + (transform.forward * attackDistance);
			GameObject newObj = Instantiate(attackObj, position, rotation) as GameObject;


			newObj.transform.localScale = new Vector3(
				actualAttackWidth,
				newObj.transform.localScale.y,
				actualAttackDepth
			);

			MeshRenderer renderer = newObj.GetComponentInChildren<MeshRenderer>(); 
			
			if (attackSideAlternater) {
				renderer.gameObject.transform.localScale = new Vector3(-1, 1, 10);
			}

			if (dashAttack) {
				int damage = 8;
				if (comboNumber > 0) {
					damage += comboNumber - 1;
					renderer.material = dashAttackComboMat;
				} else {
					renderer.material = dashAttackMat;
				}
				newObj.GetComponent<PlayerAttack>().damage = damage;
			} else {
				if (comboNumber > 0) {
					renderer.material = slashComboMat;
				} else {
					renderer.material = slashMat;
				}
				newObj.GetComponent<PlayerAttack>().damage = 4 + comboNumber;
			}

			attackActiveUntil = Time.time + (dashDuration / 2f);
		} else if (wasSwirlPressed && !isDashing && Time.time > canSwirlAfter) {
			attackSideAlternater = !attackSideAlternater;
			UpdateCombo(true);
			AudioSource audioPlayer = GetAudioSource();
			audioPlayer.pitch = Random.Range(.6f, .7f);
			audioPlayer.Play();

			Quaternion rotation = transform.rotation;
			
			float actualAttackDepth = attackDepth * 1.7f;
			float actualAttackWidth = actualAttackDepth;

			if (comboNumber > 0) {
				actualAttackDepth += .5f;
				actualAttackDepth += .5f;
			}

			Vector3 position = transform.position - new Vector3(0, .2f, 0);

			GameObject newObj = Instantiate(attackObj, position, rotation) as GameObject;

			newObj.transform.localScale = new Vector3(
				actualAttackWidth,
				newObj.transform.localScale.y,
				actualAttackDepth
			);

			MeshRenderer renderer = newObj.GetComponentInChildren<MeshRenderer>();

			if (comboNumber > 0) {
				renderer.material = dashComboMat;
			} else {
				renderer.material = dashMat;
			}
			
			if (attackSideAlternater) {
				renderer.gameObject.transform.localScale = new Vector3(-1, 1, 10);
			}

			newObj.GetComponent<PlayerAttack>().damage = 2 + (comboNumber * 2); // knockback only

			// TODO - make better
			canSwirlAfter = Time.time + dashRate * 2f;
			comboNumber = 0;
		} else {
			UpdateCombo(false);
		}
	}

	void UpdateCombo(bool wasAttackPressed) {
		float t = Time.time;

		if (wasAttackPressed) {
			if (lastAttack > 0f) {
				float d = t - lastAttack;
				Debug.Log("seconds since last attack = " + d.ToString());
			}
			lastAttack = t;

			if (comboNumber == maxCombo) {
				// Debug.Log("combo max!");
				comboNumber = -1;
			} else if (comboNumber < 0) {
				// Debug.Log("normal attack");
				canComboAfter = t + .1f;
				canComboBefore = canComboAfter + .15f;
				comboNumber = 0;
			} else if (t >= canComboAfter && t < canComboBefore) {
				canComboAfter = t + .1f;
				canComboBefore = canComboAfter + .15f;
				comboNumber += 1;
				Debug.Log("combo " + comboNumber.ToString());
			} else {
				comboNumber = -1;
				// Debug.Log("missed!");
			}
		} else if (comboNumber >= 0 && t >= canComboBefore) {
			Debug.Log("combo timed out");
			comboNumber = -1;
		}
	}

	void FixedUpdate() {
		bool wasDashPressed = GameController.inputManager.dashButton.wasPressed;

		bool isDashStarting = !isDashing && wasDashPressed && Time.time >= nextDashAvailable;
		bool isDashEnding = isDashing && Time.time > dashActiveUntil;
		isDashing = !isDashEnding && (isDashing || isDashStarting);

		bool isSlashing = Time.time < attackActiveUntil;

		if (isDashStarting) {
			dashStartPosition = transform.position;
			gameObject.layer = dashLayerIndex;
			nextDashAvailable = Time.time + dashRate;
			dashActiveUntil = Time.time + dashDuration;
		} else if (isDashEnding) {
			gameObject.layer = defaultLayerIndex;
		}

		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");
		Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
		bool isMoving = movement.normalized != Vector3.zero;
		
		if (isDashing) {
			rb.velocity = transform.forward * dashSpeed;
		} else if (isSlashing) {
			rb.velocity = transform.forward * (dashSpeed / 2f);
		} else {
			rb.velocity = movement * speed;
		}

		// if (isMoving) {
		// 	animator.SetTrigger("StartWalking");
		// } else {
		// 	animator.SetTrigger("StopWalking");
		// }

		if (isMoving && !isDashing) {
			transform.rotation = Quaternion.LookRotation(movement.normalized);
		}
		
		if (isDashEnding && !dashAttack) {
			dashAttack = false;
		} else if (isDashEnding) {
			dashAttack = false;
		}

		// Update the target position that enemies move towards.
		target = Vector3.MoveTowards(target, transform.position, speed); 
	}
}
