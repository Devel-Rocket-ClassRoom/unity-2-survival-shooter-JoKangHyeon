using System.Collections;
using TMPro;
using UnityEngine;
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

    private Coroutine damageEffectCoroutine;

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

        //TODO : PauseMenu Init

        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void DamageEffect()
    {
        if(damageEffectCoroutine != null)
        {
            StopCoroutine(damageEffectCoroutine);
            damageEffectCoroutine = null;
        }

        damageEffectCoroutine = StartCoroutine(CoDamageEffect());
    }

    IEnumerator CoDamageEffect()
    {
        damageEffect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        damageEffect.SetActive(false);
    }

}
