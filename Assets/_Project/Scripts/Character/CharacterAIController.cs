
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterManager2D))]
public class CharacterAIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterManager2D character;
    
    [Header("Chase Settings")]
    [SerializeField] private float stoppingDistance = 1f;

    
    [Header("Events")]
    public UnityEvent OnTargetReached;
    public UnityEvent OnTargetLost;
    public UnityEvent<Transform> OnTargetAcquired;
    public UnityEvent OnWaitComplete;
    
    // State
    private AIState currentState = AIState.Idle;
    private Transform currentTarget;
    private Coroutine currentBehavior;
    private bool hasReachedTarget;
    
    public enum AIState
    {
        Idle,
        Chasing,
        Waiting
    }
    
    // Properties
    public AIState CurrentState => currentState;
    public Transform CurrentTarget => currentTarget;
    public bool IsChasing => currentState == AIState.Chasing;
    public bool IsWaiting => currentState == AIState.Waiting;
    public bool HasReachedTarget => hasReachedTarget;
    
    private void Awake()
    {
        if (character == null)
            character = GetComponent<CharacterManager2D>();
    }
    
    public void ChaseTarget(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("Cannot chase null target!");
            return;
        }
        
        StopCurrentBehavior();
        currentTarget = target;
        currentState = AIState.Chasing;
        hasReachedTarget = false;
        currentBehavior = StartCoroutine(ChaseTargetCoroutine());
        OnTargetAcquired?.Invoke(target);
    }
    
    public void Stop()
    {
        StopCurrentBehavior();
        character.SetMoveInput(0);
        currentState = AIState.Idle;
        currentTarget = null;
    }
    
    /// <summary>
    /// Check if within stopping distance of target
    /// </summary>
    public bool IsInRange()
    {
        if (currentTarget == null) return false;
        return Vector2.Distance(transform.position, currentTarget.position) <= stoppingDistance;
    }

    public float GetDistanceToTarget()
    {
        if (currentTarget == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, currentTarget.position);
    }
    
    // ============= COROUTINES =============
    
    private IEnumerator ChaseTargetCoroutine()
    {
        //float updateTimer = 0f;
        
        while (currentTarget != null)
        {
            float distance = Vector2.Distance(transform.position, currentTarget.position);

            float x_distance = Mathf.Abs(transform.position.x - currentTarget.position.x);

            //Debug.Log(x_distance);

            if (distance <= stoppingDistance || x_distance <= stoppingDistance)
            {
                character.SetMoveInput(0);
                hasReachedTarget = true;
                OnTargetReached?.Invoke();
                yield return null;
                continue;
            }
            
            float direction = Mathf.Sign(currentTarget.position.x - transform.position.x);
            character.SetMoveInput(direction);
            
            yield return null;
        }
        
        character.SetMoveInput(0);
        currentState = AIState.Idle;
    }
    
    private void StopCurrentBehavior()
    {
        if (currentBehavior != null)
        {
            StopCoroutine(currentBehavior);
            currentBehavior = null;
        }
        character.SetMoveInput(0);
        Debug.Log("Stop ALL movement");
    }
    
    private void OnDrawGizmosSelected()
    {
        // Stopping distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        
        if (currentTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}