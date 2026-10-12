using System;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    private Vector3 dir;
    private WeaponData weaponData;
    private float damage;
    private string poolKey;

    private float speed;
    private float gravity = -9.81f;
    private float lifeTime = 5f;

    public Vector3 velocity;
    public void Init(Vector3 direction, WeaponData weaponData, float damage, string poolKey)
    {
        this.dir = direction;
        this.weaponData = weaponData;
        this.speed = weaponData.arrow_speed;
        this.damage = damage;
        this.poolKey = poolKey;

        velocity = direction * speed;
        transform.rotation = Quaternion.LookRotation(direction);

        timer = 0f;
    }

    private float timer;
    void Update()
    {
        //施加下坠
        velocity += Vector3.up * gravity * Time.deltaTime;

        //计算本帧位移
        Vector3 move = velocity * Time.deltaTime;

        if (Physics.Raycast(this.transform.position, move.normalized, out RaycastHit hit, move.magnitude, LayerMask.GetMask("Damageable")))
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var component))
            {
                component.ApplyDamage(damage);
            }

            Recycle();

            return;
        }

        transform.position += move;

        //让子弹始终朝前
        if (velocity.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(velocity);
        }

        //销毁
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Recycle();
        }
    }

    private void Recycle()
    {
        if (!string.IsNullOrEmpty(poolKey))
        {
            PoolMgr.Instance.PushObj(poolKey, this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
