using UnityEngine;

public abstract class LivingEntity: MonoBehaviour, IDamageable
{
    protected float health = 100f;
    public bool IsDead { get; protected set; }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        IsDead = true;
    }
}