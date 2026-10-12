using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VehicleInfoPanel : MonoBehaviour
{
    private static VehicleInfoPanel _instance;
    public static VehicleInfoPanel Instance
    {
        get { return _instance; }
    }

    public TMP_Text speedValue;
    public CanvasGroup _cg;

    public VehicleController controller;

    private bool _isOpen;

    private void Awake()
    {
        _instance = this;

        speedValue = this.transform.FindRecursive("SpeedValue").GetComponent<TMP_Text>();
        _cg = this.transform.GetComponent<CanvasGroup>();

        speedValue.text = "0";
    }

    private void Update()
    {
        if (controller == null)
        {
            return;
        }

        speedValue.text = controller.GetSpeedKmh().ToString("F1");
    }

    public void Open(VehicleController controller)
    {
        this.controller = controller;

        _isOpen = true;

        _cg.alpha = _isOpen ? 1 : 0;
        _cg.interactable = _isOpen;
        _cg.blocksRaycasts = _isOpen;

        if (AmmoUI.Instance._cg != null)
        {
            AmmoUI.Instance._cg.alpha = !_isOpen ? 1 : 0;
            AmmoUI.Instance._cg.interactable = !_isOpen;
            AmmoUI.Instance._cg.blocksRaycasts = !_isOpen;
        }
    }


    public void Close()
    {
        this.controller = null;

        _isOpen = false;

        _cg.alpha = _isOpen ? 1 : 0;
        _cg.interactable = _isOpen;
        _cg.blocksRaycasts = _isOpen;

        if (AmmoUI.Instance._cg != null)
        {
            AmmoUI.Instance._cg.alpha = !_isOpen ? 1 : 0;
            AmmoUI.Instance._cg.interactable = !_isOpen;
            AmmoUI.Instance._cg.blocksRaycasts = !_isOpen;
        }
    }
}
