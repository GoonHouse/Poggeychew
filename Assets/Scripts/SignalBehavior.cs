using UnityEngine;
using System.Collections;

public class SignalBehavior : MonoBehaviour {

    public virtual void OnEnable() {
        EventManager.OnFireEvent += FireEvent;
    }

    public virtual void OnDisable() {
        EventManager.OnFireEvent -= FireEvent;
    }

    public virtual bool FireEvent(Event E) {
        if( E.doHandle ) {
            return ProcessEvent(E);
        }

        return false;
    }

    public virtual bool ProcessEvent(Event E){
        return false;
    }
}