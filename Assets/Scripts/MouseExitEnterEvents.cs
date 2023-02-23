using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class MouseExitEnterEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event EventHandler onMouseEnter;
    public event EventHandler onMouseExit;


    public void OnPointerEnter(PointerEventData eventData)
    {
        onMouseEnter?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onMouseExit?.Invoke(this, EventArgs.Empty);
    }
}
