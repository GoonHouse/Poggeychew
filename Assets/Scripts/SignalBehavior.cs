using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignalBehavior : MonoBehaviour {
    public Dictionary<string, System.Action<Event>> subscribedEvents;

    public virtual void Awake() {
        subscribedEvents = new Dictionary<string, System.Action<Event>>(){
            //{ "Honk", E => FireEvent(E) }
        };
    }

    public virtual void OnEnable() {
        foreach(KeyValuePair<string, System.Action<Event>> subEvent in subscribedEvents) {
            EventManager.Add(subEvent.Key, subEvent.Value);
        }
    }

    public virtual void OnDisable() {
        foreach (KeyValuePair<string, System.Action<Event>> subEvent in subscribedEvents) {
            EventManager.Remove(subEvent.Key, subEvent.Value);
        }
    }
}