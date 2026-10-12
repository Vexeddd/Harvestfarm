using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysiologySystem : SingletonAutoMono<PhysiologySystem>,
    ICommandHandler<CommandConsumeHP>,
    ICommandHandler<CommandConsumeStamina>,
    ICommandHandler<CommandConsumeSatiely>

{
    public VitalContainer vitalContainer { get; } = new VitalContainer();

    public BuffModule buffModule;

    protected override void Awake()
    {
        base.Awake();

        CommandBus.Register<CommandConsumeHP>(this);
        CommandBus.Register<CommandConsumeStamina>(this);
        CommandBus.Register<CommandConsumeSatiely>(this);

        buffModule = UnityEngine.Object.FindObjectOfType<BuffModule>();
        if (buffModule == null)
        {
            GameObject obj1 = new GameObject("BuffModule");
            buffModule = obj1.AddComponent<BuffModule>();
            obj1.transform.parent = this.transform;
        }
    }


    private void Start()
    {
        VitalStat hp = vitalContainer.Register(VitalType.Hp, 100);
        VitalStat stamina = vitalContainer.Register(VitalType.Stamina, 100);
        VitalStat satiety = vitalContainer.Register(VitalType.Satiety, 100);

        hp.Current.Subscribe((val) =>
        {
            PlayerVitalPanel.Instance.HPText.text = val.ToString();
            PlayerVitalPanel.Instance.HPBar.fillAmount = vitalContainer._vitals[VitalType.Hp].Percent;
        },true);

        stamina.Current.Subscribe((val) =>
        {
            PlayerVitalPanel.Instance.StaminaText.text = val.ToString();
            PlayerVitalPanel.Instance.StaminaBar.fillAmount = vitalContainer._vitals[VitalType.Stamina].Percent;
        }, true);
        satiety.Current.Subscribe((val) =>
        {
            PlayerVitalPanel.Instance.SatietyText.text = val.ToString();
            PlayerVitalPanel.Instance.SatietyBar.fillAmount = vitalContainer._vitals[VitalType.Satiety].Percent;
        }, true);
    }



    public CommandResult Handle(CommandConsumeHP command)
    {
        bool res = vitalContainer.Get(VitalType.Hp).ModifyValue(-command.amount);
        if (!res)
        {
            return new CommandResult { Success = false, Message = "ÉúÃüºÄ¾¡" };
        }

        return CommandResult.OK();
    }

    public CommandResult Handle(CommandConsumeStamina command)
    {
        bool res = vitalContainer.Get(VitalType.Stamina).ModifyValue(-command.amount);

        return CommandResult.OK();
    }

    public CommandResult Handle(CommandConsumeSatiely command)
    {
        bool res = vitalContainer.Get(VitalType.Satiety).ModifyValue(-command.amount);

        return CommandResult.OK();
    }



    void OnEnable()
    {
        EventBus.Subscribe<VitalQueryRequestEvent>(OnVitalQueryRequest);
        EventBus.Subscribe<EatFoodEvent>(OnEatFood);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<VitalQueryRequestEvent>(OnVitalQueryRequest);
        EventBus.Unsubscribe<EatFoodEvent>(OnEatFood);
    }

    private void OnEatFood(EatFoodEvent @event)
    {
        buffModule.OnEat(@event.itemData);
    }

    private void OnVitalQueryRequest(VitalQueryRequestEvent @event)
    {
        var value = vitalContainer.Get(@event.type).Current.Value;

        @event.callBack?.Invoke(value);
    }
}
