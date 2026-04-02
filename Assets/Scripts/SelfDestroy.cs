using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float time = 2f;

    private void OnEnable()
    {
        Destroy(gameObject,time);
    }
}
