using System;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class Monster : LivingEntity
{
    public enum MonsterType
    {
        Pink = 0,
        Blue,
        Yellow
    }

    protected enum MonsterState
    {
        Idle,
        Trace,
        Attack,
        Dead,
    }



    [Header("Data")]
    public MonsterData data;
    [SerializeField]
    private MonsterState _currentState;
    public LivingEntity target;
    public float lastAttackTime;
    public LayerMask targetLayer;

    [Header("Objects")]
    public ParticleSystem hitParticle;
    public HitBox hitBox;

    //에디터에 안 보이는 것
    public event Action<Monster> ReturnToPool;

    private AudioSource monsterAudioSource;
    private Animator monsterAnimator;
    private NavMeshAgent agent;
    private Collider monsterCollider;

    protected MonsterState CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
            switch (_currentState)
            {
                case MonsterState.Idle:
                    monsterAnimator.SetBool("HasTarget", false);
                    agent.isStopped = true;
                    break;
                case MonsterState.Trace:
                    monsterAnimator.SetBool("HasTarget", true);
                    agent.isStopped = false;
                    break;
                case MonsterState.Attack:
                    monsterAnimator.SetBool("HasTarget", true);
                    agent.isStopped = true;
                    break;
                case MonsterState.Dead:
                    agent.isStopped = true;
                    monsterCollider.enabled = false;
                    monsterAnimator.SetTrigger("Die");
                    hitBox.enabled = false;
                    break;
            }
        }
    }

    #region Unity Message
    private void Awake()
    {
        monsterAudioSource = GetComponent<AudioSource>();
        monsterAnimator = GetComponent<Animator>();
        monsterCollider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        agent.isStopped = false;
        agent.ResetPath();
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        monsterCollider.enabled = true;
        CurrentState = MonsterState.Idle;
        hitBox.enabled = true;

        monsterAnimator.Rebind();
        monsterAnimator.Update(0f);

        health = data.maxHealth;
        IsDead = false;
    }

    public void Update()
    {
        switch (_currentState)
        {
            case MonsterState.Idle:
                UpdateIdle();
                break;
            case MonsterState.Trace:
                UpdateTrace();
                break;
            case MonsterState.Attack:
                UpdateAttack();
                break;
            case MonsterState.Dead:
                UpdateDead();
                break;
        }
    }
    #endregion

    #region LivingEntity
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        if (!IsDead)
        {
            monsterAudioSource.PlayOneShot(data.damagedClip);

            hitParticle.transform.position = hitPoint;
            hitParticle.transform.forward = hitNormal;
            hitParticle.Play();
        }
    }

    protected override void Die()
    {
        if (IsDead)
            return;

        base.Die();

        CurrentState = MonsterState.Dead;
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    public void OnDieAnimationEnd()
    {
        ReturnToPool?.Invoke(this);
    }
    #endregion

    #region Update State
    private void UpdateDead()
    {
        //Nothing to do 
    }

    private void UpdateAttack()
    {
        if (target == null)
        {
            CurrentState = MonsterState.Trace;
            return;
        }

        if (!hitBox.FindWithTransform(target.transform))
        {
            CurrentState = MonsterState.Trace;
            return;
        }

        var lookAt = target.transform.position;
        lookAt.y = transform.position.y;
        transform.LookAt(target.transform.position);

        if (Time.time > lastAttackTime + data.attackInterval)
        {
            Debug.Log("attack");

            lastAttackTime = Time.time;
            var livingEntity = target.GetComponent<LivingEntity>();
            if (livingEntity != null)
            {
                if (!livingEntity.IsDead)
                {
                    livingEntity.OnDamage(data.damage, transform.position, -transform.forward);
                }
            }
        }
    }

    private void UpdateTrace()
    {
        if (target != null)
        {
            if (hitBox.FindWithTransform(target.transform))
            {
                CurrentState = MonsterState.Attack;
                return;
            }
        }

        if (target == null || Vector3.Distance(target.transform.position, transform.position) > data.traceDistance)
        {
            target = null;
            CurrentState = MonsterState.Idle;
            return;
        }
        agent.SetDestination(target.transform.position);
    }

    private void UpdateIdle()
    {
        if (target != null && Vector3.Distance(target.transform.position, transform.position) < data.traceDistance)
        {
            CurrentState = MonsterState.Trace;
            return;
        }
        target = FindTarget(data.traceDistance);
    }

    private LivingEntity FindTarget(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayer);

        if (colliders.Length == 0)
        {
            return null;
        }
        var target = colliders.
                        OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).
                        First();
        return target.GetComponent<LivingEntity>();
    }
    #endregion
}