using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Linq;

public class UINavigator : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] GameObject titleMenuUI;
    [SerializeField] GameObject playSelectUI;
    [SerializeField] GameObject connectUI;
    [SerializeField] GameObject teamSelectUI;
    [SerializeField] GameObject gameTimeUI;
    [SerializeField] GameObject pauseUI;
    [SerializeField] GameObject settingsUI;
    [SerializeField] GameObject gameOverUI;

    [Header("Text Fields")]
    [SerializeField] private Text resultText;
    [SerializeField] private Text connectionText;

    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button whiteTeamButton;
    [SerializeField] private Button blackTeamButton;

    [Header("Sliders/Toggles/Dropdowns")]
    [SerializeField] private Toggle cameraToggle;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Dropdown chessLevel;

    [Header("Audio")]
    [SerializeField] private AudioMixer mixer;

    Resolution[] resolutions;

    private NetworkManager networkManager;

    public delegate void PauseStateEventHandler(object source, EventArgs args);

    public event PauseStateEventHandler PauseStateChanged;

    private void Awake()
    {
        SetDependencies();
    }
    private void SetDependencies()
    {
        cameraToggle.isOn = PlayerPrefs.GetInt("isCameraFlipOn") == 0;
        fullScreenToggle.isOn = PlayerPrefs.GetInt("isFullScreen") == 0;
        Screen.fullScreen = fullScreenToggle.isOn;
        resolutions = Screen.resolutions;
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
    }

    private void Start()
	{
		SetUIForScene(SceneManager.GetActiveScene().buildIndex);

		CreateResolutionOptions();
		mixer.SetFloat("volume", Mathf.Log10(PlayerPrefs.GetFloat("volume")) * 20);
	}


	private void SetUIForScene(int index)
	{
		if (index == 0)
			SetTitleUI();
		else if (index == 1)
			SetGameTimeUI();
		else if (index == 2)
			SetMultiPlayerStartUI();
	}

	public void SetGameTimeUI()
    {
        SetAllUIInactive();
        gameTimeUI.SetActive(true);
        pauseButton.gameObject.SetActive(true);
    }


	private void SetMultiPlayerStartUI()
    {
        SetAllUIInactive();
        connectUI.SetActive(true);


        networkManager = FindObjectOfType<NetworkManager>().GetComponent<NetworkManager>();
        networkManager.SetUIDependencies(this);
        chessLevel.AddOptions(Enum.GetNames(typeof(ChessLevel)).ToList());
    }

    private void SetTitleUI()
    {
        SetAllUIInactive();
        titleMenuUI.SetActive(true);
    }


    private void SetAllUIInactive()
	{
        playSelectUI.SetActive(false);
        settingsUI.SetActive(false);
        connectUI.SetActive(false);
        teamSelectUI.SetActive(false);
        pauseUI.SetActive(false);
        gameOverUI.SetActive(false);
        gameTimeUI.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        titleMenuUI.SetActive(false);
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


    // Used by buttons to set a UI to InActive
    public void HideUI(GameObject hideUI)
    {
        hideUI.SetActive(false);
    }

    // Used by buttons to set a UI to Active
    public void ShowUI(GameObject showUI)
    {
        showUI.SetActive(true);
    }


    // Used by buttons to load a scene
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    // Shows or Hides the Pause Button
    public void TogglePauseVisibility()
    {
        pauseButton.gameObject.SetActive(!pauseButton.gameObject.activeSelf);
    }

    // Used by Pause and Resume Buttons
    public void ChangePauseState()
    {
        OnPauseStateChanged();
    }

    // Used by Quit Button
    public void Quit()
    {
        Application.Quit();
    }

                    /* Multiplayer */

    // Used by Connect Button
    public void OnConnect()
	{
        networkManager.SetPlayerLevel((ChessLevel)chessLevel.value);
        networkManager.Connect();
	}

    public void SetConnectionText(string message)
	{
        connectionText.text = message;
    }
	public void SetJoinedRoomUI()
	{
        SetAllUIInactive();
        teamSelectUI.SetActive(true);
	}

    public void SelectTeam(int team)
	{
        networkManager.SelectTeam(team);
	}

	public void RestrictTeamChoice(TeamColor occupiedTeam)
	{
        var buttonToDeactivate = occupiedTeam == TeamColor.White ? whiteTeamButton : blackTeamButton;
        buttonToDeactivate.interactable = false;
	}



                     /* Settings */

    // Used by buttons to return from a Settings Screen
    public void BackFromSettings()
    {
        settingsUI.SetActive(false);
        if (gameTimeUI.activeSelf)
            pauseUI.SetActive(true);
        else
            titleMenuUI.SetActive(true);
    }

    // Used By Camera Toggle
    public void OnCameraToggle()
    {
        if (cameraToggle.isOn)
            PlayerPrefs.SetInt("isCameraFlipOn", 0);
        else
            PlayerPrefs.SetInt("isCameraFlipOn", 1);
    }

    // Used by FullScreen Toggle
    public void OnFullScreenToggle()
    {
        if (fullScreenToggle.isOn)
            PlayerPrefs.SetInt("isFullScreen", 0);
        else
            PlayerPrefs.SetInt("isFullScreen", 1);
        Screen.fullScreen = fullScreenToggle.isOn;
    }

    // Used by Volume Slider
    public void SetVolume(float volume)
    {
        mixer.SetFloat("volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("volume", volume);
    }


    // Used by Resolution Dropdown
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
