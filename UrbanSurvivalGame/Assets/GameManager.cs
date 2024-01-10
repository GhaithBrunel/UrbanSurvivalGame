using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private Inventory inventory;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewGame()
    {
        StartCoroutine(LoadGameScene("DemoScene", true));
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameScene("DemoScene", false));
    }

    private IEnumerator LoadGameScene(string sceneName, bool isNewGame)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // After loading the scene, find the Inventory instance
        inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            if (isNewGame)
            {
                inventory.clearInventory();
            }
            else
            {
                inventory.loadInventory();
            }
        }
        else
        {
            Debug.LogError("Inventory not found in the loaded scene.");
        }
    }
}



