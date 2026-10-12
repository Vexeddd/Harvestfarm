using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CanvasGroup _cg;

    private bool _isOpen;
    public virtual bool IsOpen
    {
        get
        {
            return _isOpen;
        }
        set
        {
            _isOpen = value;

            if (_cg == null)
            {
                _cg = GetComponent<CanvasGroup>();
            }
            _cg.alpha = _isOpen ? 1 : 0;
            _cg.interactable = _isOpen;
            _cg.blocksRaycasts = _isOpen;
            OnCursoerStateChanged();
        }
    }

    protected virtual void OnCursoerStateChanged()
    {

        if (_isOpen)
        {
            UIMgr.Instance.ShowCursor();

            UIMgr.Instance.crossHair.SetActive(false);
        }
        else
        {
            UIMgr.Instance.HideCursor();

            UIMgr.Instance.crossHair.SetActive(true);
        }
    }


    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        //print($"鼠标进入了container：{this.name}");
        InventorySystem.Instance.focusContainer = this;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //print($"鼠标离开了container：{this.name}");
        InventorySystem.Instance.focusContainer = null;
    }

    public virtual void OpenContainer()
    {
        if (_cg == null)
        {
            _cg = GetComponent<CanvasGroup>();
        }

        _cg.alpha = 1;
        _cg.interactable = true;
        _cg.blocksRaycasts = true;
    }
    public virtual void CloseContainer()
    {
        if (_cg == null)
        {
            _cg = GetComponent<CanvasGroup>();
        }

        _cg.alpha = 0;
        _cg.interactable = false;
        _cg.blocksRaycasts = false;
    }
}
