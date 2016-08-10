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
    public delegate bool FiredEvent( Event E );
    public static event FiredEvent OnFireEvent;

    public static void FireEvent(Event E){
        OnFireEvent(E);
    }
}
