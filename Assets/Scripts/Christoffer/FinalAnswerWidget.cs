using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalAnswerWidget : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI yourAnswerText;
    [SerializeField] TextMeshProUGUI theirAnswerText;
    [SerializeField] TextMeshProUGUI sameAnswerText;
    [SerializeField] GameObject yourTextObject;
    [SerializeField] GameObject theirTextObject;
    [SerializeField] GameObject sameTextObject;

    public void SetupAnswerWidget(string question, string yourAnswer, string theirAnswer, bool sameAnswer)
    {
        questionText.text = question;
        yourAnswerText.text = yourAnswer;
        theirAnswerText.text = theirAnswer;
        sameAnswerText.text = theirAnswer;

        if (sameAnswer)
        {
            yourTextObject.SetActive(false);
            theirTextObject.SetActive(false);
            sameTextObject.SetActive(true);
        }
        else
        {
            yourTextObject.SetActive(true);
            theirTextObject.SetActive(true);
            sameTextObject.SetActive(false);
        }
    }
}
