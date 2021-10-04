using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Scrips.World;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TitleMenu : MonoBehaviour
{

    public GameObject mainMenuObject;
    public GameObject settingsObject;

    [Header("Main Menu UI Elements")]
    public TextMeshProUGUI seedField;

    [Header("Settings Menu UI Elements")]
    public Slider viewDstSlider;
    public TextMeshProUGUI viewDstText;
    public Slider mouseSlider;
    public TextMeshProUGUI mouseTxtSlider;
    public Toggle threadingToggle;


    Settings settings;

    private void Awake() {
        
        if (!File.Exists(Application.dataPath + "/settings.cfg")) {

            Debug.Log("No settings file found, creating new one.");

            settings = new Settings();
            string jsonExport = JsonUtility.ToJson(settings);
            File.WriteAllText(Application.dataPath + "/settings.cfg", jsonExport);

        } else {

            Debug.Log("Settings file found, loading settings.");

            string jsonImport = File.ReadAllText(Application.dataPath + "/settings.cfg");
            settings = JsonUtility.FromJson<Settings>(jsonImport);

        }

    }

    public void StartGame() {

        VoxelData.seed = Mathf.Abs(seedField.text.GetHashCode()) / VoxelData.WorldSizeInChunks;
        SceneManager.LoadScene("main", LoadSceneMode.Single);

    }

    public void EnterSettings() {

        viewDstSlider.value = settings.renderDistance;
        UpdateViewDstSlider();
        mouseSlider.value = settings.cameraSesitivity;
        UpdateMouseSlider();
        threadingToggle.isOn = settings.enableThreading;

        mainMenuObject.SetActive(false);
        settingsObject.SetActive(true);

    }

    public void LeaveSettings () {

        settings.renderDistance = (int)viewDstSlider.value;
        settings.cameraSesitivity = mouseSlider.value;
        settings.enableThreading = threadingToggle.isOn;

        string jsonExport = JsonUtility.ToJson(settings);
        File.WriteAllText(Application.dataPath + "/settings.cfg", jsonExport);

        mainMenuObject.SetActive(true);
        settingsObject.SetActive(false);

    }

    public void QuitGame() {

        Application.Quit();

    }

    public void UpdateViewDstSlider () {
        viewDstText.text = "View Distance: " + viewDstSlider.value;
    }

    public void UpdateMouseSlider () {

        mouseTxtSlider.text = "Mouse Sensitivity: " + mouseSlider.value.ToString("F1");
    }
}
