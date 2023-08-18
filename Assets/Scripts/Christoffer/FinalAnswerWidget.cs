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

    public void SetupAnswerWidget(string question, string yourAnswer, string theirAnswer)
    {
        questionText.text = question;
        yourAnswerText.text = yourAnswer;
        theirAnswerText.text = theirAnswer;
        sameAnswerText.text = theirAnswer;

        if (yourAnswer == theirAnswer)
        {
            yourAnswerText.gameObject.SetActive(false);
            theirAnswerText.gameObject.SetActive(false);
            sameAnswerText.gameObject.SetActive(true);
        }
        else
        {
            yourAnswerText.gameObject.SetActive(true);
            theirAnswerText.gameObject.SetActive(true);
            sameAnswerText.gameObject.SetActive(false);
        }
    }
}
