using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UINavigator : MonoBehaviour
{
    [SerializeField] GameObject playSelectUI;
    [SerializeField] GameObject settingsUI;
    [SerializeField] GameObject titleMenuUI;
    [SerializeField] GameObject pauseUI;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject gameTimeUI;
    [SerializeField] CanvasGroup fader;
    [SerializeField] float fadeTime;

    [SerializeField] private Text resultText;

    [SerializeField] private Button pauseButton;

    [SerializeField] private Toggle cameraToggle;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Slider volumeSlider;

    [SerializeField] private AudioMixer mixer;

    Resolution[] resolutions;


    public delegate void PauseStateEventHandler(object source, EventArgs args);

    public event PauseStateEventHandler PauseStateChanged;

    private void Awake()
    {
        SetDependencies();
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
        if (SceneManager.GetSceneByBuildIndex(0).isLoaded)
            SetTitleUI();
        else if (SceneManager.GetSceneByBuildIndex(1).isLoaded)
            SetGameTimeUI();

        CreateResolutionOptions();
        mixer.SetFloat("volume", Mathf.Log10(PlayerPrefs.GetFloat("volume")) * 20);
    }

    private void SetGameTimeUI()
    {
        playSelectUI.SetActive(false);
        settingsUI.SetActive(false);
        titleMenuUI.SetActive(false);
        pauseUI.SetActive(false);
        gameOverUI.SetActive(false);
        gameTimeUI.SetActive(true);
        pauseButton.gameObject.SetActive(true);
    }
    private void SetTitleUI()
    {
        playSelectUI.SetActive(false);
        settingsUI.SetActive(false);
        pauseUI.SetActive(false);
        gameOverUI.SetActive(false);
        gameTimeUI.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        titleMenuUI.SetActive(true);
    }

    private void SetDependencies()
    {
        cameraToggle.isOn = PlayerPrefs.GetInt("isCameraFlipOn") == 0;
        fullScreenToggle.isOn = PlayerPrefs.GetInt("isFullScreen") == 0;
        Screen.fullScreen = fullScreenToggle.isOn;
        resolutions = Screen.resolutions;
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
    }

    private void CreateResolutionOptions()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = (currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }

    public void OnGameFinished(string winner)
    {
        gameOverUI.SetActive(true);
        resultText.text = string.Format("{0} won", winner);
    }

    public void Stalemate()
    {
        gameOverUI.SetActive(true);
        resultText.text = string.Format("Stalemate");
    }
    protected virtual void OnPauseStateChanged()
    {
        PauseStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void TogglePauseVisibility()
    {
        pauseButton.gameObject.SetActive(!pauseButton.gameObject.activeSelf);
    }

    public void HideUI(GameObject hideUI)
    {
        hideUI.SetActive(false);
    }
    public void ShowUI(GameObject showUI)
    {
        showUI.SetActive(true);
    }
    public void BackFromSettings()
    {
        settingsUI.SetActive(false);
        if (gameTimeUI.activeSelf)
            pauseUI.SetActive(true);
        else
            titleMenuUI.SetActive(true);
    }

    public void LoadScene(int index)
    {
        StartCoroutine(FadeOut(index));
    }

    public void OnCameraToggle()
    {
        if (cameraToggle.isOn)
            PlayerPrefs.SetInt("isCameraFlipOn", 0);
        else
            PlayerPrefs.SetInt("isCameraFlipOn", 1);
    }
    public void OnFullScreenToggle()
    {
        if (fullScreenToggle.isOn)
            PlayerPrefs.SetInt("isFullScreen", 0);
        else
            PlayerPrefs.SetInt("isFullScreen", 1);
        Screen.fullScreen = fullScreenToggle.isOn;
    }

    public void SetVolume(float volume)
    {
        mixer.SetFloat("volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("volume", volume);
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ChangePauseState()
    {
        OnPauseStateChanged();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private IEnumerator FadeIn()
	{
		while (fader.alpha != 0f)
		{
            fader.alpha -= Time.deltaTime / fadeTime;
            yield return null;
		}
        fader.gameObject.SetActive(false);
    }
    private IEnumerator FadeOut(int index)
    {
        fader.gameObject.SetActive(true);
        while (fader.alpha != 1f)
        {
            fader.alpha += Time.deltaTime / fadeTime;
            yield return null;
        }
        SceneManager.LoadScene(index);
    }
}
