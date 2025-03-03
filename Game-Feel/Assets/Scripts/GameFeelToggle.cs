using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameFeelToggle : MonoBehaviour
{
    public Button hitstopButton;
    public Button screenShakeButton;
    public Button sfxButton;
    public Button flashButton;
    public Button particleFXButton;

    public Color onColor = Color.green;
    public Color offColor = Color.white;

    private bool hitstopEnabled = false;
    private bool screenShakeEnabled = false;
    private bool sfxEnabled = false;
    private bool flashEnabled = false;
    private bool particleFXEnabled = false;

    void Start()
    {
        UpdateButtonVisual(hitstopButton, hitstopEnabled);
        UpdateButtonVisual(screenShakeButton, screenShakeEnabled);
        UpdateButtonVisual(sfxButton, sfxEnabled);
        UpdateButtonVisual(flashButton, flashEnabled);
        UpdateButtonVisual(particleFXButton, particleFXEnabled);

        hitstopButton.onClick.AddListener(() => ToggleFeature(ref hitstopEnabled, hitstopButton));
        screenShakeButton.onClick.AddListener(() => ToggleFeature(ref screenShakeEnabled, screenShakeButton));
        sfxButton.onClick.AddListener(() => ToggleFeature(ref sfxEnabled, sfxButton));
        flashButton.onClick.AddListener(() => ToggleFeature(ref flashEnabled, flashButton));
        particleFXButton.onClick.AddListener(() => ToggleFeature(ref particleFXEnabled, particleFXButton));
    }

    void ToggleFeature(ref bool feature, Button button)
    {
        feature = !feature;
        UpdateButtonVisual(button, feature);
        Debug.Log(button.name + " is now " + (feature ? "ON" : "OFF"));
    }

    void UpdateButtonVisual(Button button, bool isOn)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.color = isOn ? onColor : offColor;
        }
    }

    public bool IsHitstopEnabled() => hitstopEnabled;
    public bool IsScreenShakeEnabled() => screenShakeEnabled;
}
