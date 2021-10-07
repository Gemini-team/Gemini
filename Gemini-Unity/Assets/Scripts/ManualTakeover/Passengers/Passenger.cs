using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using AdvancedCustomizableSystem;
using UnityEngine.Events;

public class Passenger : MonoBehaviour {
    private const float
        ANIM_SPEED = 0.6f,
        MIN_ANIM_SPEED = 0.25f,
        SPEED_THRESHOLD = 0.15f,
        IDLE_ANIM_SPEED = 3;

    [HideInInspector]
    public UnityEvent OnDestinationReached;
    public NavMeshAgent agent { get; private set; }
    private CharacterCustomization character;

    private bool idle;
    private Quaternion idleRotation = Quaternion.identity;

    public bool ReachedDestination => agent == null || !agent.enabled || 
		(!waitingForDestination && Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance);

	private bool waitingForDestination;
	private static SyncTicket syncTicket = new SyncTicket(0.7f, 1);  // Ticket system shared by all passengers

    public void SetDestinationImmediate(Vector3 destination) {
        if (agent == null) agent = GetComponentInChildren<NavMeshAgent>();

		if (waitingForDestination) {
			Debug.LogWarning("Destination set while already waiting for destination");
		}
		agent.SetDestination(destination);
    }

    public void SetDestinationSynced(Vector3 destination) {
        if (agent == null) agent = GetComponentInChildren<NavMeshAgent>();

		waitingForDestination = true;
        StartCoroutine(syncTicket.TakeTicketAndWait(() => {
			agent.SetDestination(destination);
			waitingForDestination = false;
		}));
    }

    private void Start() {
        agent = GetComponentInChildren<NavMeshAgent>();
        character = GetComponentInChildren<CharacterCustomization>();

        character.Randomize();
        // character.BakeCharacter();
        agent.updateRotation = true;
        agent.updatePosition = true;
    }

    private void Update() {
        float speed = agent.velocity.magnitude;
        bool walk = speed > SPEED_THRESHOLD;
        character.animators.ForEach(x => {
            x.SetBool("walk", walk);
            x.speed = walk ? Mathf.Max(MIN_ANIM_SPEED, ANIM_SPEED * speed) : 1;
        });

        if (!idle && ReachedDestination) {
			idleRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
			OnDestinationReached?.Invoke();
        }
		
        if (idle) {
            transform.rotation = Quaternion.Lerp(transform.rotation, idleRotation, Time.deltaTime * IDLE_ANIM_SPEED);
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
