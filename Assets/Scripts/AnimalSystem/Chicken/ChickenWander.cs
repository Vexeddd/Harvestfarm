using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenWander : MonoBehaviour
{
    public Chicken chicken;

    private float moveSpeed = 1f;
    private float rotateSpeed = 270f;
    private bool isMoving = false;

    private float minMoveTime = 1f;
    private float maxMoveTime = 3f;
    private float minIdleTime = 1f;
    private float maxIdleTime = 3f;

    private Vector3 moveTarget = Vector3.zero;
    private float stateTimer;

    void Start()
    {
        ChooseNextState();
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            ChooseNextState();
        }

        if (isMoving)
        {
            MoveInsideBounds();
        }
    }


    public void ChooseNextState()
    {
        float index = UnityEngine.Random.value;
        if (index >= 0.55f)
        {
            ChickenIdle();
        }
        else
        {
            ChickenMove();
        }
    }

    private void ChickenIdle()
    {
        isMoving = false;
        stateTimer = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
    }

    private void ChickenMove()
    {
        isMoving = true;
        stateTimer = UnityEngine.Random.Range(minMoveTime, maxMoveTime);

        moveTarget = chicken.chickenCoop.GetRandomPointInArea();
    }

    private void MoveInsideBounds()
    {
        Vector3 moveDir = (moveTarget - this.transform.position).normalized;
         
        //this.transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        this.transform.position += moveDir * moveSpeed * Time.deltaTime;

        Quaternion lookRotation = Quaternion.LookRotation(moveDir, Vector3.up);
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);

        if (Vector3.Distance(this.transform.position, moveTarget) <= 0.1f)
        {
            moveTarget = chicken.chickenCoop.GetRandomPointInArea();
        }
    }
}
