using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageWidget : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chatMessageText;
    [SerializeField] Color playerOneColor;
    [SerializeField] Color playerTwoColor;
    [SerializeField] Image backgroundChatBox;

    public void SetupChatObject(string newText, string playerName, bool isPlayerOne)
    {
        chatMessageText.text = $"{playerName} says:\n\n{newText}";
        if (isPlayerOne)
        {
            backgroundChatBox.color = playerOneColor;
        }
        else
        {
            backgroundChatBox.color = playerTwoColor;
        }
    }
}
