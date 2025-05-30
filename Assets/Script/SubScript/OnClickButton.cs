using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*
[System.Serializable]
public class IntEvent : UnityEvent<int>{}
*/

public class OnClickButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("クリック時のイベント")]
    public UnityEvent onClickEvent;

    [Header("カーソルが乗った時のイベント")]
    public UnityEvent onHoverEnterEvent;

    [Header("カーソルが離れた時のイベント")]
    public UnityEvent onHoverExitEvent;

    [Header("カーソルが乗って、かつ動いた時のイベント")]
    public UnityEvent onHoverMoveEvent;

    private bool isPointerOver = false;
    private Vector3 lastMousePosition;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClickEvent != null)
        {
            onClickEvent.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        lastMousePosition = Input.mousePosition;

        if (onHoverEnterEvent != null)
        {
            onHoverEnterEvent.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;

        if (onHoverExitEvent != null)
        {
            onHoverExitEvent.Invoke();
        }
    }

    void Update(){
        if(isPointerOver){
            if(Input.mousePosition != lastMousePosition){
                lastMousePosition = Input.mousePosition;

                if(onHoverMoveEvent != null){
                    onHoverMoveEvent.Invoke();
                }
            }
        }
    }
}

