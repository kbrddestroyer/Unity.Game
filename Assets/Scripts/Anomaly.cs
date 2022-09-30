using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Anomaly : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] protected float attackThreshold;
    [SerializeField, Range(0f, 1f)] protected float eventThreshold;
    [SerializeField] protected bool disappears;
    [SerializeField] protected Collider collider;

    protected bool hunting = false;
    protected bool inEvent = false;
    protected bool can_hunt = true;
    protected bool can_perform_event = true;

    public abstract IEnumerator hunt();

    public abstract void ghostevent();
}
