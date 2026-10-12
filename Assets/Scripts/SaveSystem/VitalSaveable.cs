public class VitalSaveable : ISaveable
{
    public void OnSave(SaveData data)
    {
        var vitals = PhysiologySystem.Instance.vitalContainer._vitals;
        data.hp = vitals[VitalType.Hp].Current.Value;
        data.stamina = vitals[VitalType.Stamina].Current.Value;
        data.satiety = vitals[VitalType.Satiety].Current.Value;
    }

    public void OnLoad(SaveData data)
    {
        var vitals = PhysiologySystem.Instance.vitalContainer._vitals;
        vitals[VitalType.Hp].SetValue(data.hp);
        vitals[VitalType.Stamina].SetValue(data.stamina);
        vitals[VitalType.Satiety].SetValue(data.satiety);
    }
}
