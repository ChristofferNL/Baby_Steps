using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;
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

        levelData.bottomLeft = transform.position;
        levelData.numberOfPlatforms = numOfPlatforms;
        levelData.position = new Vector3[numOfPlatforms];
        levelData.scale = new Vector3[numOfPlatforms];
        levelData.rotation = new Quaternion[numOfPlatforms];
        levelData.isPassThrough = new bool[numOfPlatforms];

        for (int i = 0; i < transform.childCount; i++) 
        {
            if (transform.GetChild(i).CompareTag("PassThroughPlatform"))
            {
                levelData.isPassThrough[i -1] = true;
            }else if (transform.GetChild(i).CompareTag("SolidPlatform"))
            {
                levelData.isPassThrough[i -1] = false;
            }

            if(transform.GetChild(i).CompareTag("PassThroughPlatform") || transform.GetChild(i).CompareTag("SolidPlatform"))
            {
                levelData.position[i - 1] = transform.GetChild(i).transform.localPosition;
                levelData.scale[i - 1] = transform.GetChild(i).transform.localScale;
                levelData.rotation[i - 1] = transform.GetChild(i).transform.rotation;
            }

            if(transform.GetChild(i).name == "TopOfLevel")
            {
                levelData.height = transform.GetChild(i).transform.localPosition.y;
            }
        }

        levelData.numberOfPlatforms = numOfPlatforms;

        AssetDatabase.CreateAsset(levelData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
