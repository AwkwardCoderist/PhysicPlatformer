using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent unityEvent;
    [SerializeField] private GameObject focusObj;
    public void Focus()
    {
        focusObj.SetActive(true);
    }

    public void Interact()
    {
        unityEvent.Invoke();
    }

    public void Unfocus()
    {
        focusObj.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMovement player))
        {
            player.CurrentInteractable = this;
            Focus();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMovement player))
        {
            if (player.CurrentInteractable as InteractableButton)
            {
                if ((player.CurrentInteractable as InteractableButton) == this)
                {
                    player.CurrentInteractable = null;
                    Unfocus();
                }
            }
        }
    }
}
