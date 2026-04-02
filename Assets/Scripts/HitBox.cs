using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour, IEnumerable<Collider>, IEnumerable
{
    private List<Collider> colliders = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (!colliders.Contains(other))
        {
            colliders.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }

    private void OnDisable()
    {
        colliders.Clear();
    }    

    public IEnumerator<Collider> GetEnumerator()
    {
        return ((IEnumerable<Collider>)colliders).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)colliders).GetEnumerator();
    }

    public bool FindWithTransform(Transform t)
    {
        var target = colliders.Find(c => c != null && c.transform == t);
        return target != null;
    }

    public void Clear()
    {
        colliders.Clear();
    }
}
