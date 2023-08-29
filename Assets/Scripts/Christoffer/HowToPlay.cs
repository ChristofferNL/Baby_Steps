using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    public enum ImageState
    {
        FIRST,
        SECOND,
        THIRD,
        FOURTH
    }

    ImageState imageState;

    [SerializeField] Image activeImageObject;
    [SerializeField] Sprite imageOne;
    [SerializeField] Sprite imageTwo;
    [SerializeField] Sprite imageThree;
    [SerializeField] Sprite imageFour;
    [SerializeField] Button goRightButton;
    [SerializeField] Button goLeftButton;

	private void Start()
	{
		SetVisuals();
	}

	public void GoRight()
    {
        imageState++;
        SetVisuals();
    }

    public void GoLeft() 
    {
        imageState--;
        SetVisuals();
    }

    void SetVisuals()
    {
        switch (imageState) 
        {
            case ImageState.FIRST:
                goRightButton.gameObject.SetActive(true);
                goLeftButton.gameObject.SetActive(false);
				activeImageObject.sprite = imageOne;
				break; 

            case ImageState.SECOND:
				goRightButton.gameObject.SetActive(true);
				goLeftButton.gameObject.SetActive(true);
                activeImageObject.sprite = imageTwo;
				break;

            case ImageState.THIRD:
				goRightButton.gameObject.SetActive(true);
				goLeftButton.gameObject.SetActive(true);
                activeImageObject.sprite = imageThree;
				break;
            case ImageState.FOURTH:
                goRightButton.gameObject.SetActive(false);
                goLeftButton.gameObject.SetActive(true);
                activeImageObject.sprite = imageFour;
                break;
        }
    }

	public void OpenHowToPlay()
	{
		this.gameObject.SetActive(true);
	}

	public void CloseHowToPlay()
    {
        this.gameObject.SetActive(false);
    }
}
