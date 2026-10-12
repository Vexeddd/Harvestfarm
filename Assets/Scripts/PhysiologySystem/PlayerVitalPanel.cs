using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVitalPanel : SingletonMono<PlayerVitalPanel>
{
    public TMP_Text HPText;
    public TMP_Text StaminaText;
    public TMP_Text SatietyText;

    public Image HPBar;
    public Image StaminaBar;
    public Image SatietyBar;
    protected override void Awake()
    {
        base.Awake();

        HPText = this.transform.FindRecursive("HPText").gameObject.GetComponent<TMP_Text>();
        StaminaText = this.transform.FindRecursive("StaminaText").gameObject.GetComponent<TMP_Text>();
        SatietyText = this.transform.FindRecursive("SatietyText").gameObject.GetComponent<TMP_Text>();

        HPBar = this.transform.FindRecursive("HPBar").gameObject.GetComponent<Image>();
        StaminaBar = this.transform.FindRecursive("StaminaBar").gameObject.GetComponent<Image>();
        SatietyBar = this.transform.FindRecursive("SatietyBar").gameObject.GetComponent<Image>();
    }
}
