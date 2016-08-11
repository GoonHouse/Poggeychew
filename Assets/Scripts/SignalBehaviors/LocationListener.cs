using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CreatureBrain))]
public class LocationListener : SignalBehavior {
    public List<string> moveVerbs;
    private CreatureBrain cb;

    void Start() {
        cb = GetComponent<CreatureBrain>();
    }

    public override void Awake() {
        subscribedEvents = new Dictionary<string, System.Action<Event>>(){
            { "BrainParse", E => BrainParse(E) }
        };
    }

    public bool BrainParse(Event E) {
        var tokens = (string[])E.args["tokens"];
        var tlist = new List<string>(tokens);
        if (E.doHandle) {
            int i = 0;
            foreach(string moveVerb in moveVerbs) {
                if( tlist.Contains(moveVerb)) {
                    var remainingTokens = tlist.GetRange(i, tlist.Count - i);
                    var target = cb.FindLocation(remainingTokens);
                    if (target != null && target.pos != null) {
                        cb.Follow(target.pos);
                        E.doHandle = false;
                        return true;
                    }
                }
                i++;
            }
        }

        return false;
    }
}
