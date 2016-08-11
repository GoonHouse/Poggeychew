using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Event {
    public string id;
    public Dictionary<string, System.Object> args;
    public bool doHandle = true;

    public Event(string id, Dictionary<string, System.Object> args, bool doHandle = true) {
        this.id = id;
        this.args = args;
        this.doHandle = doHandle;
    }
}

public static class EventManager {
    public delegate bool GenericEvent( Event E );
    //public static event GenericEvent OnFireEvent;

    public static Dictionary<string, System.Action<Event>> events = new Dictionary<string, System.Action<Event>>();

    public static void Fire(Event E){
        events[E.id](E);
    }

    public static void Add(string eventType, System.Action<Event> handler) {
        System.Action<Event> foundEvent;
        events.TryGetValue(eventType, out foundEvent);
        if( foundEvent != null) {
            events[eventType] += handler;
        } else {
            events[eventType] = handler;
        }
    }

    public static void Remove(string eventType, System.Action<Event> handler) {
        events[eventType] -= handler;
    }
}
