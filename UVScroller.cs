using UnityEngine;
using System.Collections;

public class UVScroller : MonoBehaviour 
{
	public float XRate;
	public float YRate;

	public string textureToOffset = "_MainTex";
	private Vector2 uvPos;
	private Material scrollMat;
	private GameManager gameM;
	public bool timeScaled = true;
	public int materialIndex = -1;

	public bool sharedMaterialScroll = false;
	
	// Use this for initialization
	void Start () 
	{
		if(sharedMaterialScroll == false)
		{
			scrollMat = gameObject.GetComponent<Renderer>().material;
			if(materialIndex != -1)
			{
				scrollMat = gameObject.GetComponent<Renderer>().materials[materialIndex];
			}
		}
		else
		{
			scrollMat = gameObject.GetComponent<Renderer>().sharedMaterial;
			if(materialIndex != -1)
			{
				scrollMat = gameObject.GetComponent<Renderer>().sharedMaterials[materialIndex];
			}
		}

		uvPos = new Vector2(0.0f, 0.0f); //scrollMat.GetTextureOffset(textureToOffset);

		if(GameObject.Find("_GAME_MANAGER") != null)
		{
			gameM = GameObject.Find("_GAME_MANAGER").GetComponent<GameManager>();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(timeScaled == true)
		{
			uvPos.x += (Time.deltaTime * XRate) % 1.0f;
			uvPos.y += (Time.deltaTime * YRate) % 1.0f;
			scrollMat.SetTextureOffset(textureToOffset, uvPos);
		}
		else
		{
			if(gameM != null)
			{
				//uvPos.x += (gameM.deltaTimeUnscaled * XRate) % 1.0f;
				//uvPos.y += (gameM.deltaTimeUnscaled * YRate) % 1.0f;
				scrollMat.SetTextureOffset(textureToOffset, uvPos);
			}
			else
			{
				uvPos.x += (Time.deltaTime * XRate) % 1.0f;
				uvPos.y += (Time.deltaTime * YRate) % 1.0f;
				scrollMat.SetTextureOffset(textureToOffset, uvPos);
			}
		}
	}
}
