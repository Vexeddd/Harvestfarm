using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonCamera : MonoBehaviour
{
    private Transform playerTarget;
    private float verticalOffst = 2f;

    private float xSensitivity = 380f;
    private float ySensitivity = 240f;
    private float scrollSensitivity = 2f;

    private float minY = -30f;
    private float maxY = 60f;

    //[SerializeField]
    private float distance = 4f;
    private float minDistance = 2f;
    private float maxDistance = 5f;

    private Vector3 targetPos;
    private Vector3 desirePos;
    private float mouseHorizontalOffset;
    private float mouseVerticalOffset;

    private Vector3 currentVelocity;
    [SerializeField]
    private float smoothTime = 0.1f;

    private void Awake()
    {
        playerTarget = GameObject.Find("Player").transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()   
    {
        if (!IsMouseLocked())
        {
            return;
        }

        mouseHorizontalOffset += Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        mouseVerticalOffset -= Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
        mouseVerticalOffset = Mathf.Clamp(mouseVerticalOffset, minY, maxY);
        Quaternion cameraRotation = Quaternion.Euler(mouseVerticalOffset, mouseHorizontalOffset, 0);

        distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        targetPos = playerTarget.position + Vector3.up * verticalOffst;
        desirePos = targetPos - cameraRotation * Vector3.forward * distance;

        transform.position = Vector3.SmoothDamp(transform.position, desirePos, ref currentVelocity, smoothTime);
        transform.LookAt(targetPos);
    }


    public Vector3 GetCameraFoward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        
        return forward.normalized;
    }

    public bool IsMouseLocked()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }
}
