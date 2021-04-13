using UnityEngine;
using UnityEngine.AI;
using AdvancedCustomizableSystem;
using UnityEngine.Events;

public class Passenger : MonoBehaviour {
    private const float 
        ANIM_SPEED = 0.6f,
        MIN_ANIM_SPEED = 0.25f,
        SPEED_THRESHOLD = 0.15f;

    [HideInInspector]
    public UnityEvent OnDestinationReached;
    public NavMeshAgent agent { get; private set; }
    private CharacterCustomization character;

    private bool idle;

    public bool ReachedDestination => agent == null || (agent.enabled && agent.remainingDistance <= agent.stoppingDistance);

    private void Start() {
        agent = GetComponentInChildren<NavMeshAgent>();
        character = GetComponentInChildren<CharacterCustomization>();

        character.Randomize();
        // character.BakeCharacter();
        agent.updateRotation = true;
        agent.updatePosition = true;
    }

    public void SetDestination(Vector3 destination) {
        if (agent == null) agent = GetComponentInChildren<NavMeshAgent>();
        agent.SetDestination(destination);
    }

    private void Update() {
        float speed = agent.velocity.magnitude;
        bool walk = speed > SPEED_THRESHOLD;
        character.animators.ForEach(x => {
            x.SetBool("walk", walk);
            x.speed = walk ? Mathf.Max(MIN_ANIM_SPEED, ANIM_SPEED * speed) : 1;
        });

        if (!idle && ReachedDestination) {
            OnDestinationReached?.Invoke();
        }
    }

    private void LateUpdate() {
        idle = ReachedDestination;
    }

    void OnDrawGizmos() {
        if (agent == null) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(agent.destination, 0.25f);
    }
}
