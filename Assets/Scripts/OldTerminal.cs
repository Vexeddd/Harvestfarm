using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class OldTerminal : MonoBehaviour, IInteractalbe
{
    public bool CanInteract()
    {
        return true;
    }

    public string GetInteractTip()
    {
        return "°´F ÐÞ¸´·ç³µ";
    }

    public void Ineract(GameObject obj)
    {
        QuestManager.Instance.ReportAction(QuestType.InteractBuilding, "OldTerminal");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
