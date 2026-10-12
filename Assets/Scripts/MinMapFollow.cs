using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMapFollow : MonoBehaviour
{
    public Transform _playerArrow;
    public float cameraHeight = 120f;

    // Start is called before the first frame update
    void Start()
    {
        if (_playerArrow == null)
        {
            _playerArrow = GameObject.Find("PlayerArrow").transform;
        }
    }

    private void LateUpdate()
    {
        Vector3 pos = Player.Instance.transform.position;
        pos.y = cameraHeight;
        this.transform.position = pos;

        if (_playerArrow != null)
        {
            _playerArrow.transform.localEulerAngles = new Vector3(0f, 0f, -Player.Instance.transform.eulerAngles.y);
        }
    }
}
