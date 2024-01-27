using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class mapGen1 : MonoBehaviour
{
    public GameObject roadPrefab;
    public int iterations = 5;
    private string currentString = "F";
    private float angle = 90;

    void Start()
    {
        StringBuilder sb = new StringBuilder();

        // Apply L-System rules for a number of iterations
        for (int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                sb.Append(ApplyRules(c));
            }

            currentString = sb.ToString();
            sb = new StringBuilder();
        }

        GenerateCityLayout();
    }


    void GenerateCityLayout()
    {
        Vector3 currentPosition = Vector3.zero;
        Vector3 direction = Vector3.forward;
        Quaternion rotation = Quaternion.identity;

        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F':
                    // Instantiate road segment
                    Instantiate(roadPrefab, currentPosition, rotation);
                    // Move forward
                    currentPosition += direction * roadPrefab.transform.localScale.z;
                    break;
                case '+':
                    // Turn right
                    rotation *= Quaternion.Euler(0, angle, 0);
                    direction = rotation * Vector3.forward;
                    break;
                case '-':
                    // Turn left
                    rotation *= Quaternion.Euler(0, -angle, 0);
                    direction = rotation * Vector3.forward;
                    break;
            }
        }
    }

    string ApplyRules(char c)
    {
        switch (c)
        {
            case 'F':
                return "FF-[-F+F+F]+[+F-F-F]";
                // Add more rules as needed
        }
        return c.ToString();
    }
}



