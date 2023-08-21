using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
        Debug.Log("number of platforms:" + chunkDataSOs[levelId].numberOfPlatforms);

        if (levelIdOrder.Count < 1)
        {
            nextStartPos = originalAnchorPos.position;
        }

        for (int i = 0; i < chunkDataSOs[levelId].numberOfPlatforms;)
        {
            GameObject spawnedObject;
            Debug.Log("trying to spawn");

            if (chunkDataSOs[levelId].isPassThrough[i])
            {
                Debug.Log("is pass through");
                spawnedObject = GetPooledPassTrough();
                if (spawnedObject == null) return;
                spawnedObject.transform.localPosition = nextStartPos + chunkDataSOs[levelId].position[i];
                spawnedObject.transform.localScale = chunkDataSOs[levelId].scale[i];
                spawnedObject.transform.rotation = chunkDataSOs[levelId].rotation[i];
                spawnedObject.SetActive(true);
            }
            else
            {
                Debug.Log("solid");
                spawnedObject = GetPooledSolid();
                if (spawnedObject == null) return;
                spawnedObject.transform.localPosition = nextStartPos + chunkDataSOs[levelId].position[i];
                spawnedObject.transform.localScale = chunkDataSOs[levelId].scale[i];
                spawnedObject.transform.rotation = chunkDataSOs[levelId].rotation[i];
                spawnedObject.SetActive(true);
            }
            i++;
        }

        levelIdOrder.Add(levelId);
 
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
