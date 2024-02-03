using UnityEngine;
using UnityEngine.Windows.Speech;

public class PlayerSpeechRecognition : MonoBehaviour
{
    private DictationRecognizer dictationRecognizer;
    public NPC npc; // Reference to the NPC script, assign in the Inspector

    void Start()
    {
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += OnDictationResult;
        dictationRecognizer.Start();
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Recognized Speech: " + text);
        ProcessRecognizedSpeech(text);
    }

    void OnDestroy()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationResult -= OnDictationResult;
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
        }
    }

    private void ProcessRecognizedSpeech(string recognizedText)
    {
        // Convert to lowercase for case-insensitive comparison
        recognizedText = recognizedText.ToLower();

        if (recognizedText.Contains("hello") || recognizedText.Contains("hi"))
        {
            npc.Respond("Greetings, traveler! What brings you to these lands?");
        }
        else if (recognizedText.Contains("quest"))
        {
            npc.Respond("Ah, seeking adventure, are we? I might have a task fit for a brave soul like you.");
        }
        else if (recognizedText.Contains("bye") || recognizedText.Contains("farewell"))
        {
            npc.Respond("Safe travels, adventurer. May the winds be ever in your favor.");
        }
        else if (recognizedText.Contains("help") || recognizedText.Contains("assist"))
        {
            npc.Respond("Of course! What do you need assistance with? Quests, lore, directions?");
        }
        else if (recognizedText.Contains("trade") || recognizedText.Contains("shop"))
        {
            npc.Respond("Looking to trade, eh? Let's see what you have or what you might need.");
        }
        else if (recognizedText.Contains("enemy") || recognizedText.Contains("danger"))
        {
            npc.Respond("Beware, for dangers lurk around every corner. What specifically concerns you?");
        }
        else if (recognizedText.Contains("story") || recognizedText.Contains("lore"))
        {
            npc.Respond("Ah, interested in the tales of old? Which story piques your curiosity?");
        }
        else if (recognizedText.Contains("thank") || recognizedText.Contains("thanks"))
        {
            npc.Respond("You're most welcome! It's always a pleasure to assist a fellow adventurer.");
        }
        else if (recognizedText.Contains("magic") || recognizedText.Contains("spell"))
        {
            npc.Respond("Magic, you say? Are you seeking to learn, or perhaps you're after a particular spell?");
        }
        else if (recognizedText.Contains("weapon") || recognizedText.Contains("armor"))
        {
            npc.Respond("Ah, gearing up for battle? I have some items that might interest you.");
        }
        else
        {
            npc.Respond("Hmm, I'm not quite sure what you're asking for. Could you clarify?");
        }
    }

}






