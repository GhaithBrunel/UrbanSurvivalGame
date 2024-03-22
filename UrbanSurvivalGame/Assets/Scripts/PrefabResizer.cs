using UnityEngine;


public class PrefabResizer : MonoBehaviour
{
    public GameObject prefab; // Assign your prefab here
    public Vector3 standardSize = new Vector3(1, 1, 1); // Set your standard size here

    void Start()
    {
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        instance.transform.localScale = standardSize;
    }
}
