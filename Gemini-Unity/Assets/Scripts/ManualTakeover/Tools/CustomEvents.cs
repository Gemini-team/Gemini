using UnityEngine;
using UnityEngine.Events;

public class MessageEvent : UnityEvent<string> { }

public class IntEvent : UnityEvent<int> { }

public class MaybeIntEvent : UnityEvent<int?> { }

public class CollisionEvent : UnityEvent<Collision> { }
