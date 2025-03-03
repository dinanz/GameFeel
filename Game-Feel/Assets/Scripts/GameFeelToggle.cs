using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

    //camera shake
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private float shakeAmount = 0.1f;
    private float shakeDuration = 0.5f;

    //screen flash
    private Texture2D overlayTexture;
    private float alpha = 0;
    private bool isFlashing = false;

    void Start()
    {
        //camera shake
        mainCamera = Camera.main;
        originalCameraPosition = mainCamera.transform.position;

        //screen flash
        overlayTexture = new Texture2D(1, 1);
        overlayTexture.SetPixel(0, 0, Color.red);
        overlayTexture.Apply();

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

//ONLY FOR TESTING DO NOT TURN ON FOR ACTUAL GAME
        // if (button == screenShakeButton && screenShakeEnabled)
        // {
        //     ShakeCamera();
        // }

        // if (button == flashButton && flashEnabled)
        // {
        //     FlashScreenRed();
        // }
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
    public bool IsFlashEnabled() => flashEnabled;
    public bool IsParticleEnabled() => particleFXEnabled;
    public bool IsSfxEnabled() => sfxEnabled;

    public void ShakeCamera() //called in player script
    {
        if (screenShakeEnabled)
        {
            StartCoroutine(ShakeCameraCoroutine());
        }
    }

    private IEnumerator ShakeCameraCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float shakeX = Random.Range(-shakeAmount, shakeAmount);
            float shakeY = Random.Range(-shakeAmount, shakeAmount);
            mainCamera.transform.position = new Vector3(originalCameraPosition.x + shakeX, originalCameraPosition.y + shakeY, originalCameraPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.position = originalCameraPosition;
    }

    public void FlashScreenRed(float duration = 0.2f, float fadeSpeed = 0.5f) //called in player script
    {
        if (flashEnabled && !isFlashing) 
        {
            StartCoroutine(FlashEffect(duration, fadeSpeed));
        }
    }

    private IEnumerator FlashEffect(float duration, float fadeSpeed)
    {
        isFlashing = true;
        alpha = 0.5f;

        yield return new WaitForSeconds(duration);

        float elapsedTime = 0;
        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            alpha = Mathf.Lerp(0.5f, 0, elapsedTime / fadeSpeed);
            yield return null;
        }

        alpha = 0;
        isFlashing = false;
    }
    private void OnGUI()
    {
        if (alpha > 0)
        {
            GUI.color = new Color(1, 0, 0, alpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), overlayTexture);
        }
    }
}
