using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractPanel : UIContainer
{
    private static NPCInteractPanel _instance;
    public static NPCInteractPanel Instance
    {
        get { return _instance; }
    }

    public NpcController currentNpc;
    public Transform _optionsLayout;

    private void Awake()
    {
        _instance = this as NPCInteractPanel;

        _optionsLayout = this.transform.FindRecursive("OptionsLayout");
        _cg = this.GetComponent<CanvasGroup>();
    }

    public void Open(NpcController npcController)
    {
        ClearOptions();

        this.currentNpc = npcController;

        if (WeaponSystem.Instance.currentWeapon != null)
        {
            WeaponSystem.Instance.currentWeapon.isDialog = true;
        }

        StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        yield return new WaitForSeconds(0.1f);

        AddPlotOption();

        AddOption("闲聊", OnChat);
        AddOption("送礼", OnGift);
        AddOption("离开", OnLeave);

        this.IsOpen = true;
    }

    private void AddPlotOption()
    {
        if (NpcSystem.Instance.plot_dialog.ContainsKey(currentNpc.npcData.npcId))
        {
            AddOption("剧情", () =>
            {
                OnStory(NpcSystem.Instance.plot_dialog[currentNpc.npcData.npcId]);
            });
        }
    }


    private void AddOption(string v, Action callBack)
    {
        var go = Instantiate(NpcSystem.Instance.OptionBlock_prefab, _optionsLayout);
        go.GetComponentInChildren<TMP_Text>().text = v;
        go.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            callBack?.Invoke();
        });
    }

    private void OnStory(NpcRegisterDialogEvent npcRegisterDialogEvent)
    {
        DialogPanel.Instance.Show(npcRegisterDialogEvent);

        NpcSystem.Instance.plot_dialog.Remove(currentNpc.npcData.npcId);

        OnLeave();
        //上报任务
        //QuestManager.Instance.ReportAction(QuestType.TalkToNPC, currentNpc.npcData.npcId);
    }


        
    private void OnChat()
    {
        //QuestManager.Instance.ReportAction(QuestType.TalkToNPC, currentNpc.npcData.npcId);

        Debug.Log("和NPC闲聊");
    }

    private void OnGift()
    {
        Debug.Log("送礼");
    }

    private void OnLeave()
    {
        IsOpen = false;

        if (WeaponSystem.Instance.currentWeapon != null)
        {
            WeaponSystem.Instance.currentWeapon.isDialog = false;
        }

        currentNpc.isInteracting = false;
        currentNpc = null;
    }

    private void ClearOptions()
    {
        var count = _optionsLayout.childCount;

        //倒着销毁
        for (int i = count - 1; i >= 0; i--)
        {
            Destroy(_optionsLayout.GetChild(i).gameObject);
        }
    }
}
