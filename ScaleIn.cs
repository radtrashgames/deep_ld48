using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleIn : MonoBehaviour
{
    public Vector3 originalScale = Vector3.one;
    public AnimationCurve scaleInCurve;
    public float scaleTimer = 0.0f;
    public float scaleDelay = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;        
    }

    // Update is called once per frame
    void Update()
    {
        if(scaleTimer < scaleDelay)
        {
            scaleTimer += Time.deltaTime;
            transform.localScale = originalScale * scaleInCurve.Evaluate(scaleTimer / scaleDelay);
        }
        else
        {
            transform.localScale = originalScale;
        }        
    }

    public void OnEnable()
    {
        scaleTimer = 0.0f;
    }
}
