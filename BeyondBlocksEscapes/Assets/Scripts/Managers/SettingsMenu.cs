using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace BBE
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private TMPro.TMP_Dropdown _resolutionDropdown;
        [SerializeField] private Toggle _fullscreenToggle;

        Resolution[] _resolutions;


        private void Start()
        {
            _resolutions = Screen.resolutions;
            _resolutionDropdown.ClearOptions();

            _fullscreenToggle.isOn = Screen.fullScreen;

            _resolutions = RemoveDuplicateResolutions( _resolutions );

            List<string> resolutionOptions = new List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < _resolutions.Length; i++)
            {
                string resolutionString = $"{_resolutions[i].width} x {_resolutions[i].height}";
                if (!resolutionOptions.Contains(resolutionString))
                {
                    resolutionOptions.Add(resolutionString);

                    if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
                    {
                        currentResolutionIndex = i;
                    }
                }

            }

            _resolutionDropdown.AddOptions(resolutionOptions);
            _resolutionDropdown.value = currentResolutionIndex;
            _resolutionDropdown.RefreshShownValue();
        }
        private Resolution[] RemoveDuplicateResolutions(Resolution[] resolutions)
        {
            return resolutions
                .GroupBy(res => new { res.width, res.height }) 
                .Select(group => group.First()) 
                .ToArray(); 
        }



        public void SetVolume(float volume)
        {
            _audioMixer.SetFloat("Volume", volume);
        }

        public void SetGraphicsQuality(int graphicQualityIndex)
        {
            QualitySettings.SetQualityLevel(graphicQualityIndex);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            if (resolutionIndex >= 0 && resolutionIndex < _resolutions.Length)
            {
                
                Resolution resolution = _resolutions[resolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
        }
    }
}