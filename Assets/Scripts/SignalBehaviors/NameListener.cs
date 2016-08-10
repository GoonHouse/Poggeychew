using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NameListener : SignalBehavior {
    public string myName;

    public override bool ProcessEvent(Event E) {
        /*
        foreach(KeyValuePair<string, System.Object> arg in E.args) {
            Debug.Log(arg.Key + ": " + arg.Value);
        }
        */
        if ( E.id == "KeywordRecognized" && ( (string)E.args["keyword"] == myName ) ) {
            Debug.Log("I heard my name ("+myName+") , what do you want?!");
            E.doHandle = false;
            return true;
        }

        return false;
    }
}
