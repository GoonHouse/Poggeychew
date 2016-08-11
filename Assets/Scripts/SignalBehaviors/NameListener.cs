using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NameListener : SignalBehavior {
    public string myName;

    public override void Awake() {
        subscribedEvents = new Dictionary<string, System.Action<Event>>(){
            { "BrainParse", E => BrainParse(E) }
        };
    }

    public bool BrainParse(Event E) {
        var tokens = (string[])E.args["tokens"];
        var tlist = new List<string>(tokens);
        if (E.doHandle && tlist.Contains(myName)) {
            Debug.Log("I heard my name (" + myName + ") , what do you want?!");
            E.doHandle = false;
            return true;
        }

        return false;
    }
}
