using UnityEngine;
using TMPro;
using UnityEngine.Windows.Speech;

public class NPC : MonoBehaviour
{
    public TextMeshProUGUI textMesh; // Assign in the Inspector
    public GameObject foodPrefab; // Assign the food prefab in the Inspector
    private int foodSpawnCount = 0;
    private const int maxFoodSpawns = 2; // Maximum food spawns
    private DictationRecognizer dictationRecognizer;

    void Awake()
    {
        // Initialize the DictationRecognizer
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += OnDictationResult;
    }

    void Start()
    {
        // Optionally, disable the DictationRecognizer until the player is within range
        // dictationRecognizer.Start(); // This line is now removed, start is called in OnTriggerEnter
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Ensure DictationRecognizer is not null and not already running before starting it
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
            // Check if the DictationRecognizer is running before attempting to stop it
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
        ProcessRecognizedSpeech(text.ToLower()); // Process the speech
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
            Instantiate(foodPrefab, transform.position + Vector3.forward, Quaternion.identity); // Spawn food
            Respond("Here's some food for you!");
            foodSpawnCount++;
        }
        else if (foodSpawnCount >= maxFoodSpawns)
        {
            Respond("I've run out of food to give.");
        }
        else if (recognizedText.Contains("hello") || recognizedText.Contains("hi"))
        {
            Respond("Hello, adventurer! What brings you to our town?");
        }
        else if (recognizedText.Contains("quest"))
        {
            Respond("Ah, in search of adventure? There's a wolves that's been troubling the nearby villages.");
        }
        else if (recognizedText.Contains("wolves"))
        {
            Respond("Yes, the wolves. They've been trying to take over ");
        }
        else if (recognizedText.Contains("help") || recognizedText.Contains("assist"))
        {
            Respond("I can offer you supplies or information. What do you need to help on your journey?");
        }
        else if (recognizedText.Contains("supplies"))
        {
            Respond("Here, take this healing potion. It should help you on your quest.");
            // add some prefabs 
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







