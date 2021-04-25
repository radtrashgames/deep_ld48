using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour 
{
	private Vector3 vectorBuffer = Vector3.zero;
	public GameManager gameM;
	public PlayerController playerC;
	public bool typeStopAndReorient;
	public float stopDelay = 1.0f;
	private float stopTimer = 0.0f;
	public bool typeRandomizedRotations;
	public float speed = 1.0f;
	public Vector3 direction;
	public bool timeScaled = true;
	public float randomSpeedAdd = 0.0f;
	public bool playerProximity = false;
	public float playerProximitySpeedMod = 1.0f;
	public float playerProximityDistanceBegin = 200.0f;
	public float delta;
	public float playerProxMod;
	public float dist;

	public bool dampen = false;
	public float dampenAmount = 1.0f;
	public Quaternion originalRotation;

	// start some shit
	void Start () 
	{
		originalRotation = transform.localRotation;
		gameM = GameObject.Find("GameManager").GetComponent<GameManager>();
		playerC = gameM.playerController; //GameObject.Find("Player").GetComponent<PlayerController>();
		if(typeRandomizedRotations == true)
		{
			pickRandomDirection();
		}

		randomSpeedAdd = Random.Range(0.0f, randomSpeedAdd);
	}
	
	void pickRandomDirection()
	{
		vectorBuffer.x = Random.Range(-1.0f, 1.0f);
		vectorBuffer.y = Random.Range(-1.0f, 1.0f);
		vectorBuffer.z = Random.Range(-1.0f, 1.0f);
		direction = vectorBuffer;
	}


	// a little rotatin never hurt nobody, unless you count people who were cut in half with industrial fan blades
	void Update () 
	{
		if(gameM == null) { gameM = GameObject.Find("_GAME_MANAGER").GetComponent<GameManager>(); }
		if(timeScaled == true)
		{
			delta = Time.deltaTime; 
		}
		else
		{
			//delta = gameM.deltaTimeUnscaled;
		}

		/*if(playerProximity == true)
		{
			dist = Vector3.Distance(gameM.playerController.transform.position, transform.position);
			if(dist < playerProximityDistanceBegin)
			{
				playerProxMod = playerProximitySpeedMod * ((playerProximityDistanceBegin - dist) / playerProximityDistanceBegin);
			}
		}*/

		transform.Rotate(direction*(speed+randomSpeedAdd)*(delta*60.0f), Space.Self);

		if(dampen == true)
		{
			transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, dampenAmount * Time.deltaTime);
		}

		if(typeStopAndReorient == true)
		{
			stopTimer += delta;
			if(stopTimer > stopDelay)
			{
				pickRandomDirection();
				stopTimer = 0.0f;	
			}
		}
	}
}
