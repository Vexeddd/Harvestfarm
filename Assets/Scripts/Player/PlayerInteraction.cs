using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractalbe currentTarget;

    public float detectionDistance = 8f;

    private Ray detectRay;
    private RaycastHit hit;
    // Update is called once per frame
    void Update()
    {
        if (InputContextManager.Instance.Current != InputContext.GamePlay)
        {
            currentTarget = null;
            InteractionUI.Instance.Hide();

            return;
        }

        if (InventorySystem.Instance.isUIOpen())
        {   
            InteractionUI.Instance.Hide();
            return;
        }
        FindInteractableObj();
        InteractHandler();
    }


    private void FindInteractableObj()
    {
        detectRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        if (Physics.Raycast(detectRay, out hit, detectionDistance))
        {
            var interactable = hit.collider.gameObject.GetComponentInParent<IInteractalbe>();

            if (interactable != null)
            {
                currentTarget = interactable;
                if (currentTarget.CanInteract())
                {
                    InteractionUI.Instance.Show(currentTarget.GetInteractTip());
                }
                else
                {
                    currentTarget = null;
                    InteractionUI.Instance.Hide();
                }
            }
            else
            {
                currentTarget = null;
                InteractionUI.Instance.Hide(); 
            }
        }
        else
        {
            currentTarget = null;
            InteractionUI.Instance.Hide();
        }

    }

    private void InteractHandler()
    {
        if (currentTarget == null) return;

        if (Input.GetKeyDown(KeyCode.F) && currentTarget.CanInteract())
        {
            currentTarget.Ineract(gameObject);
        }
    }
}
