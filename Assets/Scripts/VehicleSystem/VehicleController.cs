using System;
using UnityEngine;


/// <summary>
/// 操控汽车
/// </summary>
public class VehicleController : MonoBehaviour
{
    //操控汽车
    // 监听 wasd 前进后退 

    //
    public float maxForwardSpeed = 20f; //最大前进速度
    public float maxReverseSpeed = 6f; //最大倒车速度

    public float forwardAcceleration = 10f; //前进加速度
    public float reverseAcceleration = 6f; //倒车加速度

    //
    public float breakeDeceleration = 30f; //刹车速度
    public float naturalDeceleration = 5f; //松油门后自然减速

    public float maxTurnAngle = 45f; //最大转向速度
    public float minTurnAngle = 15f; //最小转向速度

    public float maxTurnSpeed = 1; //每秒转角速度

    public float currentSpeed; //当前速度
    public float currentTurnAngle; //当前转角速度


    public Rigidbody _rb;
    public VehicleBase _vehicle; //车辆


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _vehicle = GetComponent<VehicleBase>();

        _rb.centerOfMass = new Vector3(0, -0.5f, 0); //降低重心, 防止侧翻

        _rb.interpolation = RigidbodyInterpolation.Interpolate; //平滑运动
        _rb.drag = 0.1f; //空气阻力
        _rb.angularDrag = 2f; //角速度阻力
    }

    //物理移动
    private void FixedUpdate()
    {
        //如果不控制
        if (InputContextManager.Instance.Current != InputContext.Driving)
        {
            return;
        }

        HandleMovement(); //处理前进后退
        HandleSteering(); //处理转弯



    }

    private void HandleSteering()
    {
        //静止情况下 车身不会产生转弯角度
        if (Mathf.Abs(currentSpeed) < 0.1f)
        {
            currentTurnAngle = 0f;
            return;
        }

        //监听键盘按键
        float steerInput = 0f;
        if (Input.GetKey(KeyCode.A)) steerInput = -1;
        if (Input.GetKey(KeyCode.D)) steerInput = 1;

        //速度影响因素
        float speedFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxForwardSpeed);

        //倒车反向转弯
        float directionSign = currentSpeed >= 0f ? 1f : -1f;

        //目标旋转角度
        float targetTurnAngle = Mathf.Lerp(maxTurnAngle, minTurnAngle, speedFactor)
                                * steerInput * directionSign;


        //角度限制
        currentTurnAngle = Mathf.MoveTowards(currentTurnAngle, targetTurnAngle,
            maxTurnSpeed * Time.fixedDeltaTime
        );

        //换算成四元素
        Quaternion turnRotation = Quaternion.Euler(0f, currentTurnAngle, 0f);

        //刚体应用旋转
        _rb.MoveRotation(_rb.rotation * turnRotation);








    }

    private void HandleMovement()
    {
        bool pressForward = Input.GetKey(KeyCode.W); //监听前进键按下
        bool pressReverse = Input.GetKey(KeyCode.S); //监听后退键按下

        //如果前进
        if (pressForward)
        {
            //前进逻辑=================================
            //从0开始加速

            //如果现在是倒车
            if (currentSpeed < 0f)
            {
                //先刹车
                currentSpeed = Mathf.MoveTowards(
                    currentSpeed,
                    0f,
                    breakeDeceleration * Time.fixedDeltaTime
                );
            }
            else
            {
                //如果倒车到最大速度了, 按下W 应该是刹车
                currentSpeed += forwardAcceleration * Time.fixedDeltaTime;
                //最大值不能超过汽车的最大速度
                currentSpeed = MathF.Min(currentSpeed, maxForwardSpeed);
            }



        }
        else if (pressReverse)
        {
            //判断当前速度 如果速度大于0 , 说明当前是正在前进中 急刹车
            if (currentSpeed > 0f)
            {
                //先刹车
                currentSpeed = Mathf.MoveTowards(
                    currentSpeed,
                    0f,
                    breakeDeceleration * Time.fixedDeltaTime
                );
            }
            else
            {
                //倒车逻辑=================================
                currentSpeed -= reverseAcceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Max(currentSpeed, -maxReverseSpeed);
            }

        }
        else
        {
            //松开油门, 自然减速到静止
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                0f,
                naturalDeceleration * Time.fixedDeltaTime
            );

        }

        Vector3 velocity = transform.forward * currentSpeed;
        velocity.y = _rb.velocity.y; //保留重力
        _rb.velocity = velocity;


    }

    //显示速度ui
    public float GetSpeedKmh()
    {
        return _rb.velocity.magnitude * 3.6f;
    }

}