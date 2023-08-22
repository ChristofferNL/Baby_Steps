using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Pool;

public class LevelGenerator : NetworkBehaviour
{
    [SerializeField] LevelChunkDataSO[] chunkDataSOs;

    [Header("Solid Platforms")]
    public List<GameObject> pooledPlatforms;
    public GameObject platformsToPool;
    public int platformAmount;

    [Header("Pass Through Platforms")]
    public List<GameObject> pooledPassthrough;
    public GameObject passThroughToPool;
    public int passThroughAmount;

    [SerializeField] float currentHeight;
    [SerializeField] float lastHeight;
    [SerializeField] float heightNextSpawn;

    private List<GameObject> spawnedObjects;

    public List<int> levelIdOrder = new List<int>();
    public List<int> levelSpawnOrder = new List<int>();
    public List<GameObject> spawnedChunk1 = new List<GameObject>();
    public List<GameObject> spawnedChunk2 = new List<GameObject>();
    public List<GameObject> spawnedChunk3 = new List<GameObject>();
    [SerializeField] GameObject levelChunkHolder;
    public int amountOfSpawnedLevels;

    public Vector3 nextStartPos;
    public Vector3 oldStartPos;

    public bool testingMode = false;

    public int currentLevelId;
    public float playerHeight;
    [SerializeField] Transform player1;
    [SerializeField] Transform player2;
    [SerializeField] Transform originalAnchorPos;


    private void Start()
    {
        if (testingMode)
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

            //creating 
            pooledPassthrough = new List<GameObject>(platformAmount);
            GameObject tmp2;
            for (int i = 0; i < passThroughAmount; i++)
            {
                tmp2 = Instantiate(passThroughToPool);
                tmp2.SetActive(false);
                pooledPassthrough.Add(tmp2);
                //tmp2.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if(!IsClient && !testingMode) { return; }
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

        //creating 
        pooledPassthrough = new List<GameObject>();
        GameObject tmp2;
        for (int i = 0; i < passThroughAmount; i++)
        {
            tmp2 = Instantiate(passThroughToPool);
            tmp2.SetActive(false);
            pooledPassthrough.Add(tmp2);
            tmp2.GetComponent<NetworkObject>().Spawn();
        }
    }


    private void FixedUpdate()
    {
        
        playerHeight = ((player1.position + player2.position) / 2).y;

        if(playerHeight > heightNextSpawn)
        {
            SpawnChunk();
        }
    }

    void SpawnChunk()
    {
        int levelId = Random.Range(0, chunkDataSOs.Length);
        //Debug.Log("number of platforms:" + chunkDataSOs[levelId].numberOfPlatforms);

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

            //Debug.Log("lowest height: " + lowestHeight);

            if(lowestHeight ==  heightChunk1) 
            {
                for (int i = 0; i < spawnedChunk1.Count; i++)
                {
                    spawnedChunk1[i].SetActive(false);
                }
                spawnedChunk1.Clear();
                spawnedChunk1 = new List<GameObject>();
            }
            else if(lowestHeight == heightChunk2)
            {
                for (int i = 0; i < spawnedChunk2.Count; i++)
                {
                    spawnedChunk2[i].SetActive(false);
                }
                spawnedChunk2.Clear();
                spawnedChunk2 = new List<GameObject>();
            }
            else if (lowestHeight == heightChunk3)
            {
                for (int i = 0; i < spawnedChunk3.Count; i++)
                {
                    spawnedChunk3[i].SetActive(false);
                }
                spawnedChunk3.Clear();
                spawnedChunk3 = new List<GameObject>();
            }    
        }

        int chunkToSaveTo = 0;
        if(spawnedChunk1.Count == 0) { chunkToSaveTo = 1; }
        else if(spawnedChunk2.Count == 0) { chunkToSaveTo = 2; }
        else if(spawnedChunk3.Count == 0) { chunkToSaveTo = 3; }

        Debug.Log("chunktosaveto: " + chunkToSaveTo);

        //GameObject spawnedHolderParent = Instantiate(levelChunkHolder, transform.position, Quaternion.identity);

        for (int i = 0; i < chunkDataSOs[levelId].numberOfPlatforms;)
        {
            GameObject spawnedObject;

            if (chunkDataSOs[levelId].isPassThrough[i])
            {
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

                /*
                spawnedObject.GetComponent<NetworkTransform>().enabled = false;
                spawnedObject.GetComponent<NetworkObject>().des = false;
                spawnedObject.transform.parent = spawnedHolderParent.transform;
                spawnedObject.GetComponent<NetworkObject>().enabled = true;
                spawnedObject.GetComponent<NetworkTransform>().enabled = true;
                */
                spawnedObject.SetActive(true);
            }
            else
            {
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

                /*
                spawnedObject.GetComponent<NetworkObject>().enabled = false;
                spawnedObject.transform.parent = spawnedHolderParent.transform;
                spawnedObject.GetComponent<NetworkObject>().enabled = true;
                */
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
}
