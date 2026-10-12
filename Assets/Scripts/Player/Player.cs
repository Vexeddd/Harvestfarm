using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private static Player _instance;
    public static Player Instance
    {
        get { return _instance; }
    }

    public void ApplyDamage(float damage)
    {
        CommandBus.Send(new CommandConsumeHP
        {
            amount = damage
        });
    }

    public PlayerVehicleController pvc;

    public Rigidbody _rb;
    public Collider _collider;
    public Animator _animator;

    private void Awake()
    {
        _instance = this;

        pvc = GetComponent<PlayerVehicleController>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _animator = GetComponentInChildren<Animator>();
    }
}
