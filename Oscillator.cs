using UnityEngine;
using System.Collections;

public class Oscillator : MonoBehaviour 
{
	public Vector3 positionOscillate = Vector3.zero;
	public Vector3 rotationOscillate = Vector3.zero;
	public Vector3 scaleOscillate = Vector3.zero;

	private bool positionEnabled = false;
	private bool rotationEnabled = false;
	private bool scaleEnabled = false;

	private Vector3 originalPosition;
	private Quaternion originalRotation;
	private Vector3 scaleOriginal;
	public float speed = 1.0f;
	private float offset = 0.0f;
	public bool randomOffset = true;
	public Vector3 genericVector = Vector3.zero;
	public Quaternion genericQuat = Quaternion.identity;
	public AnimationCurve oscillateCurve;
	
	// Use this for initialization
	void Start () 
	{
		originalPosition = transform.localPosition;
		originalRotation = transform.localRotation;
		scaleOriginal = transform.localScale;
		if(randomOffset == true)
		{
			offset = Random.Range(-100.0f, 100.0f);
		}

		if(positionOscillate != Vector3.zero)
		{
			positionEnabled = true;
		}

		if(rotationOscillate != Vector3.zero)
		{
			rotationEnabled = true;
		}

		if(scaleOscillate != Vector3.zero)
		{
			scaleEnabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(positionEnabled == true)
		{
			genericVector = originalPosition + (positionOscillate * Mathf.Sin(((offset + Time.realtimeSinceStartup) * speed)));
			transform.localPosition = genericVector;
		}

		if(rotationEnabled == true)
		{
			transform.localRotation = originalRotation;
			transform.Rotate(rotationOscillate * oscillateCurve.Evaluate(Mathf.Sin(((offset + Time.realtimeSinceStartup)) * speed)), Space.Self);
		}

		if(scaleEnabled == true)
		{
			genericVector = Vector3.Lerp(scaleOriginal, scaleOscillate, Mathf.Abs(Mathf.Sin(offset + Time.realtimeSinceStartup * speed)));
			transform.localScale = genericVector;
		}
	}
}
