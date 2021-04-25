using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour 
{
	private GameManager gameM;
	public Transform cam;
	public Vector3 offset = Vector3.zero;
	public bool matchCamera = false;
	public bool matchPlayer = false;
	public bool lerp = false;
	public float lerpSpeed = 3.0f;
	public float lerpRandom = 1.0f;
	private Vector3 genericVec = Vector3.zero;
	private Quaternion genericQuat = Quaternion.identity;

	// Use this for initialization
	void Start () 
	{
		lerpSpeed += Random.Range(0.0f, lerpRandom);
		gameM = GameObject.Find("_GAME_MANAGER").GetComponent<GameManager>();
		if(cam == null)
		{
			/*if(GameObject.Find("MainCamera") != null)
			{
				cam = GameObject.Find("MainCamera").transform;
			}*/

			if(matchCamera == true)
			{
				cam = Camera.main.transform; //gameM.vrEye;
			}

			if(matchPlayer == true)
			{
				cam = GameObject.Find("Player").transform;
			}
		}
	}
	
	// Update is called once per frame 
	void LateUpdate () 
	{
		if(cam == null) 
		{ 
			cam = Camera.main.transform;

			return;
			/*if(GameObject.Find("MainCamera") != null)
			{
				cam = GameObject.Find("MainCamera").transform; 
			}
			else
			{
				return;
			}*/
		}

		if(matchCamera == true)
		{
			if(lerp == true)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, cam.rotation, Time.deltaTime * lerpSpeed);
				transform.Rotate(offset);
			}
			else
			{
				transform.rotation = cam.rotation;
				transform.Rotate(offset);
			}
		}
		else if(matchPlayer == true)
		{
			if(lerp == true)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, gameM.playerController.transform.rotation, Time.deltaTime * lerpSpeed);
				transform.Rotate(offset);
			}
			else
			{
				transform.rotation = gameM.playerController.transform.rotation;
				transform.Rotate(offset);
			}
		}
		else
		{
			if(lerp == true)
			{
				if(cam.position != transform.position)
				{
					genericQuat = transform.rotation;	
					transform.LookAt(cam);
					transform.Rotate(90.0f, 0.0f, 0.0f);
					transform.Rotate(offset);
					transform.rotation = Quaternion.Lerp(genericQuat, transform.rotation, Time.deltaTime * lerpSpeed);
				}
			}
			else
			{
				if(cam.position != transform.position)
				{
					transform.LookAt(cam);
				}
				transform.Rotate(90.0f, 0.0f, 0.0f);
				transform.Rotate(offset);
			}
		}
	}
}
