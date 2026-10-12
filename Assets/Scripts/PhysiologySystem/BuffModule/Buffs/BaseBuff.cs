using UnityEngine;

public class BaseBuff 
{
    public BuffData buffData;
    public float timeLeft;

    public bool IsFinshed => timeLeft <= 0;

    public BaseBuff(BuffData data) 
    {
        buffData = data;
    }

    public virtual void Apply()
    {
        timeLeft = buffData.duration;
    }


    public virtual bool OnUpdate(float deltaTime)
    {
        timeLeft = Mathf.Max(0, timeLeft - deltaTime);

        return IsFinshed;
    }

    public virtual void OnRemoved()
    {
        Debug.Log("buffÒÆ³ýÁË");
    }
}
