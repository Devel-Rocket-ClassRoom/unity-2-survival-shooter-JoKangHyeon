using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    public float speed = 5f;
    public float barrelHeight = 0.3f;
    public LayerMask groundMask;

    [Header("Shoot")]
    public ParticleSystem muzzleParticle;
    public AudioClip shotClip;

    public Transform fireTransform;
    public LayerMask shootMask;
    public float fireDistance = 20f;
    public float damage = 10f;
    public float fireRate = 0.5f;

    private Coroutine coShot;
    private LineRenderer bulletLineRenderer;
    private float fireTimer;

    private Animator playerAnimator;
    private AudioSource playerAudioSource;
    private Rigidbody playerRigidbody;


    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        playerAudioSource = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
    }


    void Update()
    {
        if (GameManager.instance.IsGameOver || Time.timeScale == 0)
            return;

        #region Movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        playerAnimator.SetBool("Moving", movement.magnitude > 0.01f);


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }
        #endregion

        #region Shoot
        fireTimer += Time.deltaTime;
        if (Input.GetButton("Fire1"))
        {
            Fire();
        }
        #endregion
    }

    private void FixedUpdate()
    {
        
    }

    private void Fire()
    {
        if (fireTimer < fireRate)
            return;

        fireTimer = 0;

        Vector3 hitPosition = Vector3.zero;
        var ray = new Ray(fireTransform.position, fireTransform.forward);

        if (Physics.Raycast(ray, out var hit, fireDistance, shootMask))
        {
            hitPosition = hit.point;

            var target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
            {
                target.OnDamage(10, hit.point, hit.normal);
            }
        }
        else
        {
            hitPosition = fireTransform.position + fireTransform.forward * fireDistance;
        }

        if (coShot != null)
        {
            StopCoroutine(coShot);
            coShot = null;
        }
        coShot = StartCoroutine(CoShotEffect(hitPosition));
    }

    public IEnumerator CoShotEffect(Vector3 hitPosition)
    {
        muzzleParticle.Play();

        playerAudioSource.PlayOneShot(shotClip);

        bulletLineRenderer.SetPosition(0, fireTransform.position);
        bulletLineRenderer.SetPosition(1, hitPosition);
        bulletLineRenderer.enabled = true;

        yield return new WaitForSeconds(0.03f);

        bulletLineRenderer.enabled = false;
        coShot = null;
    }
}
