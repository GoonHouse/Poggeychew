using UnityEngine;
using System.Collections;
using UnityEngine.Windows.Speech;

public class SpeechDictation : MonoBehaviour {
    public string hypothesis;
    public string recognition;

    public InputSubterfuge isub;

    private DictationRecognizer m_DictationRecognizer;

    void Start() {
        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (text, confidence) => {
            isub.SetResult(text);
        };

        m_DictationRecognizer.DictationHypothesis += (text) => {
            isub.SetHypothesis(text);
        };

        m_DictationRecognizer.DictationComplete += (completionCause) => {
            if (completionCause != DictationCompletionCause.Complete) {
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
            }
        };

        m_DictationRecognizer.DictationError += (error, hresult) => {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };

        m_DictationRecognizer.Start();
    }
}