using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightedButton : Button
{
    TextMeshProUGUI text;
    Image buttonImage;

    UIGamePlayManager manager = null;

    protected override void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        buttonImage = GetComponent<Image>();
    }
    void Update()
    {
        if (manager != null)
        {
            //Check if the GameObject is being highlighted
            if (IsHighlighted())
            {
                text.color = Color.black;
                buttonImage.sprite = manager.selectedSprite;
            }
            else
            {
                text.color = Color.white;
                buttonImage.sprite = manager.notSelectedSprite;
            }
        }
        else
        {
            manager = FindObjectOfType<UIGamePlayManager>();
        }

    }
}
