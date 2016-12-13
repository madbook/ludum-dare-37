using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public static GameController instance;
	public static InputManager inputManager = new InputManager();

	public GameObject playerTemplate;
	public Vector2 playerSpawn;
	
	private float nextSpawnWave = 0f;
	public GameObject spawner;
	public int maxSpawners = 1;
	private int numSpawners = 0;
	private const float spawnerChance = .3f;

	public GameObject enemy;
	private int score = 0;
	private int highScore = 0;
	private int nextLevel = 4; 
	public Text scoreText;
	public Text highScoreText;

	private int health = 1;
	public Text healthText;

	private GameObject player;
	private bool playing;
	private float restartAfter;

	private const float boardSize = 15f;

	public Camera c;
	
	void Start () {
		instance = this;
		Init();
	}

	void Init() {
		maxSpawners = 2;
		numSpawners = 0;
		score = 0;
		nextLevel = 4; 
		health = 1;

		UpdateHealth();
		UpdateScore();

		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}
		// Spawn player
		player = Instantiate(
			playerTemplate,
			new Vector3(playerSpawn.x, 0, playerSpawn.y),
			Quaternion.identity
		) as GameObject;
		player.transform.parent = transform;
		c.GetComponent<CameraController>().UpdateFollow();
		playing = true;
	}

	void Update() {
		inputManager.Update();
		GibSpawner.Update();

		if (!playing) {
			if (Time.time > restartAfter) {
				Init();
			}
			return;
		}

		if (Time.time >= nextSpawnWave) {
			SpawnWave();
			nextSpawnWave = Time.time + 12;
		}
	}

	void SpawnWave() {
		// Spawn spawners
		for (int i = numSpawners; i < maxSpawners; i++) {
			Vector2 position = GetRandomBoardPosition();
			SpawnSpawner(position);
		}
	}

	public static Vector2 GetRandomBoardPosition() {
		float n = (boardSize - 1) / 2f;
		return new Vector2(
			Random.Range(-n, n),
			Random.Range(-n, n)
		);
	}

	public static void SpawnSpawner(Vector2 position) {
		GameObject obj = GameObject.Instantiate(
			instance.spawner,
			new Vector3 (
				position.x,
				instance.spawner.transform.localPosition.y,
				position.y
			),
			instance.spawner.transform.localRotation
		) as GameObject;
		instance.numSpawners += 1;
		obj.transform.parent = instance.transform;
	}

	public static void KillSpawner(GameObject obj, bool playerKilled = false) {
		Destroy(obj);
		instance.numSpawners -= 1;
		if (playerKilled) {
			instance.AddScore(5);
		}
	}

	public static void SpawnEnemy(Vector2 position) {
		GameObject obj = GameObject.Instantiate(
			instance.enemy,
			new Vector3(
				position.x,
				0,
				position.y
			),
			Quaternion.identity
		) as GameObject;
		obj.transform.parent = instance.transform;
	}

	public static void HitEnemy(GameObject obj, int damage) {
		obj.GetComponent<EnemyController>().OnHit(damage);

		if (instance.player == null) {
			return;
		}

		for (int i = damage * 3; i > 0; i--) {
			Vector3 direction = obj.transform.position - instance.player.transform.position;
			GibSpawner.SpawnGib(obj.transform.position, direction.normalized, obj.GetComponent<MeshRenderer>().sharedMaterial);
		}
	}
	
	public static void HitSpawner(GameObject obj, int damage) {
		obj.GetComponent<SpawnController>().OnHit(damage);

		for (int i = (damage-1) * 3; i > 0; i--) {
			Vector3 direction = obj.transform.position - instance.player.transform.position;
			GibSpawner.SpawnGib(obj.transform.position, direction.normalized, obj.GetComponent<SpawnController>().material);
		}
	}

	public static void KillEnemy(GameObject obj) {
		instance.AddScore(1);
		if (instance.numSpawners == 0 || (Random.value < spawnerChance && instance.numSpawners < instance.maxSpawners)) {
			SpawnSpawner(
				new Vector2(obj.transform.position.x, obj.transform.position.z)
			);
		}
		Destroy(obj);
	}

	void AddScore(int points) {
		score += points;
		if (score >= nextLevel) {
			nextLevel *= 2;
			maxSpawners += 1;
		}
		if (score > highScore) {
			highScore = score;
		}
		UpdateScore();
	}

	void UpdateScore() {
		scoreText.text = score.ToString();
		highScoreText.text = highScore.ToString();
	}

	public void TakeDamage(int damage) {
		health -= damage;
		if (health <= 0) {
			GameOver();
		}
		UpdateHealth();
	}

	void UpdateHealth() {
		healthText.text = health.ToString();
	}

	void GameOver() {
		Destroy(player.gameObject);
		nextSpawnWave = 0f;
		playing = false;
		restartAfter = Time.time + 2f;
	}
}
