using UnityEngine;

public class PlayerCharacter : LivingEntity
{
    [Header("Objects")]
    public UiManager uiManager;
    public ParticleSystem hitParticle;

    [Header("Data")]
    public AudioClip damagedClip;
    public AudioClip deadClip;
    public float maxHealth = 40f;

    private AudioSource playerAudioSource;

    private void Awake()
    {
        health = maxHealth;
        playerAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        uiManager.UpdateHealth(health, maxHealth);
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);

        if (!IsDead)
        {
            playerAudioSource.PlayOneShot(damagedClip);

            hitParticle.transform.position = hitPoint;
            hitParticle.transform.forward = hitNormal;
            hitParticle.Play();

            uiManager.UpdateHealth(health, maxHealth);
            uiManager.ShowDamageEffect();
        }
    }

    protected override void Die()
    {
        if (IsDead)
            return;

        base.Die();
        playerAudioSource.PlayOneShot(deadClip);
        uiManager.ShowGameOver();

        GameManager.instance.GameOver();
    }
}
