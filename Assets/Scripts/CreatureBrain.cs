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

    private UnityStandardAssets.Characters.ThirdPerson.AICharacterControl aicc;

    #region OpenNLP_Helpers
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

    private OpenNLP.Tools.Util.Span[] PosTokenizeSentence(string sentence) {
        if (mTokenizer == null) {
            mTokenizer = new OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer(mModelPath + "EnglishTok.nbin");
        }

        return mTokenizer.TokenizePositions(sentence);
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

    private string[] ExChunkSentence(OpenNLP.Tools.Util.Span[] tokens, string[] tags) {
        if (mChunker == null) {
            mChunker = new OpenNLP.Tools.Chunker.EnglishTreebankChunker(mModelPath + "EnglishChunk.nbin");
        }

        return mChunker.Chunk(tokens, tags);
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
    #endregion OpenNLP_Helpers

    private static string UppercaseFirst(string s) {
        if (string.IsNullOrEmpty(s)) {
            return string.Empty;
        }
        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }

    public void Parse(string input){
        var tokens = TokenizeSentence(input);
        var tags = PosTagTokens(tokens);
        var posTokens = PosTokenizeSentence(input);
        var chunks = ChunkSentence(tokens, tags);
        var exchunks = ExChunkSentence(posTokens, tags);
        Debug.Log("CHUNKS: " + chunks);
        foreach (string exchunk in exchunks) {
            Debug.Log("EXCHUNK: " + exchunk);
        }

        var e = new Event(
            "BrainParse",
            new Dictionary<string, System.Object>() {
                { "tokens", tokens },
                { "tags", tags },
                { "chunks", chunks },
                { "fullPhrase", input }
            }
        );
        EventManager.Fire(e);
    }

    public Location FindLocation(string name) {
        foreach (Location location in locations) {
            if (location.names.Contains(name)) {
                return location;
            }
        }
        return null;
    }

    public Location FindLocation(List<string> names) {
        foreach( string name in names) {
            var ploc = FindLocation(name);
            if( ploc != null) {
                return ploc;
            }
        }
        return null;
    }

    public void Follow(Transform target) {
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
