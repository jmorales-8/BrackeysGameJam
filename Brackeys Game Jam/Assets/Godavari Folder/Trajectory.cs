﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [Header("Line renderer veriables")]
    public LineRenderer BodyPredictLine;
    [Range(2, 30)]
    public int resolution;

    [Header("Formula variables")]
    public Vector2 velocity;
    public float yLimit; //for later
    private float g;

    [Header("Linecast variables")]
    [Range(2, 30)]
    public int BodyPredictLineResolution;
    public LayerMask CanHit;

    private void Start()
    {
        g = Mathf.Abs(Physics2D.gravity.y);
    }

    private void Update()
    {
        StartCoroutine(RenderArc());
    }

    private IEnumerator RenderArc()
    {
        BodyPredictLine.positionCount = resolution + 1;
        BodyPredictLine.SetPositions(CalculateLineArray());
        yield return null;
    }

    private Vector3[] CalculateLineArray()
    {
        Vector3[] lineArray = new Vector3[resolution + 1];

        var lowestTimeValue = MaxTimeX() / resolution;

        for (int i = 0; i < lineArray.Length; i++)
        {
            var t = lowestTimeValue * i;
            lineArray[i] = CalculateLinePoint(t);
        }

        return lineArray;
    }

    private Vector2 HitPosition()
    {
        var lowestTimeValue = MaxTimeY() / BodyPredictLineResolution;

        for (int i = 0; i < BodyPredictLineResolution + 1; i++)
        {
            var t = lowestTimeValue * i;
            var tt = lowestTimeValue * (i + 1);

            var hit = Physics2D.Linecast(CalculateLinePoint(t), CalculateLinePoint(tt), CanHit);

            if (hit)
                return hit.point;
        }

        return CalculateLinePoint(MaxTimeY());
    }

    private Vector3 CalculateLinePoint(float t)
    {
        float x = velocity.x * t;
        float y = (velocity.y * t) - (g * Mathf.Pow(t, 2) / 2);
        return new Vector3(x + transform.position.x, y + transform.position.y);
    }

    private float MaxTimeY()
    {
        var v = velocity.y;
        var vv = v * v;

        var t = (v + Mathf.Sqrt(vv + 2 * g * (transform.position.y - yLimit))) / g;
        return t;
    }

    private float MaxTimeX()
    {
        var x = velocity.x;
        if(x == 0)
        {
            velocity.x = 000.1f;
            x = velocity.x;
        }
        
        var t = (HitPosition().x - transform.position.x) / x;
        return t;
    }
}