using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : UIContainer
{
    private static DialogPanel _instance;
    public static DialogPanel Instance
    {
        get { return _instance; }
    }

    public Image _npcImage; //npc头像
    public TMP_Text _npcName; //npc名称
    public TMP_Text _dialogText; //对话内容

    private void Awake()
    {
        _instance = this as DialogPanel;

        _npcImage = transform.FindRecursive("NpcImage").GetComponent<Image>();
        _npcName = transform.FindRecursive("NpcName").GetComponent<TMP_Text>();
        _dialogText = transform.FindRecursive("DialogText").GetComponent<TMP_Text>();
    }

    public void Show(NpcRegisterDialogEvent npcEvent)
    {
        //_npcImage.sprite = ...

        _npcName.text = npcEvent.data.npc;

        //修改文字
        Play(npcEvent.data.lines.ToList(), () =>
        {
            npcEvent.callBack?.Invoke();    
        });

        //显示
        IsOpen = true;

        if (WeaponSystem.Instance.currentWeapon != null)
        {
            WeaponSystem.Instance.currentWeapon.isDialog = true;
        }
    }

    public List<string> _lines; //所有对话
    public int _lineIndex; //当前的对话的索引
    public Action onFinished; //完成回调
    public Coroutine _typingCoroutine; //打字的协程
    public bool isTyping; //正在打字
    public bool isWaiting; //是否正在等待

    public float charInterval = 0.04f; //打一个字的间隔

    private void Play(List<string> list, Action value)
    {
        _lines = list;
        _lineIndex = 0;
        onFinished = value;

        _dialogText.text = "";
        PlayLine();
    }

    private void PlayLine()
    {
        if (_lineIndex >= _lines.Count)
        {
            FinishedDialogue();
            return;
        }

        _typingCoroutine = StartCoroutine(TypeLine(_lines[_lineIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        isWaiting = false;

        _dialogText.text = "";

        for (int i = 0; i < line.Length; i++)
        {
            _dialogText.text += line[i];

            yield return new WaitForSeconds(charInterval);
        }

        isTyping = false;
        isWaiting = true;
    }

    private void FinishedDialogue()
    {
        IsOpen = false;

        if (WeaponSystem.Instance.currentWeapon != null)
        {
            WeaponSystem.Instance.currentWeapon.isDialog = false;
        }


        _dialogText.text = "";

        onFinished?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isWaiting)
                {
                    _lineIndex += 1;
                    PlayLine();
                }

                else if (isTyping)
                {
                    StopCoroutine(_typingCoroutine);

                    _dialogText.text = _lines[_lineIndex].ToString();

                    isTyping = false;
                    isWaiting = true;
                }
            }
        }
    }
}
