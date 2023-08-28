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
    [SerializeField] Transform LevelObjectsParent;
    private int currentSaveId;
    private void Start()
    {
        GetLevelData();
    }

    [SerializeField] string levelName = "placeholder name";
    private bool haveDone = false;


    public void GetLevelData()
    {
        if(LevelObjectsParent == null)
        {
            LevelObjectsParent = new GameObject().transform;
        }

        //deletes the file if it already exists
        if (File.Exists("Assets/LevelData/" + levelName)) { File.Delete("Assets/LevelData/" + levelName); }

        Debug.Log("child count:" + transform.childCount) ;
        int numOfPlatforms = 0;
        LevelChunkDataSO levelData = ScriptableObject.CreateInstance<LevelChunkDataSO>();
        string path = "Assets/LevelData/" + levelName + ".asset";


        for (int i = 0; i < transform.childCount; i++)
        {
            foreach (Transform child in transform.GetChild(i))
            {
                foreach (Transform child2 in child)
                {
                    if (child2.CompareTag("PassThroughPlatform") || child2.CompareTag("SolidPlatform") && !child.GetComponent<QuestionPlatform>() || child2.GetComponent<QuestionPlatform>())
                    {
                        numOfPlatforms++;
                    }
                }
                if (child.CompareTag("PassThroughPlatform") && !child.GetComponentInChildren<QuestionPlatform>() || child.CompareTag("SolidPlatform") && !child.GetComponentInChildren<QuestionPlatform>())
                {
                    numOfPlatforms++;
                }
            }
            if (transform.GetChild(i).CompareTag("PassThroughPlatform") && !transform.GetChild(i).GetComponentInChildren<QuestionPlatform>() || transform.GetChild(i).CompareTag("SolidPlatform") && !transform.GetChild(i).GetComponentInChildren<QuestionPlatform>())
            {
                numOfPlatforms++;
            }
        }
        levelData.bottomLeft = bottomLeft.position * LevelObjectsParent.localScale.x;
        levelData.numberOfPlatforms = numOfPlatforms;
        levelData.height = topOfLevel.localPosition.y * LevelObjectsParent.localScale.x;
        levelData.position = new Vector3[numOfPlatforms];
        levelData.scale = new Vector3[numOfPlatforms];
        levelData.color = new Color[numOfPlatforms];
        levelData.rotation = new Quaternion[numOfPlatforms];
        levelData.isPassThrough = new bool[numOfPlatforms];
        levelData.isQuestion = new bool[numOfPlatforms];

        currentSaveId = 0;
        /*
        for (int i = 0; i < transform.childCount; i++) 
        {
            /*  ended up searching for question platforms twice, thought this only did it one level deep at first but it also searches grandchildren and their children and so on
            if (transform.GetChild(i).GetComponentInChildren<QuestionPlatform>())
            {
                levelData.isQuestion[currentSaveId] = true;
            }
            

            if (transform.GetChild(i).childCount > 0)
            {
                if(transform.GetChild(i).GetChild(0).GetComponent<QuestionPlatform>())
                levelData.isQuestion[currentSaveId] = true;
            }

            if (transform.GetChild(i).CompareTag("PassThroughPlatform"))
            {
                levelData.isPassThrough[currentSaveId] = true;
            }else if (transform.GetChild(i).CompareTag("SolidPlatform"))
            {
                levelData.isPassThrough[currentSaveId] = false;
            }
            
            if((transform.GetChild(i).CompareTag("PassThroughPlatform")) 
                || (transform.GetChild(i).CompareTag("SolidPlatform")))
                Debug.LogError("youre here");
                levelData.position[currentSaveId] = transform.GetChild(i).transform.localPosition;
                levelData.scale[currentSaveId] = transform.GetChild(i).transform.localScale;
                levelData.rotation[currentSaveId] = transform.GetChild(i).transform.rotation;
                currentSaveId++;
            }

            //GETS CHILDREN OF CHILDREN
            if (transform.GetChild(i).childCount > 0)
            {
                Transform childParent = transform.GetChild(i);
                
                for (int a = 0; a < childParent.childCount; a++)
                {
                    Debug.LogError("childcount: " + childParent.childCount);
                    if (childParent.GetChild(a).GetChild(0).GetComponent<QuestionPlatform>() != null)
                    {
                        
                        levelData.isQuestion[currentSaveId] = true;
                    }

                    if (childParent.GetChild(a).CompareTag("PassThroughPlatform"))
                    {
                        levelData.isPassThrough[currentSaveId] = true;
                    }
                    else if (childParent.GetChild(a).CompareTag("SolidPlatform"))
                    {
                        levelData.isPassThrough[currentSaveId] = false;
                    }

                    if (childParent.GetChild(a).CompareTag("PassThroughPlatform") || transform.GetChild(i).CompareTag("SolidPlatform") || transform.GetChild(i).GetComponentInChildren<QuestionPlatform>())  //if its any type of platform
                    {
                        levelData.position[currentSaveId] = transform.GetChild(i).transform.localPosition + childParent.GetChild(a).transform.localPosition;
                        levelData.scale[currentSaveId] = transform.GetChild(i).transform.localScale + childParent.GetChild(a).transform.localPosition;
                        levelData.rotation[currentSaveId] = transform.GetChild(i).transform.rotation * childParent.GetChild(a).transform.rotation;
                        currentSaveId++;
                    }
                }
            }
            
        }
    */
        for (int i = 0; i < transform.childCount; i++)
        {
            foreach (Transform child in transform.GetChild(i))
            {
                foreach (Transform child2 in child)
                {
                    if (child2.GetComponent<QuestionPlatform>())
                    {
                        levelData.isQuestion[currentSaveId] = true;
                        levelData.position[currentSaveId] = (child2.parent.transform.position - transform.position) /* * LevelObjectsParent.localScale.x*/;
                        levelData.scale[currentSaveId] = child2.parent.transform.localScale * LevelObjectsParent.localScale.x;
                        levelData.rotation[currentSaveId] = child2.parent.transform.rotation;
                        currentSaveId++;
                    }else if (child2.CompareTag("PassThroughPlatform"))
                    {
                        levelData.isPassThrough[currentSaveId] = true;
                        levelData.position[currentSaveId] = (child2.parent.transform.position - transform.position)  /* * LevelObjectsParent.localScale.x*/;
                        levelData.scale[currentSaveId] = child2.parent.transform.localScale * LevelObjectsParent.localScale.x;
                        levelData.rotation[currentSaveId] = child2.rotation;
                        currentSaveId++;
                    }
                    else if(child2.CompareTag("SolidPlatform"))
                    {
                        levelData.isPassThrough[currentSaveId] = false;
                        levelData.position[currentSaveId] = (child2.parent.transform.position - transform.position)  /* * LevelObjectsParent.localScale.x*/;
                        levelData.scale[currentSaveId] = child2.parent.transform.localScale * LevelObjectsParent.localScale.x;
                        levelData.rotation[currentSaveId] = child2.rotation;
                        currentSaveId++;
                    }
                }
                if (child.CompareTag("PassThroughPlatform") && !child.GetComponentInChildren<QuestionPlatform>() || child.CompareTag("SolidPlatform") && !child.GetComponentInChildren<QuestionPlatform>())
                {
                    if(child.CompareTag("PassThroughPlatform") && !child.GetComponentInChildren<QuestionPlatform>())
                    {
                        levelData.isPassThrough[currentSaveId] = true;
                        levelData.position[currentSaveId] = (child.position - transform.position)  /* * LevelObjectsParent.localScale.x*/;
                        levelData.scale[currentSaveId] = child.localScale * LevelObjectsParent.localScale.x;
                        levelData.rotation[currentSaveId] = child.rotation;
                        currentSaveId++;
                    }
                    else if(child.CompareTag("SolidPlatform") && !child.GetComponentInChildren<QuestionPlatform>())
                    {
                        levelData.isPassThrough[currentSaveId] = false;
                        levelData.position[currentSaveId] = (child.position - transform.position)  /* * LevelObjectsParent.localScale.x*/;
                        levelData.scale[currentSaveId] = child.localScale * LevelObjectsParent.localScale.x;
                        levelData.rotation[currentSaveId] = child.rotation;
                        currentSaveId++;
                    }
                }
            }
            if (transform.GetChild(i).CompareTag("PassThroughPlatform") && !transform.GetChild(i).GetComponentInChildren<QuestionPlatform>() || transform.GetChild(i).CompareTag("SolidPlatform") && !transform.GetChild(i).GetComponentInChildren<QuestionPlatform>())
            {
                if (transform.GetChild(i).CompareTag("PassThroughPlatform") && !transform.GetChild(i).GetComponentInChildren<QuestionPlatform>())
                {
                    levelData.isPassThrough[currentSaveId] = true;
                    levelData.position[currentSaveId] = transform.GetChild(i).transform.localPosition  /* * LevelObjectsParent.localScale.x*/;
                    levelData.scale[currentSaveId] = transform.GetChild(i).transform.localScale * LevelObjectsParent.localScale.x;
                    levelData.rotation[currentSaveId] = transform.GetChild(i).transform.rotation;
                    currentSaveId++;
                }
                else if (transform.GetChild(i).CompareTag("SolidPlatform") && !transform.GetChild(i).GetComponentInChildren<QuestionPlatform>())
                {
                    levelData.isPassThrough[currentSaveId] = false;
                    levelData.position[currentSaveId] = transform.GetChild(i).transform.localPosition  /* * LevelObjectsParent.localScale.x*/;
                    levelData.scale[currentSaveId] = transform.GetChild(i).transform.localScale * LevelObjectsParent.localScale.x;
                    levelData.rotation[currentSaveId] = transform.GetChild(i).transform.rotation;
                    currentSaveId++;
                }
            }
        }

        levelData.numberOfPlatforms = numOfPlatforms;

        //AssetDatabase.CreateAsset(levelData, path);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }
}
