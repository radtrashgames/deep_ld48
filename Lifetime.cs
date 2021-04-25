using UnityEngine;
using System.Collections;

public class Lifetime : MonoBehaviour 
{
	public float lifetime = 1.0f;
	void Update () 
	{
		lifetime -= Time.deltaTime;
		if(lifetime < 0.0f)
		{
			Destroy(gameObject);
		}
	}
}
