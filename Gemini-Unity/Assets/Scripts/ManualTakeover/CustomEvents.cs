using UnityEngine;
using UnityEngine.Events;

public class MessageEvent : UnityEvent<string> { }

public class CollisionEvent : UnityEvent<Collision> { }
