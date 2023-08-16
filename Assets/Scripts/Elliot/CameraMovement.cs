using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] Vector3 offset;
    [SerializeField] float sizeDistanceMultiplier = 2;
    [SerializeField] float sizeChangeLerpSpeed = 0.2f;
    [SerializeField] float baseSize = 3;
    [SerializeField] float maxSize = 100;
    [SerializeField] Camera mainCamera;

    private Vector3 _centerPosition;

    public Transform player1;       //the first target for the camera (player)
    public Transform player2;       //the second target for the camera (player)

    //shake
    [SerializeField] float smoothSpeed = 0.2f;   //speed at which the camera follows the player
    [SerializeField] float shakeSpeed = 10;
    [SerializeField] float maxShakeOffset = 2;
    [SerializeField] float shakeDecreaseSpeed = 2;
    private Vector3 shakeOffset;
    private float shakeOffsetX;
    private float shakeOffsetY;
    private float shakeOffsetZ;
    private float distanceBetweenPlayers;
    public float trauma;


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.L))
        {
            AddCameraShake(0.6f);
        }
    }

    void FixedUpdate()
    {
        distanceBetweenPlayers = Vector3.Distance(player1.position, player2.position);
        _centerPosition = Vector3.Lerp(player1.position, player2.position, 0.5f);

        Vector3 desiredPosition = _centerPosition + offset;                                          //Position the camera will try to go to
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);  //Position the camera will be at on the path to the desired position

        transform.position = smoothedPosition;


        //CAMERA SHAKE//

        trauma = Mathf.Clamp((trauma - 1f * Time.deltaTime * shakeDecreaseSpeed), 0, 1);

        if (trauma > 0)
        {
            shakeOffsetX = maxShakeOffset * trauma * (Mathf.PerlinNoise(Random.Range(0, 10), Time.time * shakeSpeed) * 2 - 1);
            shakeOffsetY = maxShakeOffset * trauma * (Mathf.PerlinNoise(Random.Range(0, 10), Time.time * shakeSpeed) * 2 - 1);
            shakeOffsetZ = maxShakeOffset * trauma * (Mathf.PerlinNoise(Random.Range(0, 10), Time.time * shakeSpeed) * 2 - 1);
        }
        else
        {
            shakeOffsetX = 0;
            shakeOffsetY = 0;
            shakeOffsetZ = 0;
        }

        shakeOffset = new Vector3(shakeOffsetX, shakeOffsetY, shakeOffsetZ);
        transform.position += shakeOffset;

        float changeValue = Mathf.Lerp(mainCamera.orthographicSize, baseSize + distanceBetweenPlayers * sizeDistanceMultiplier, sizeChangeLerpSpeed);
        mainCamera.orthographicSize = Mathf.Clamp(changeValue, baseSize,maxSize);

    }

    private void AddCameraShake(float shakeToAdd)
    {
        trauma += shakeToAdd;
    }
}
