using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Game UI Objects")]
    public TextMeshProUGUI scoreText;
    public Slider healthSlider;
    public GameObject gameoverBlocker;
    public GameObject damageEffect;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public Slider musicVolumeSlider;
    public Slider effectVolumeSlider;
    public Toggle muteToggle;

    [Header("Data")]
    public AudioMixer audioMixer;

    private Coroutine damageEffectCoroutine;
    private WaitForSeconds damageEffectWait = new WaitForSeconds(0.1f);
    private Animator uiAnimator;
    private bool isPaused = false;

    const string k_PrefsMusicVolumeKey = "MusicVolume";
    const string k_PrefsEffectVolumeKey = "EffectVolume";
    const string k_PrefsMuteKey = "Mute";

    const string k_AudioMixerMusicParameter = "MusicVolume";
    const string k_AudioMixerEffectParameter = "EffectVolume";

    void Awake()
    {
        uiAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat(k_PrefsMusicVolumeKey, 1f);
        effectVolumeSlider.value = PlayerPrefs.GetFloat(k_PrefsEffectVolumeKey, 1f);
        muteToggle.isOn = PlayerPrefs.GetInt(k_PrefsMuteKey, 1) == 1;
        ApplyVolume();
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth / maxHealth;
    }

    public void Pause()
    {
        Time.timeScale = 0f;

        musicVolumeSlider.value = PlayerPrefs.GetFloat(k_PrefsMusicVolumeKey, 1f);
        effectVolumeSlider.value = PlayerPrefs.GetFloat(k_PrefsEffectVolumeKey, 1f);
        muteToggle.isOn = PlayerPrefs.GetInt(k_PrefsMuteKey, 1) == 1;

        pauseMenu.SetActive(true);
        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;

        PlayerPrefs.SetFloat(k_PrefsMusicVolumeKey, musicVolumeSlider.value);
        PlayerPrefs.SetFloat(k_PrefsEffectVolumeKey, effectVolumeSlider.value);
        PlayerPrefs.SetInt(k_PrefsMuteKey, muteToggle.isOn ? 1 : 0);

        pauseMenu.SetActive(false);
        isPaused = false;
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void ShowDamageEffect()
    {
        if (damageEffectCoroutine != null)
        {
            StopCoroutine(damageEffectCoroutine);
            damageEffectCoroutine = null;
        }

        damageEffectCoroutine = StartCoroutine(CoDamageEffect());
    }

    IEnumerator CoDamageEffect()
    {
        damageEffect.SetActive(true);
        yield return damageEffectWait;
        damageEffect.SetActive(false);
        damageEffectCoroutine = null;
    }

    public void ShowGameOver()
    {
        uiAnimator.SetTrigger("GameOver");
    }

    public void OnGameOverAnimationEnd()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnMusicVolumeSliderChanged(float value)
    {
        ApplyVolume();
    }

    public void OnEffectVolumeSliderChanged(float value)
    {
        ApplyVolume();
    }

    public void OnMuteToggleChanged(bool soundOn)
    {
        ApplyVolume();
    }

    public void ApplyVolume()
    {
        if (muteToggle.isOn)
        {
            float dBValue = Mathf.Log10(effectVolumeSlider.value) * 20;
            audioMixer.SetFloat(k_AudioMixerEffectParameter, dBValue);

            dBValue = Mathf.Log10(musicVolumeSlider.value) * 20;
            audioMixer.SetFloat(k_AudioMixerMusicParameter, dBValue);
        }
        else
        {
            audioMixer.SetFloat(k_AudioMixerMusicParameter, -80f);
            audioMixer.SetFloat(k_AudioMixerEffectParameter, -80f);
        }
    }
}
