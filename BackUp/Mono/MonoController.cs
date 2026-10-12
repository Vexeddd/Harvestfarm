using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoController : MonoBehaviour
{
    private event UnityAction updateEvent;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateEvent != null)
        {
            updateEvent.Invoke();
        }
    }

    public void AddUpdateListener(UnityAction action)
    {
        updateEvent += action;
    }

    public void RemoveUpdateListener(UnityAction action) 
    { 
        updateEvent -= action;
    }
}
