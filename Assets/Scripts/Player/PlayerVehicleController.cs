using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicleController : MonoBehaviour
{
    public VehicleBase currentVehicle;

    public void EnterVehicle(Transform seat, VehicleBase vehicleBase)
    {
        Debug.Log("上车");
        currentVehicle = vehicleBase;

        //关闭玩家_rb
        Player.Instance._rb.velocity = Vector3.zero;
        Player.Instance._rb.isKinematic = true;

        //关闭碰撞器
        Player.Instance._collider.enabled = false;

        //玩家对齐到座位
        transform.SetParent(seat);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        //切换Input上下文
        InputContextManager.Instance.SetContext(InputContext.Driving);

        //玩家驾驶动画
        Player.Instance._animator.SetBool("IsDriving", true);
    }

    public void ExitVehicle()
    {
        Debug.Log("下车");
        Debug.Log(InputContextManager.Instance.Current);
        //玩家出座位
        transform.SetParent(null);

        //开启_rb
        Player.Instance._rb.isKinematic = false;
        Player.Instance._rb.velocity = Vector3.zero;

        //开启碰撞器
        Player.Instance._collider.enabled = true;


        //删掉座位信息
        VehicleSeat.Detack(this);

        //输入上下文
        InputContextManager.Instance.SetContext(InputContext.GamePlay);

        //驾驶动画
        Player.Instance._animator.SetBool("IsDriving", false);

        //移除汽车控制脚本
        Destroy(currentVehicle.GetComponent<VehicleController>());


        //关闭汽车UI
        VehicleInfoPanel.Instance.Close();
    }


    private void Update()
    {
        if (currentVehicle == null)
        {
            return;
        }

        if (InputContextManager.Instance.Current == InputContext.Driving && Input.GetKeyDown(KeyCode.F))
        {
            ExitVehicle();



            currentVehicle.OnExit();


            currentVehicle = null;
        }
    }

}
