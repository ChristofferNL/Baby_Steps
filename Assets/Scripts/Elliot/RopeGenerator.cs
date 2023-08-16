using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;


public class RopeGenerator : MonoBehaviour
{
    [SerializeField] GameObject ropeJointPrefab;
    [SerializeField] int numberOfJoints = 10;
    [SerializeField] float ropeLenght = 10;
    public Transform player1;
    public Transform player2;

    private Rigidbody2D previousRb;
    [SerializeField] float timeBetweenRopeLoad = 0.04f;

    private void Start()
    {
        StartCoroutine(GenerateRope());
    }

    public IEnumerator GenerateRope()
    {
        yield return new WaitForSeconds(5);
        Vector3 vectorBetweenPlayers = player2.position - player1.position;
        float distanceBetweenPlayers = vectorBetweenPlayers.magnitude;

        float distanceBetwenJoints = ropeLenght / numberOfJoints;

        for (float i = 0; i < numberOfJoints; i++)
        {
            Vector3 spawnPos;
            float lerpValue = i / numberOfJoints;
            spawnPos = Vector3.Lerp(player1.position + vectorBetweenPlayers.normalized * distanceBetweenPlayers / numberOfJoints, player2.position - vectorBetweenPlayers.normalized * distanceBetweenPlayers / numberOfJoints, lerpValue);
            //spawnPos = Vector3.Lerp(player2.position, player1.position , lerpValue);
            //Vector3 spawnPos = player1.transform.position + vectorBetweenPlayers.normalized * distanceBetweenPlayers * ((i + 1) / numberOfJoints);
            GameObject spawnedObject = Instantiate(ropeJointPrefab, spawnPos, Quaternion.identity);
            DistanceJoint2D joint = spawnedObject.GetComponent<DistanceJoint2D>();

            if (i == 0)
            {
                joint.connectedBody = player1.GetComponent<Rigidbody2D>();
            }
            else
            {
                joint.connectedBody = previousRb;
            }
            previousRb = spawnedObject.GetComponent<Rigidbody2D>();
            joint.distance = distanceBetwenJoints;

            yield return new WaitForSeconds(timeBetweenRopeLoad);
        }

        player2.GetComponent<DistanceJoint2D>().connectedBody = previousRb;
        player2.GetComponent<DistanceJoint2D>().distance = distanceBetwenJoints;
    }
}
