using UnityEngine;
using TMPro;
using UnityEngine.Windows.Speech;

public class NPC : MonoBehaviour
{
    public TextMeshProUGUI textMesh; 
    public GameObject foodPrefab; 
    private int foodSpawnCount = 0;
    private const int maxFoodSpawns = 4; 
    private DictationRecognizer dictationRecognizer;
    public GameObject bowPrefab;

    void Awake()
    {
        
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += OnDictationResult;
    }

    void Start()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // if the player entres the talking zone it turns on the speach recgonzier
        {
            if (dictationRecognizer != null && dictationRecognizer.Status != SpeechSystemStatus.Running)
            {
                dictationRecognizer.Start();
                Debug.Log("Voice recognition activated.");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            if (dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
                Debug.Log("Voice recognition deactivated.");
            }
        }
    }

    void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log($"Recognized Speech: {text}");
        ProcessRecognizedSpeech(text.ToLower()); 
    }

    public void Respond(string message)
    {
        if (textMesh != null)
        {
            textMesh.text = message;
        }
        else
        {
            Debug.LogWarning("TextMeshPro component not found on NPC.");
        }
    }

    private void ProcessRecognizedSpeech(string recognizedText)
    {
        if (recognizedText.Contains("i'm hungry") && foodSpawnCount < maxFoodSpawns)
        {
            Instantiate(foodPrefab, transform.position + Vector3.forward+ Vector3.up, Quaternion.identity); // Spawn food
            Respond("Here's some food for you!");
            foodSpawnCount++;
        }
        else if (foodSpawnCount >= maxFoodSpawns)
        {
            Respond("I've run out of food to give.");
        }
        else if (recognizedText.Contains("hello") || recognizedText.Contains("hi"))
        {
            Respond("Hello, adventurer! What brings you to our town? Are you here to complete quests ");
        }
        else if (recognizedText.Contains("quest"))
        {
            Respond("Ah, in search of adventure? There's a wolves that's been troubling the nearby villages. Would you like to hear abour the wolves");
        }
        else if (recognizedText.Contains("wolves"))
        {
            Respond("Yes, the wolves. They've been trying to take over the city they've been killing our people. I'll help you take them down ");
        }
        else if (recognizedText.Contains("help") || recognizedText.Contains("assist"))
        {
            Respond("I can offer you supplies or information. What do you need to help on your journey?");
        }
        else if (recognizedText.Contains("supplies"))
        {
            Respond("Here, take this healing potion. It should help you on your quest.");
            // add prefabs of potion /bandage
        }
        else if (recognizedText.Contains("information"))
        {
            Respond("The Wofls is said to be weak to physical damage. Perhaps that will aid you in your battle.");
        }
        else if (recognizedText.Contains("thank"))
        {
            Respond("You're welcome! Safe travels, and may fortune favor you.");
        }
        else if (recognizedText.Contains("goodbye"))
        {
            Respond("Farewell, brave adventurer. Return to us safely.");
        }
        else
        {
            Respond("Hmm, I'm not quite sure what you're asking for. Could you clarify?");
        }

        if (recognizedText.Contains("i need a weapon"))
        {
            if (bowPrefab != null)
            {
                Instantiate(bowPrefab, transform.position + Vector3.forward * 2 + Vector3.up, Quaternion.identity); // Spawn the bow
                Respond("Here's a bow for you. Use it wisely!");
            }
            else
            {
                Respond("I'm sorry, but I don't have any weapons to give.");
            }
        }



    }


    void OnDestroy()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationResult -= OnDictationResult;
            dictationRecognizer.Dispose();
        }
    }
}







