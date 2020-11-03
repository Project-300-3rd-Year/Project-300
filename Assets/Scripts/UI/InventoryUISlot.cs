using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Comp, Vars, Prop

    Image image;

    #endregion

    #region Start,Update
    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {

    }

    #endregion

    public void SetSize(int width, int height)
    {
        //Change size of background.
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, height);

        transform.SetParent(UITest.Instance.transform);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
