using UnityEngine;
using System.Collections;

public class InputSubterfuge : MonoBehaviour {
    public CreatureBrain cb;
    public UnityEngine.UI.Text resultText;
    public UnityEngine.UI.InputField hypothesisText;

    public void SetResult(string text) {
        resultText.text = text;
        SetHypothesis("");
        cb.Parse(text);
    }

    public void SetHypothesis(string text) {
        hypothesisText.text = text;
    }

    public void SubmitHypothesis() {
        SetResult(hypothesisText.text);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
