using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

/*
[CustomEditor(typeof(GenerateLevelChunkData))]
public class GenerateLevelChunkDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myScript = target as GenerateLevelChunkData;
        //myScript.GetLevelData();
    }
    
}
*/

public class GenerateLevelChunkData : MonoBehaviour
{
    [SerializeField] Transform bottomLeft;
    [SerializeField] Transform topOfLevel;
    private void Start()
    {
        GetLevelData();
    }

    [SerializeField] string levelName = "placeholder name";
    private bool haveDone = false;

    private void Update()
    {
        if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.N) && !haveDone)
        {
            haveDone = true;
            GetLevelData();
        }
    }
    

    public void GetLevelData()
    {
        //deletes the file if it already exists
        if(File.Exists("Assets/LevelData/" + levelName)) { File.Delete("Assets/LevelData/" + levelName); }

        Debug.Log("child count:" + transform.childCount) ;
        int numOfPlatforms = 0;
        LevelChunkDataSO levelData = ScriptableObject.CreateInstance<LevelChunkDataSO>();
        string path = "Assets/LevelData/" + levelName + ".asset";


        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("PassThroughPlatform") || transform.GetChild(i).CompareTag("SolidPlatform"))
            {
                numOfPlatforms++;
            }
        }

        levelData.bottomLeft = bottomLeft.position;
        levelData.numberOfPlatforms = numOfPlatforms;
        levelData.height = topOfLevel.localPosition.y;
        levelData.position = new Vector3[numOfPlatforms];
        levelData.scale = new Vector3[numOfPlatforms];
        levelData.color = new Color[numOfPlatforms];
        levelData.rotation = new Quaternion[numOfPlatforms];
        levelData.isPassThrough = new bool[numOfPlatforms];
        levelData.isQuestion = new bool[numOfPlatforms];

        for (int i = 0; i < transform.childCount; i++) 
        {
            if (transform.GetChild(i).GetComponentInChildren<QuestionPlatform>())
            {
                levelData.isQuestion[i - 1] = true;
            }

            if (transform.GetChild(i).CompareTag("PassThroughPlatform"))
            {
                levelData.isPassThrough[i -1] = true;
            }else if (transform.GetChild(i).CompareTag("SolidPlatform"))
            {
                levelData.isPassThrough[i -1] = false;
            }

            if(transform.GetChild(i).CompareTag("PassThroughPlatform") || transform.GetChild(i).CompareTag("SolidPlatform") || transform.GetChild(i).GetComponentInChildren<QuestionPlatform>())  //if its any type of platform
            {
                levelData.position[i - 1] = transform.GetChild(i).transform.localPosition;
                levelData.scale[i - 1] = transform.GetChild(i).transform.localScale;
                levelData.rotation[i - 1] = transform.GetChild(i).transform.rotation;
            }
        }

        levelData.numberOfPlatforms = numOfPlatforms;

        //AssetDatabase.CreateAsset(levelData, path);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }
}
