using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Location : System.Object {
    public List<string> names;
    public Transform pos;
}

public class CreatureBrain : MonoBehaviour {
    public List<Location> locations = new List<Location>();
    public List<string> verbs = new List<string>(){
        "follow",
        "goto",
    };

    private UnityStandardAssets.Characters.ThirdPerson.AICharacterControl aicc;

    public void Parse(string input){
        var words = input.Split(' ');
        for(int i = 0; i < words.Length; i++) {
            var e = new Event(
                "KeywordRecognized",
                new Dictionary<string, System.Object>() {
                    { "keyword", words[i] },
                    { "fullPhrase", words }
                }
            );
            EventManager.FireEvent(e);

            if( verbs.Contains(words[i]) ) {
                switch (words[i]) {
                    case "follow":
                    case "goto":
                        var target = FindLocation(words[i + 1]);
                        if( target != null && target.pos != null ) {
                            Follow(target.pos);
                        }
                        break;
                    default:
                        Debug.LogWarningFormat("Brain understood a verb it could not complete: {0}.", words[i]);
                        break;
                }
                    
            }
        }
    }

    public Location FindLocation(string name) {
        foreach( Location location in locations ) {
            if( location.names.Contains(name) ) {
                return location;
            }
        }
        return null;
    }

    void Follow(Transform target) {
        aicc.target = target;
    }

	// Use this for initialization
	void Start () {
        aicc = GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
