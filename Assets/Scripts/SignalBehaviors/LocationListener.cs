using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocationListener : SignalBehavior {
    public List<string> moveVerbs;

    public override bool ProcessEvent(Event E) {
        /*
        foreach(KeyValuePair<string, System.Object> arg in E.args) {
            Debug.Log(arg.Key + ": " + arg.Value);
        }
        */
        var kw = (string)E.args["keyword"];
        if ( E.id == "KeywordRecognized" && moveVerbs.Contains( kw ) ){
            Debug.LogWarning("MOVE RECOGNIZED ("+ kw +") NOT IMPLEMENTED!");
            E.doHandle = false;
            return true;
        }

        return false;
    }
}
