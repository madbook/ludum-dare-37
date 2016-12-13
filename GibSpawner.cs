using UnityEngine;
using System.Collections.Generic;

public class GibSpawner {
	private const float speed = .1f;

	class Gib {
		public int ticks;
		public GameObject obj;

		public Gib(int ticks, GameObject obj) {
			this.ticks = ticks;
			this.obj = obj;
		}

		public void Update() {
			ticks -= 1;
			obj.transform.position += obj.transform.forward * speed;
		}
	}

	private static List<Gib> gibs = new List<Gib>();

	public static void SpawnGib(Vector3 startPosition, Vector3 direction, Material material = default(Material)) {
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
		obj.transform.localScale = new Vector3(.1f, .1f, .1f);
		obj.transform.position = startPosition;
		obj.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(
			Random.Range(-5f, 5f),
			Random.Range(-80f, 80f),
			Random.Range(-5f, 5f)
		);
		obj.GetComponent<MeshRenderer>().sharedMaterial = material;
		gibs.Add(new Gib(15, obj));
	}

	public static void Update() {
		List<Gib> toRemove = new List<Gib>();

		foreach (Gib gib in gibs) {
			gib.Update();
			if (gib.ticks <= 0) {
				toRemove.Add(gib);
			}
		}

		foreach (Gib gib in toRemove) {
			Destroy(gib);
		}
		toRemove.Clear();
	}

	private static void Destroy(Gib gib) {
		gibs.Remove(gib);
		GameObject.Destroy(gib.obj);
	}
}
