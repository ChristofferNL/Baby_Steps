using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManagerOfDoom : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

	private void Start()
	{
		volumeSlider.onValueChanged.AddListener(VolumeChangeOfDoom);
	}

	public void VolumeChangeOfDoom(float newVolumeOfDoom)
    {
        AudioManager.instance.ChangeVolume(newVolumeOfDoom);
    }
}
