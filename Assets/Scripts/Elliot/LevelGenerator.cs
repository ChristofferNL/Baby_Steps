using LobbyRelaySample;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Pool;

public class LevelGenerator : NetworkBehaviour
{
    [SerializeField] LevelChunkDataSO[] chunkDataSOs;
    [SerializeField] bool createPoolsOnRuntime = false;
    [Header("Solid Platforms")]
    public List<GameObject> pooledPlatforms;
    public GameObject platformsToPool;
    public int platformAmount;

    [Header("Pass Through Platforms")]
    public List<GameObject> pooledPassthrough;
    public GameObject passThroughToPool;
    public int passThroughAmount;

    [Header("Question Platforms")]
    public List<GameObject> pooledQuestions;
    public GameObject questionToPool;
    public int questionAmount;

    [Header("Bouncy Platforms")]
    public List<GameObject> pooledBouncy;
    public GameObject bouncyToPool;
    public int bouncyAmount;

    [Header("Question Flags")]
    public List<GameObject> pooledFlags;
    public GameObject flagsToPool;
    public int flagsAmount;

    [SerializeField] float heightNextSpawn;

    private List<GameObject> spawnedObjects;

    public List<int> levelIdOrder = new List<int>();
    public List<int> levelSpawnOrder = new List<int>();
    public List<GameObject> spawnedChunk1 = new List<GameObject>();
    public List<GameObject> spawnedChunk2 = new List<GameObject>();
    public List<GameObject> spawnedChunk3 = new List<GameObject>();
    public List<GameObject> spawnedChunk4 = new List<GameObject>();
    public int amountOfSpawnedLevels;

    public Vector3 nextStartPos;
    public Vector3 oldStartPos;

    public bool testingMode = false;
    private int questionOverrideId;
    private bool canSpawn = false;

    public int currentLevelId;
    public float playerHeight;
    [SerializeField] Transform player1;
    [SerializeField] Transform player2;
    [SerializeField] Transform originalAnchorPos;
    [SerializeField] QuestionManager questionManager;
    [SerializeField] GameObject questionFlag;

