using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

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

    public string mModelPath = ".\\";

    private OpenNLP.Tools.SentenceDetect.MaximumEntropySentenceDetector mSentenceDetector;
    private OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer mTokenizer;
    private OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger mPosTagger;
    private OpenNLP.Tools.Chunker.EnglishTreebankChunker mChunker;
    private OpenNLP.Tools.Parser.EnglishTreebankParser mParser;
    private OpenNLP.Tools.NameFind.EnglishNameFinder mNameFinder;
    private OpenNLP.Tools.Lang.English.TreebankLinker mCoreferenceFinder;

    private string[] SplitSentences(string paragraph) {
        if (mSentenceDetector == null) {
            mSentenceDetector = new OpenNLP.Tools.SentenceDetect.EnglishMaximumEntropySentenceDetector(mModelPath + "EnglishSD.nbin");
        }

        return mSentenceDetector.SentenceDetect(paragraph);
    }

    private string[] TokenizeSentence(string sentence) {
        if (mTokenizer == null) {
            mTokenizer = new OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer(mModelPath + "EnglishTok.nbin");
        }

        return mTokenizer.Tokenize(sentence);
    }

    private string[] PosTagTokens(string[] tokens) {
        if (mPosTagger == null) {
            mPosTagger = new OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger(mModelPath + "EnglishPOS.nbin", mModelPath + @"\Parser\tagdict");
        }

        return mPosTagger.Tag(tokens);
    }

    private string ChunkSentence(string[] tokens, string[] tags) {
        if (mChunker == null) {
            mChunker = new OpenNLP.Tools.Chunker.EnglishTreebankChunker(mModelPath + "EnglishChunk.nbin");
        }

        return mChunker.GetChunks(tokens, tags);
    }

    private OpenNLP.Tools.Parser.Parse ParseSentence(string sentence) {
        if (mParser == null) {
            mParser = new OpenNLP.Tools.Parser.EnglishTreebankParser(mModelPath, true, false);
        }

        return mParser.DoParse(sentence);
    }

    private string FindNames(string sentence) {
        if (mNameFinder == null) {
            mNameFinder = new OpenNLP.Tools.NameFind.EnglishNameFinder(mModelPath + "namefind\\");
        }

        string[] models = new string[] { "date", "location", "money", "organization", "percentage", "person", "time" };
        return mNameFinder.GetNames(models, sentence);
    }

    private string FindNames(OpenNLP.Tools.Parser.Parse sentenceParse) {
        if (mNameFinder == null) {
            mNameFinder = new OpenNLP.Tools.NameFind.EnglishNameFinder(mModelPath + "namefind\\");
        }

        string[] models = new string[] { "date", "location", "money", "organization", "percentage", "person", "time" };
        return mNameFinder.GetNames(models, sentenceParse);
    }

    private string IdentifyCoreferents(string[] sentences) {
        if (mCoreferenceFinder == null) {
            mCoreferenceFinder = new OpenNLP.Tools.Lang.English.TreebankLinker(mModelPath + "coref");
        }

        System.Collections.Generic.List<OpenNLP.Tools.Parser.Parse> parsedSentences = new System.Collections.Generic.List<OpenNLP.Tools.Parser.Parse>();

        foreach (string sentence in sentences) {
            OpenNLP.Tools.Parser.Parse sentenceParse = ParseSentence(sentence);
            string findNames = FindNames(sentenceParse);
            parsedSentences.Add(sentenceParse);
        }
        return mCoreferenceFinder.GetCoreferenceParse(parsedSentences.ToArray());
    }

    public void AdvancedParse(string input) {
        var tokens = TokenizeSentence(input);
        Debug.Log("TOKENS: " + string.Join(",", tokens));
        var tags = PosTagTokens(tokens);
        Debug.Log("TAGS: " + string.Join(",", tags));
        var chunks = ChunkSentence(tokens, tags);
        Debug.Log("CHUNKS: " + chunks);
        /*
        if ( g != null ) {
            var w = g.GetChildren();
            
        }
        */
    }

    public void Parse(string input){
        AdvancedParse(input);
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
