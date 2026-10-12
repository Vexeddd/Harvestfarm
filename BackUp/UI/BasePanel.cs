using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();
    protected virtual void Awake()
    {
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<InputField>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }


    public virtual void ShowMe()
    {

    }

    public virtual void HideMe()
    {

    }

    public T GetControl<T>(string name) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(name))
        {
            for (int i = 0; i < controlDic[name].Count; i++)
            {
                if (controlDic[name][i] is T)
                {
                    return controlDic[name][i] as T;
                }
            }
        }
        return null;
    }

    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; i++)
        {
            string objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
            {
                controlDic[objName].Add(controls[i]);
            }
            else
            {
                controlDic.Add(objName, new List<UIBehaviour> { controls[i] });
            }

            if (controls[i] is Button)
            {
                (controls[i] as Button).onClick.AddListener(() =>
                {
                    OnClicked(objName);
                });
            }
            else if (controls[i] is Toggle)
            {
                (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                {
                    OnValueChanged(objName, value);
                });
            }
        }
    }

    protected virtual void OnClicked(string btnName)
    {

    }

    protected virtual void OnValueChanged(string toggleName, bool value)
    {

    }
}
