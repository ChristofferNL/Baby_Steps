using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLineDraw : MonoBehaviour
{
    [SerializeField] Transform[] points;
    [SerializeField] Transform player1;
    [SerializeField] Transform player2;
    [SerializeField] LineRenderer LineRenderer;

    private void FixedUpdate()
    {
        LineRenderer.positionCount = points.Length + 2;

        LineRenderer.SetPosition(0, player1.position);  //sets the first position to player 1
        for (int i = 0; i < points.Length; i++) 
        {
            LineRenderer.SetPosition(i + 1, points[i].position);
        }
        LineRenderer.SetPosition(points.Length + 1, player2.position);  //sets the last position to player 1
    }
}
