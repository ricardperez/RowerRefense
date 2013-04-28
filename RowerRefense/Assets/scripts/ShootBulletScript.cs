using UnityEngine;
using System.Collections;

public class ShootBulletScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnParticleCollision(GameObject other) {
        Debug.Log ("Particle collision");
	}
}
