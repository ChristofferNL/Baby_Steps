using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatMessageWidget : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chatMessageText;

    public void SetupChatObject(string newText, string playerName)
    {
        chatMessageText.text = $"{playerName} says:\n\n{newText}";
    }
}
