using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tip : UIContainer
{
    public override bool IsOpen
    {
        get
        {
            return base.IsOpen;
        }
        set
        {
            base.IsOpen = value;

            if (_cg == null)
            {
                _cg = GetComponent<CanvasGroup>();
            }

            _cg.interactable = false;
            _cg.blocksRaycasts = false;
        }
    }

    protected override void OnCursoerStateChanged()
    {

    }


    public Image tipIcon;
    public TMP_Text tipTitle;
    public TMP_Text tipDescribe;
    public RectTransform _rt;

    private Vector2 offset = new Vector2(200,60);

    private void Awake()
    {
        tipIcon = transform.Find("TipIcon").GetComponent<Image>();
        tipTitle = transform.Find("TipTitle").GetComponent<TMP_Text>();
        tipDescribe = transform.Find("TipDescribe").GetComponent<TMP_Text>();

        _cg = GetComponent<CanvasGroup>();
        _rt = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOpen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIMgr.Instance.canvas.GetComponent<RectTransform>(),
                                                                    Input.mousePosition,
                                                                    UIMgr.Instance.UICamera,
                                                                    out Vector2 localPoint);


            _rt.anchoredPosition = Vector2.Lerp(_rt.anchoredPosition,
                                                localPoint + offset,
                                                Time.deltaTime * 35);  
        }
    }

    public void SetUI(ItemData itemData)
    {
        if (itemData == null)
        {
            return;
        }

        tipIcon.sprite = itemData.icon;
        tipTitle.text = itemData.name.ToString();
        tipDescribe.text = itemData.describe.ToString();
    }



    public override void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
