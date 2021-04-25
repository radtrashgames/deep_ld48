using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class PlayerController : MonoBehaviour
{
    public bool debug = true;
    public GameManager gameManager;
    public Vector3 originalPosition = Vector3.zero;
    public float moveSpeed = 1.0f;
    public Vector2 moveVelocity = Vector2.zero;
    public float moveVelocityMax = 1.0f;
    public float moveVelocityAddSpeed = 0.1f;
    public float inputDeadZone = 0.1f;
    public float moveDampenSpeed = 0.1f;
    public Vector3 moveBoundary = Vector3.zero;
    public Rigidbody rb;
    public float wallPercent = 0.9f;
    public float wallPushSpeed = 1.0f;

    public Vector3 lastTouchPosition = Vector3.zero;
    public float dragSpeed = 1.0f;

    public Transform healthBarRoot;
    public Vector3 originalHealthBarScale = Vector3.zero;
    public float airRemaining = 100.0f;
    public float airLossRate = 1.0f;
    public float airMax = 100.0f;
    public ParticleSystem airBubbleExplosionFX;
    public ParticleSystem dustPuffFX;
    public float displayHealth = 100.0f;
    public Material healthBarMaterial;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        originalHealthBarScale = healthBarRoot.localScale;
        healthBarMaterial = healthBarRoot.Find("HealthBarBG").GetComponent<Renderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.showingIntro)
        {
            return;
        }

        float padInput_x = Input.GetAxis("Horizontal");
        float padInput_y = Input.GetAxis("Vertical");
        Vector3 directForce = Vector3.zero;


        if (Input.GetMouseButtonDown(0))
        {
            lastTouchPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            moveVelocity.x = (Input.mousePosition.x - lastTouchPosition.x) * dragSpeed;
            moveVelocity.y = (Input.mousePosition.y - lastTouchPosition.y) * dragSpeed;
            lastTouchPosition = Input.mousePosition;
        }

        if (Mathf.Abs(padInput_x) > inputDeadZone)
        {
            moveVelocity.x += (padInput_x * Time.deltaTime * moveVelocityAddSpeed);
            directForce.x = (padInput_x * Time.deltaTime * moveVelocityAddSpeed);
        }
        if (Mathf.Abs(padInput_y) > inputDeadZone)
        {
            moveVelocity.y += (padInput_y * Time.deltaTime * moveVelocityAddSpeed);
            directForce.z = (padInput_y * Time.deltaTime * moveVelocityAddSpeed);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveVelocity.y += (Time.deltaTime * moveVelocityAddSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveVelocity.y -= (Time.deltaTime * moveVelocityAddSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveVelocity.x -= (Time.deltaTime * moveVelocityAddSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveVelocity.x += (Time.deltaTime * moveVelocityAddSpeed);
        }


        moveVelocity.x = Mathf.Clamp(moveVelocity.x, -1.0f, 1.0f);
        moveVelocity.y = Mathf.Clamp(moveVelocity.y, -1.0f, 1.0f);

        moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, moveDampenSpeed * Time.deltaTime);

        //transform.Translate(new Vector3(moveVelocity.x, 0.0f, moveVelocity.y) * Time.deltaTime * moveSpeed);
        rb.AddForce((new Vector3(moveVelocity.x, 0.0f, moveVelocity.y) * Time.deltaTime * moveSpeed), ForceMode.Impulse);


        // Health / Air bar update
        airRemaining -= airLossRate * Time.deltaTime;
        /*int N = 4;
        float M = 1.5f;
        float healthCalc = (float)(((float)airMax / (float)(100 - airRemaining))) * 100f;
        float N_root = (float)Mathf.Pow((healthCalc / 100f), (1f / M));
        float N_power = (float)Mathf.Pow((healthCalc / 100f), N);*/

        displayHealth = Mathf.Lerp(displayHealth, (airRemaining / airMax), Time.deltaTime * 10.0f);
        healthBarRoot.localScale = new Vector3(originalHealthBarScale.x * displayHealth, originalHealthBarScale.y, originalHealthBarScale.z);

        /*if (healthCalc < 50)
        {
            healthBarMaterial.SetColor("_Color", Color.Lerp(Color.red, Color.yellow, (float)N_root));
            healthBarMaterial.SetColor("_EmissionColor", Color.Lerp(Color.red, Color.yellow, (float)N_root) * Color.gray);
        }
        else if (healthCalc >= 50)
        {
            healthBarMaterial.SetColor("_Color", Color.Lerp(Color.yellow, Color.green, (float)N_power));
            healthBarMaterial.SetColor("_EmissionColor", Color.Lerp(Color.yellow, Color.green, (float)N_power) * Color.gray);
        }*/

        // Player Death
        if (airRemaining <= 0.0f)
        {
            gameManager.deathText.text = "You ran out of [AIR] after diving " + gameManager.displayDepth.ToString() + " Meters";
            Die();
        } else if(airRemaining <= (airMax*0.2f))
        {
            gameManager.warningText.gameObject.SetActive(true);
        }


    }

    public void Die()
    {
        gameManager.DespawnAll();
        transform.position = originalPosition;
        gameManager.playerDepth = 0.0f;
        airRemaining = 100.0f;
        gameManager.EndGame();
    }

    public void OnCollisionEnter(Collision collision)
    {
        dustPuffFX.transform.position = collision.GetContact(0).point;
        dustPuffFX.Emit(5);
        airRemaining -= (collision.relativeVelocity.magnitude * 0.1f);
        if (debug)
        {
            Debug.Log("HIT A COLLIDER, MAGNITUDE: "+ collision.relativeVelocity.magnitude.ToString());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Mine")
        {
            airRemaining = -1.0f;
            airBubbleExplosionFX.transform.position = other.transform.position;
            airBubbleExplosionFX.transform.Translate(new Vector3(0.0f, -3.0f, 0.0f), Space.World);
            airBubbleExplosionFX.Emit(300);

            gameManager.deathText.text = "You hit a mine after diving " + gameManager.displayDepth.ToString() + " Meters";
            Die();
        }

        if (other.tag == "AirBubble")
        {
            airRemaining = Mathf.Clamp(airRemaining + 25.0f, 0.0f, airMax);
            airBubbleExplosionFX.transform.position = other.transform.position;
            airBubbleExplosionFX.transform.Translate(new Vector3(0.0f, -3.0f, 0.0f), Space.World);
            airBubbleExplosionFX.Emit(300);
            LeanPool.Despawn(other.gameObject);

            if (debug)
            {
                Debug.Log("HIT AN AIRBUBBLE");
            }
        }
    }

    public void FixedUpdate()
    {
        // boundary check 
        if (transform.position.x > moveBoundary.x)
        {
            Vector3 resetPos = transform.position;
            resetPos.x = moveBoundary.x;
            transform.position = resetPos;
            Vector3 velocityReset = rb.velocity;
            velocityReset.x = 0.0f;
            rb.velocity = velocityReset;
        }
        if (transform.position.x < -moveBoundary.x)
        {
            Vector3 resetPos = transform.position;
            resetPos.x = -moveBoundary.x;
            transform.position = resetPos;
            Vector3 velocityReset = rb.velocity;
            velocityReset.x = 0.0f;
            rb.velocity = velocityReset;
        }
        if (transform.position.z > moveBoundary.z)
        {
            Vector3 resetPos = transform.position;
            resetPos.z = moveBoundary.z;
            transform.position = resetPos;
            Vector3 velocityReset = rb.velocity;
            velocityReset.z = 0.0f;
            rb.velocity = velocityReset;
        }
        if (transform.position.z < -moveBoundary.z)
        {
            Vector3 resetPos = transform.position;
            resetPos.z = -moveBoundary.z;
            transform.position = resetPos;
            Vector3 velocityReset = rb.velocity;
            velocityReset.z = 0.0f;
            rb.velocity = velocityReset;
        }

        // Push away from wall 
        if (transform.position.x > moveBoundary.x * wallPercent)
        {
            rb.AddForce(new Vector3(-wallPushSpeed * Time.deltaTime, 0.0f, 0.0f), ForceMode.Impulse);
        }
        if (transform.position.x < -moveBoundary.x * wallPercent)
        {
            rb.AddForce(new Vector3(wallPushSpeed * Time.deltaTime, 0.0f, 0.0f), ForceMode.Impulse);
        }
        if (transform.position.z > moveBoundary.z * wallPercent)
        {
            rb.AddForce(new Vector3(0.0f, 0.0f, -wallPushSpeed * Time.deltaTime), ForceMode.Impulse);
        }
        if (transform.position.z < -moveBoundary.z * wallPercent)
        {
            rb.AddForce(new Vector3(0.0f, 0.0f, wallPushSpeed * Time.deltaTime), ForceMode.Impulse);
        }


    }
}
