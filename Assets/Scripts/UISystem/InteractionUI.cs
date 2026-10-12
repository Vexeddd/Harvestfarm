using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionUI : SingletonMono<InteractionUI>
{
    private TMP_Text text;
    private GameObject textgo;

    protected override void Awake()
    {
        base.Awake();

        text = GetComponentInChildren<TMP_Text>();
        textgo = text.gameObject;

        Hide();
    }


    public void Show(string tip)
    {
        textgo.SetActive(true);
        text.text = tip;
    }

    public void Hide()
    {
        text.text = null; 
        textgo.SetActive(false);
    }

}
