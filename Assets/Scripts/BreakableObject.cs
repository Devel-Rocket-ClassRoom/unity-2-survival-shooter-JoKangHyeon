using UnityEngine;
using UnityEngine.Audio;

public class BreakableObject : LivingEntity
{
    public GameObject breakResult;
    public float maxHealth = 10;
    public ParticleSystem hitParticle;

    public AudioClip damageClip;

    public AudioClip breakAudio;
    public AudioMixerGroup breakAudioMixerGroup;

    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);

        if (hitParticle != null)
        {
            hitParticle.transform.position = hitPoint;
            hitParticle.transform.forward = hitNormal;
            hitParticle.Play();
        }

        if(damageClip != null) 
            audioSource?.PlayOneShot(damageClip);
    }

    protected override void Die()
    {
        base.Die();
        var spawnPos = transform.position;
        spawnPos.y += 1f;
        Instantiate(breakResult, spawnPos, Quaternion.identity);

        if (breakAudio != null)
        {
            GameObject breakAudioGameObject = new GameObject();
            breakAudioGameObject.transform.position = transform.position;
            AudioSource breakAudioSource = breakAudioGameObject.AddComponent<AudioSource>();
            breakAudioSource.outputAudioMixerGroup = breakAudioMixerGroup;

            breakAudioSource.PlayOneShot(breakAudio);
            Destroy(breakAudioGameObject, breakAudio.length);
        }

        Destroy(gameObject);
    }
}
