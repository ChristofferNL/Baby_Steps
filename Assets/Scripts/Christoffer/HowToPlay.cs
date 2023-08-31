using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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


    //touch stuff
    [SerializeField] float touchRequiredMoveAmount = 1;
    private bool isGettingTouch;
    private Vector3 touchStartPos;
    private int touchId;

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

    //Below is touch input stuff
    //might break for you christopher since stuff like that happened earlier but lets hope it doesnt
	public void StartGettingTouchInput()
	{
        if (Touchscreen.current != null)
		{
	        for (int i = 0; i < Touchscreen.current.touches.Count; i++)
	        {
	            //Debug.LogError("touch pos:"+ i + " : " + Touchscreen.current.touches[i].position.ReadValue());
	            //figures out the id of the touch which caused the startmovment function to get called and saves that id to use as a reference when getting input later
	            if (Touchscreen.current.touches[i].position.ReadValue() != Vector2.zero && !isGettingTouch)
	            {
	                touchId = i;
	                isGettingTouch = true;
	                touchStartPos = Touchscreen.current.touches[touchId].startPosition.ReadValue();
	            }
	        }
		}
	}
    public void StopGettingTouchInput()
    {
        if (Touchscreen.current.touches[touchId].position.ReadValue().x > touchStartPos.x + touchRequiredMoveAmount && imageState != ImageState.FIRST)
        {
            GoLeft();
        } 
        else if(Touchscreen.current.touches[touchId].position.ReadValue().x < touchStartPos.x - touchRequiredMoveAmount && imageState != ImageState.FOURTH)
        {   
            GoRight();
        }
        isGettingTouch = false;
        touchId = 9999;
    }
}
