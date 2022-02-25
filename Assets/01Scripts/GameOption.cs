using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOption : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullscreenBtn;

    List<Resolution> resolutions = new List<Resolution>();
    [SerializeField] FullScreenMode screenMode;

    [SerializeField] Slider soundSlider;
    [SerializeField] TMPro.TMP_Text soundText;
    int resolutionNum;

    private void Start()
    {
        InitUI();
    }

    public void InitUI()
    {
        for(int i = 0; i< Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRate == 60)
                resolutions.Add(Screen.resolutions[i]);
        }
        resolutionDropdown.options.Clear();
        resolutions.Reverse();

        int optionNum = 0;

        foreach(var item in resolutions)
        {
            TMPro.TMP_Dropdown.OptionData option = new TMPro.TMP_Dropdown.OptionData();
            option.text = item.width + " ¡¿ " + item.height;
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.value = optionNum;
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();

        fullscreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }

    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
    }

    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }


    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }

    public void SoundTextValue(int value)
    {
        soundText.text = value.ToString() + " %";
    }
    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, screenMode);
    }
}
