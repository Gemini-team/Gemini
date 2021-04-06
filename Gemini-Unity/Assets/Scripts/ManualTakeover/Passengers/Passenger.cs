using UnityEngine;
using UnityEngine.AI;
using AdvancedCustomizableSystem;

public class Passenger : MonoBehaviour {
    private const float 
        ANIM_SPEED = 0.75f,
        MIN_ANIM_SPEED = 0.25f,
        SPEED_THRESHOLD = 0.15f;

    public NavMeshAgent agent { get; private set; }
    private CharacterCustomization character;

    private float waitUntil;

    public bool IsBusy => agent == null || !agent.enabled || waitUntil > Time.time;
    public bool ReachedDestination => agent == null || (agent.enabled && agent.remainingDistance <= agent.stoppingDistance);

    private void Start() {
        agent = GetComponentInChildren<NavMeshAgent>();
        character = GetComponentInChildren<CharacterCustomization>();

        character.Randomize();
        // character.BakeCharacter();
        agent.updateRotation = true;
        agent.updatePosition = true;
    }

    public void SetDestination(Vector3 destination, float waitTime = 0) {
        if (agent == null) return;

        waitUntil = Time.time + waitTime;
        agent.SetDestination(destination);
    }

    public void MoveToDestination() {
        transform.position = agent.destination;
    }

    private void Update() {
        float speed = agent.velocity.magnitude;
        bool walk = speed > SPEED_THRESHOLD;
        character.animators.ForEach(x => {
            x.SetBool("walk", walk);
            x.speed = walk ? Mathf.Max(MIN_ANIM_SPEED, ANIM_SPEED * speed) : 1;
        });
    }

    void OnDrawGizmosSelected() {
        if (agent == null) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, agent.steeringTarget);
        Gizmos.DrawSphere(agent.destination, 0.25f);
    }
}