    private void Start()
    {
        if (testingMode && createPoolsOnRuntime)
        {
            //creating solid pool
            pooledPlatforms = new List<GameObject>(platformAmount);
            GameObject tmp;
            for (int i = 0; i < platformAmount; i++)
            {
                tmp = Instantiate(platformsToPool);
                tmp.SetActive(false);
                pooledPlatforms.Add(tmp);
                //tmp.GetComponent<NetworkObject>().Spawn();
            }

            //creating passthrough
            pooledPassthrough = new List<GameObject>(platformAmount);
            GameObject tmp2;
            for (int i = 0; i < passThroughAmount; i++)
            {
                tmp2 = Instantiate(passThroughToPool);
                tmp2.SetActive(false);
                pooledPassthrough.Add(tmp2);
                //tmp2.GetComponent<NetworkObject>().Spawn();
            }

            //creating questions
            pooledQuestions = new List<GameObject>(questionAmount);
            GameObject tmp3;
            for (int i = 0; i < questionAmount; i++)
            {
                tmp3 = Instantiate(questionToPool);
                tmp3.SetActive(false);
                pooledQuestions.Add(tmp3);
                //tmp2.GetComponent<NetworkObject>().Spawn();
            }

            //creating bouncy
            pooledBouncy = new List<GameObject>(bouncyAmount);
            GameObject tmp4;
            for (int i = 0; i < bouncyAmount; i++)
            {
                tmp4 = Instantiate(bouncyToPool);
                tmp4.SetActive(false);
                pooledBouncy.Add(tmp4);
                //tmp2.GetComponent<NetworkObject>().Spawn();
            }

            //creating bouncy
            pooledBouncy = new List<GameObject>(flagsAmount);
            GameObject tmp5;
            for (int i = 0; i < flagsAmount; i++)
            {
                tmp5 = Instantiate(flagsToPool);
                tmp5.SetActive(false);
                pooledFlags.Add(tmp5);
                //tmp2.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        StartCoroutine(WaitToStartSpawning(4));
        if (pooledPlatforms.Count != 0) foreach (GameObject obj in pooledPlatforms) { obj.SetActive(false); };
        if (pooledPassthrough.Count != 0) foreach (GameObject obj in pooledPassthrough) { obj.SetActive(false); };
        if (pooledQuestions.Count != 0) foreach (GameObject obj in pooledQuestions) { obj.SetActive(false); };
        if (pooledBouncy.Count != 0) foreach (GameObject obj in pooledQuestions) { obj.SetActive(false); };

        if(!testingMode && !createPoolsOnRuntime) { return; }
        //creating solid pool
        pooledPlatforms = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < platformAmount; i++)
        {
            tmp = Instantiate(platformsToPool);
            tmp.SetActive(false);
            pooledPlatforms.Add(tmp);
            tmp.GetComponent<NetworkObject>().Spawn();
        }

        //creating passthrough
        pooledPassthrough = new List<GameObject>();
        GameObject tmp2;
        for (int i = 0; i < passThroughAmount; i++)
        {
            tmp2 = Instantiate(passThroughToPool);
            tmp2.SetActive(false);
            pooledPassthrough.Add(tmp2);
            tmp2.GetComponent<NetworkObject>().Spawn();
        }

        //creating questions
        pooledQuestions = new List<GameObject>(questionAmount);
        GameObject tmp3;
        for (int i = 0; i < questionAmount; i++)
        {
            tmp3 = Instantiate(questionToPool);
            tmp3.SetActive(false);
            pooledQuestions.Add(tmp3);
            tmp3.GetComponent<NetworkObject>().Spawn();
        }

        //creating bouncy
        pooledBouncy = new List<GameObject>(bouncyAmount);
        GameObject tmp4;
        for (int i = 0; i < bouncyAmount; i++)
        {
            tmp4 = Instantiate(bouncyToPool);
            tmp4.SetActive(false);
            pooledBouncy.Add(tmp4);
            tmp4.GetComponent<NetworkObject>().Spawn();
        }

        //creating bouncy
        pooledBouncy = new List<GameObject>(flagsAmount);
        GameObject tmp5;
        for (int i = 0; i < flagsAmount; i++)
        {
            tmp5 = Instantiate(flagsToPool);
            tmp5.SetActive(false);
            pooledFlags.Add(tmp5);
            tmp5.GetComponent<NetworkObject>().Spawn();
        }
    }

    IEnumerator WaitToStartSpawning(float waitTime)
    {
        while(NetworkManager.Singleton.ConnectedClients.Count < GameManager.Instance.LocalLobby.PlayerCount)
        {
            Debug.LogError("CHECKING");
            yield return 0;
        }
        yield return new WaitForSecondsRealtime(waitTime);
        canSpawn = true;
    }


    private void FixedUpdate()
    {
        playerHeight = ((player1.position + player2.position) / 2).y;

        if(playerHeight > heightNextSpawn && canSpawn)
        {
            if(IsHost || IsServer) 
            {
                //Random.Range(0, chunkDataSOs.Length)
                SpawnChunkClientRpc(Random.Range(0, chunkDataSOs.Length));  
            }
        }
    }

    [ClientRpc]
    void SpawnChunkClientRpc(int levelId, ClientRpcParams clientRpcParams = default)
    {
        if (levelIdOrder.Count < 1)
        {
            nextStartPos = originalAnchorPos.position;
        }

        //destroys the lowest level
        if(spawnedChunk1.Count != 0 && spawnedChunk2.Count != 0 && spawnedChunk3.Count != 0)
        {

            //gets the height of the chunk 1
            float sum1 = 0;
            for (int i = 0; i < spawnedChunk1.Count; i++)
            {
                sum1 += spawnedChunk1[i].transform.position.y;
            }
            float heightChunk1 = sum1 / spawnedChunk1.Count;

            //gets the height of the chunk 2
            float sum2 = 0;
            for (int i = 0; i < spawnedChunk2.Count; i++)
            {
                sum2 += spawnedChunk2[i].transform.position.y;
            }
            float heightChunk2 = sum2 / spawnedChunk2.Count;

            //gets the height of the chunk 3
            float sum3 = 0;
            for (int i = 0; i < spawnedChunk3.Count; i++)
            {
                sum3 += spawnedChunk3[i].transform.position.y;
            }
            float heightChunk3 = sum3 / spawnedChunk3.Count;

            float lowestHeight = heightChunk1;
            if(lowestHeight > heightChunk2) { lowestHeight = heightChunk2; }
            if(lowestHeight > heightChunk3) { lowestHeight = heightChunk3; }

            if(lowestHeight ==  heightChunk1) 
            {
                for (int i = 0; i < spawnedChunk1.Count; i++)
                {
                    if (spawnedChunk1[i].transform.childCount > 0)
                    {
                        spawnedChunk1[i].GetComponentInChildren<QuestionPlatform>().hasSpawnedQuestion = false;
                    }
                    spawnedChunk1[i].SetActive(false);
                }
                spawnedChunk1.Clear();
                //spawnedChunk1 = new List<GameObject>(0);
            }
            else if(lowestHeight == heightChunk2)
            {
                for (int i = 0; i < spawnedChunk2.Count; i++)
                {
                    if (spawnedChunk2[i].transform.childCount > 0)
                    {
                        spawnedChunk2[i].GetComponentInChildren<QuestionPlatform>().hasSpawnedQuestion = false;
                    }
                    spawnedChunk2[i].SetActive(false);
                }
                spawnedChunk2.Clear();
                //spawnedChunk2 = new List<GameObject>(0);
            }
            else if (lowestHeight == heightChunk3)
            {
                for (int i = 0; i < spawnedChunk3.Count; i++)
                {
                    if (spawnedChunk3[i].transform.childCount > 0)
                    {
                        spawnedChunk3[i].GetComponentInChildren<QuestionPlatform>().hasSpawnedQuestion = false;
                    }
                    spawnedChunk3[i].SetActive(false);
                }
                spawnedChunk3.Clear();
                //spawnedChunk3 = new List<GameObject>(0);
            }    
        }

        int chunkToSaveTo = 0;
        if(spawnedChunk1.Count == 0) { chunkToSaveTo = 1; }
        else if(spawnedChunk2.Count == 0) { chunkToSaveTo = 2; }
        else if(spawnedChunk3.Count == 0) { chunkToSaveTo = 3; }


        for (int i = 0; i < chunkDataSOs[levelId].numberOfPlatforms;)
        {
            GameObject spawnedObject;

            if (chunkDataSOs[levelId].isBouncy[i])
            {
                //spawns a bouncy platform
                spawnedObject = GetPooledBouncy();
                if (spawnedObject == null) return;
                spawnedObject.transform.localPosition = nextStartPos + chunkDataSOs[levelId].position[i];
                spawnedObject.transform.localScale = chunkDataSOs[levelId].scale[i];
                spawnedObject.transform.rotation = chunkDataSOs[levelId].rotation[i];

                if (chunkToSaveTo == 1)
                {
                    spawnedChunk1.Add(spawnedObject);
                }
                else if (chunkToSaveTo == 2)
                {
                    spawnedChunk2.Add(spawnedObject);
                }
                else if (chunkToSaveTo == 3)
                {
                    spawnedChunk3.Add(spawnedObject);
                }
                spawnedObject.SetActive(true);
            }

            if (chunkDataSOs[levelId].isQuestion[i]) 
            {
                //spawns a question platform
                spawnedObject = GetPooledQuestion();
                if (spawnedObject == null) return;
                spawnedObject.transform.localPosition = nextStartPos + chunkDataSOs[levelId].position[i];
                spawnedObject.transform.localScale = chunkDataSOs[levelId].scale[i];
                spawnedObject.transform.rotation = chunkDataSOs[levelId].rotation[i];

                GameObject spawnedFlag;
                spawnedFlag = GetPooledFlag();
                spawnedFlag.transform.localRotation = Quaternion.Euler(0, 0, 0);
                spawnedFlag.transform.localPosition = spawnedObject.transform.localPosition + Vector3.up * spawnedObject.transform.localScale.y * 1.75f;

                if (chunkToSaveTo == 1)
                {
                    spawnedChunk1.Add(spawnedObject);
                    spawnedChunk1.Add(spawnedFlag); 
                }
                else if (chunkToSaveTo == 2)
                {
                    spawnedChunk2.Add(spawnedObject);
                    spawnedChunk2.Add(spawnedFlag);
                }
                else if (chunkToSaveTo == 3)
                {
                    spawnedChunk3.Add(spawnedObject);
                    spawnedChunk3.Add(spawnedFlag);
                }
                spawnedObject.SetActive(true);
                spawnedFlag.SetActive(true);
                

                QuestionPlatform questionPlatformScript = spawnedObject.GetComponentInChildren<QuestionPlatform>();
                questionPlatformScript.questionManager = questionManager;
                questionPlatformScript.flagTransform = spawnedFlag.transform;
                questionPlatformScript.flagPivotPoint = spawnedFlag.transform.position + Vector3.down * spawnedFlag.transform.localScale.magnitude * 1.04f + Vector3.right * 0.9f;
            }

            if (chunkDataSOs[levelId].isPassThrough[i] && !chunkDataSOs[levelId].isQuestion[i] && !chunkDataSOs[levelId].isBouncy[i])
            {
                //spawns a pass through platform
                spawnedObject = GetPooledPassTrough();
                if (spawnedObject == null) return;
                spawnedObject.transform.localPosition = nextStartPos + chunkDataSOs[levelId].position[i];
                spawnedObject.transform.localScale = chunkDataSOs[levelId].scale[i];
                spawnedObject.transform.rotation = chunkDataSOs[levelId].rotation[i];

                if(chunkToSaveTo == 1)
                {
                    spawnedChunk1.Add(spawnedObject);
                }
                else if(chunkToSaveTo == 2)
                {
                    spawnedChunk2.Add(spawnedObject);
                } 
                else if(chunkToSaveTo == 3)
                {
                    spawnedChunk3.Add(spawnedObject);
                }
                spawnedObject.SetActive(true);
            }
            else if(!chunkDataSOs[levelId].isQuestion[i] && !chunkDataSOs[levelId].isBouncy[i])
            {
                //spawns a solid platform
                spawnedObject = GetPooledSolid();
                if (spawnedObject == null) return;
                spawnedObject.transform.localPosition = nextStartPos + chunkDataSOs[levelId].position[i];
                spawnedObject.transform.localScale = chunkDataSOs[levelId].scale[i];
                spawnedObject.transform.rotation = chunkDataSOs[levelId].rotation[i];

                if (chunkToSaveTo == 1)
                {
                    spawnedChunk1.Add(spawnedObject);
                }
                else if (chunkToSaveTo == 2)
                {
                    spawnedChunk2.Add(spawnedObject);
                }
                else if (chunkToSaveTo == 3)
                {
                    spawnedChunk3.Add(spawnedObject);
                }
                spawnedObject.SetActive(true);
            }
            i++;
        }

        levelSpawnOrder.Add(amountOfSpawnedLevels);
        levelIdOrder.Add(levelId);
        amountOfSpawnedLevels++;

        oldStartPos = nextStartPos;
        nextStartPos = oldStartPos + Vector3.up * chunkDataSOs[levelId].height;
        heightNextSpawn = oldStartPos.y;
    }

    //gets a pooled passhtrough
    public GameObject GetPooledPassTrough()
    {
        for (int i = 0; i < passThroughAmount; i++)
        {
            if (!pooledPassthrough[i].activeInHierarchy)
            {
                return pooledPassthrough[i];
            }
        }
        return null;
    }

    //gets a pooled solid 
    public GameObject GetPooledSolid()
    {
        for (int i = 0; i < platformAmount; i++)
        {
            if (!pooledPlatforms[i].activeInHierarchy)
            {
                return pooledPlatforms[i];
            }
        }
        return null;
    }

    //gets a pooled question 
    public GameObject GetPooledQuestion()
    {
        for (int i = 0; i < questionAmount; i++)
        {
            if (!pooledQuestions[i].activeInHierarchy)
            {
                return pooledQuestions[i];
            }
        }
        return null;
    }

    //gets a pooled Bouncy Platform 
    public GameObject GetPooledBouncy()
    {
        for (int i = 0; i < bouncyAmount; i++)
        {
            if (!pooledBouncy[i].activeInHierarchy)
            {
                return pooledBouncy[i];
            }
        }
        return null;
    }

    //gets a pooled flags 
    public GameObject GetPooledFlag()
    {
        for (int i = 0; i < flagsAmount; i++)
        {
            if (!pooledFlags[i].activeInHierarchy)
            {
                return pooledFlags[i];
            }
        }
        return null;
    }
}