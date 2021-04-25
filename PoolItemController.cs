using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class PoolItemController : MonoBehaviour
{
    public Vector3 randomRotation = Vector3.zero;
    public float randomScaleAdd = 0.0f;
    public Vector3 originalScale = Vector3.one;

    public void OnEnable()
    {
        init();
    }

    public void Awake()
    {
        init();
    }

    public void init()
    { 
        transform.rotation = Quaternion.identity;
        Vector3 rand = new Vector3(Random.Range(randomRotation.x, -randomRotation.x),
            Random.Range(randomRotation.y, -randomRotation.y),
            Random.Range(randomRotation.z, -randomRotation.z));
        transform.Rotate(rand);
        transform.localScale = originalScale + (originalScale * Random.Range(0.0f, randomScaleAdd));
    }

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Despawner")
        {
            LeanPool.Despawn(gameObject);
        }
    }
}
