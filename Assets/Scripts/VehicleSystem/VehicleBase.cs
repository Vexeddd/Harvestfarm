using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class VehicleBase : MonoBehaviour, IInteractalbe
{
    public List<Transform> seats = new List<Transform>();

    public Transform _seatList;

    public bool isInteracting = false;

    public float reenterCooldown = 1f;
    private float lastExitTime = -999f;

    private void Awake()
    {
        _seatList = transform.FindRecursive("SeatList");

        for (int i = 0; i < _seatList.childCount; i++)
        {
            seats.Add(_seatList.GetChild(i));
        }
    }

    public bool CanInteract()
    {
        return !isInteracting && Time.time - lastExitTime > reenterCooldown;
    }

    public string GetInteractTip()
    {
        return "按F 上车";
    }

    public void Ineract(GameObject obj)
    {
        if (!isInteracting)
        {
            //TryEnter();
            StartCoroutine(TryEnter());
        }
        isInteracting = true;
    }

    IEnumerator TryEnter()
    {
        yield return null;

        for (int i = 0; i < seats.Count; i++)
        {
            if (!VehicleSeat.IsOccupied(seats[i]))
            {
                //记录座位
                VehicleSeat.Attack(Player.Instance.pvc, seats[i]);

                //玩家上车
                Player.Instance.pvc.EnterVehicle(seats[i], this);

                //挂载汽车控制脚本
                var controller = this.gameObject.AddComponent<VehicleController>();

                //显示车辆信息UI
                VehicleInfoPanel.Instance.Open(controller);

                //退出
                break;
            }
            else
            {

            }
        }
    }

    public void OnExit()
    {
        lastExitTime = Time.time;
        isInteracting = false;
    }
}
