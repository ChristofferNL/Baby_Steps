using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Chunk", menuName ="Level")]
public class LevelChunkDataSO : ScriptableObject
{
    public int numberOfPlatforms;
    public bool[] isPassThrough;
    public Vector3[] position;
    public Quaternion[] rotation;
    public Vector3 bottomLeft;
    public Color[] color;
    public float height;
    public Vector3[] scale;
}
