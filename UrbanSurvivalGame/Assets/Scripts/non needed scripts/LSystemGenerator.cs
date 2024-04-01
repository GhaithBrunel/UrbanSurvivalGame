

using System.Collections.Generic;
// dead code gives me compiler errors when i try to delete it 

using UnityEngine;

public class StructuredRoadNetworkGenerator : MonoBehaviour
{
    public GameObject roadPrefab; // Assign the road prefab in Unity Editor

    private string axiom = "F";
    private Dictionary<char, List<string>> rules = new Dictionary<char, List<string>>();
    private float angle = 90f; // A fixed angle for turns to make roads more grid-like
    private int iterations = 10; // Reduce iterations for less complexity

    void Start()
    {
        // Define L-system rules for more structured roads
        rules.Add('F', new List<string> { "FF", "F+F", "F-F" });
        rules.Add('X', new List<string> { "FX", "F-F+X", "F+F-X" });

        GenerateRoadNetwork();
    }

    void GenerateRoadNetwork()
    {
        string currentString = GenerateLSystemPattern();
        Vector3 currentPosition = Vector3.zero;
        Quaternion currentRotation = Quaternion.identity;
        Stack<Vector3> positionStack = new Stack<Vector3>();
        Stack<Quaternion> rotationStack = new Stack<Quaternion>();

        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F':
                    Vector3 startPosition = currentPosition;
                    // Move forward by a distance equal to the prefab's length plus some additional spacing
                    currentPosition += currentRotation * Vector3.forward * (roadPrefab.GetComponent<Renderer>().bounds.size.z + additionalSpacing);
                    PlaceRoadSegment(startPosition, currentRotation);
                    break;
                case '+':
                    currentRotation *= Quaternion.Euler(0, angle, 0);
                    break;
                case '-':
                    currentRotation *= Quaternion.Euler(0, -angle, 0);
                    break;
                case '[':
                    positionStack.Push(currentPosition);
                    rotationStack.Push(currentRotation);
                    break;
                case ']':
                    currentPosition = positionStack.Pop();
                    currentRotation = rotationStack.Pop();
                    break;
            }
        }
    }


    private float additionalSpacing = 1.0f; // Adjust as needed


    void PlaceRoadSegment(Vector3 position, Quaternion rotation)
    {
        Instantiate(roadPrefab, position, rotation);
    }

    string GenerateLSystemPattern()
    {
        string currentString = axiom;
        for (int i = 0; i < iterations; i++)
        {
            string newString = "";
            foreach (char c in currentString)
            {
                newString += rules.ContainsKey(c) ? ChooseStructuredRule(rules[c]) : c.ToString();
            }
            currentString = newString;
        }
        return currentString;
    }

    string ChooseStructuredRule(List<string> ruleSet)
    {
        // Optional: Adjust rule selection logic for more control over road layout
        return ruleSet[Random.Range(0, ruleSet.Count)];
    }
}


























