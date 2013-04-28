using UnityEngine;
using System.Collections;

public class DestroyParticleSystemScript : MonoBehaviour
{
	
	
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!this.particleSystem.IsAlive ()) {
			Destroy (this.gameObject);
		}
	
	}
}
