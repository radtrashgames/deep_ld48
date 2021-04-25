using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class GameManager : MonoBehaviour
{
    public PlayerController playerController;
    public LeanGameObjectPool sharkObjectPool;
    public LeanGameObjectPool rockObjectPool;

    public AnimationCurve sharkSpawnCurve;
    public AnimationCurve rockSpawnCurve;

    public float playerDepth = 0.0f;
    public float displayDepth = 0.0f;
    public float playerDepthLevelSwitch = 100.0f;
    public TMPro.TextMeshPro depthMeterTMPro;
    public Vector3 playerOriginalPosition = Vector3.zero;
    public float playerSpawnOffset = -10.0f;

    public bool showingIntro = true;
    public Transform introText;
    public TMPro.TextMeshPro deathText;
    public Transform warningText;

    [System.Serializable]
    public class Level
    {
        public List<TunnelSpawn> tunnelSpawners; 
    }

    public List<Level> levels;
    public int currentLevel = 0;

    [System.Serializable]
    public class TunnelSpawn
    {
        public string name = "SpawnerName";
        public LeanGameObjectPool objectPool;
        public AnimationCurve spawnCurve;
        public float randomY = 180.0f;
        public bool onUnitSphere = false;
        public float unitSphereSize = 1.0f;
        public Vector3 randomOffset = Vector3.one;
        public float spawnTimer = 0.0f;
        public float spawnDelay = 1.0f;
        public float spawnRandom = 0.2f;
        public float depthBegin = 0.0f;
        public float depthEnd = 4000.0f;
    }

    public void StartGame()
    {
        showingIntro = false;
        introText.gameObject.SetActive(false);
        playerController.rb.useGravity = true;
        deathText.gameObject.SetActive(false);
        deathText.gameObject.SetActive(false);
    }

    public void EndGame()
    {
        showingIntro = true;
        introText.gameObject.SetActive(true);
        deathText.gameObject.SetActive(true);
        warningText.gameObject.SetActive(false);
        playerController.rb.useGravity = false;
        playerController.rb.velocity = Vector3.zero;
    }



    // Start is called before the first frame update
    void Start()
    {
        playerOriginalPosition = playerController.transform.position;
        EndGame();
        deathText.gameObject.SetActive(false);
    }

    public void DespawnAll()
    {
        for (int i = 0; i < levels[currentLevel].tunnelSpawners.Count; i++)
        {
            levels[currentLevel].tunnelSpawners[i].objectPool.DespawnAll();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(showingIntro)
        {
            if(Input.anyKey)
            {
                StartGame();
            }
        }

        float lastDepth = playerDepth;
        playerDepth = (playerController.transform.position - playerOriginalPosition).y;
        displayDepth = Mathf.RoundToInt(Mathf.Abs(playerDepth * 3));
        float depthDelta = lastDepth - playerDepth;
        float playerDepthLerp = playerDepth / playerDepthLevelSwitch;
        
        for(int i = 0; i < levels[currentLevel].tunnelSpawners.Count; i++)
        {

            if (Mathf.Abs(playerDepth) < levels[currentLevel].tunnelSpawners[i].depthBegin || Mathf.Abs(playerDepth) > levels[currentLevel].tunnelSpawners[i].depthEnd)
            {
                continue;
            }

            levels[currentLevel].tunnelSpawners[i].spawnTimer += depthDelta * 0.1f; // Time.deltaTime;

            if(levels[currentLevel].tunnelSpawners[i].spawnTimer > levels[currentLevel].tunnelSpawners[i].spawnDelay)
            {

                float chanceCurve = levels[currentLevel].tunnelSpawners[i].spawnCurve.Evaluate(playerDepthLerp);
                if(Random.Range(0.0f, 1.0f) < chanceCurve)
                {
                    Vector3 randomPos = new Vector3(
                        Random.Range(levels[currentLevel].tunnelSpawners[i].randomOffset.x, -levels[currentLevel].tunnelSpawners[i].randomOffset.x),
                        Random.Range(levels[currentLevel].tunnelSpawners[i].randomOffset.y, -levels[currentLevel].tunnelSpawners[i].randomOffset.y),
                        Random.Range(levels[currentLevel].tunnelSpawners[i].randomOffset.z, -levels[currentLevel].tunnelSpawners[i].randomOffset.z));
                    randomPos.y += (playerDepth + playerSpawnOffset);
                    levels[currentLevel].tunnelSpawners[i].objectPool.Spawn(randomPos);
                }
                levels[currentLevel].tunnelSpawners[i].spawnTimer = 0.0f;
            }
        }

        depthMeterTMPro.text = "[AIR]:\n[DEPTH]: " + Mathf.RoundToInt(Mathf.Abs(displayDepth)).ToString() + " Meters";
    }
}
