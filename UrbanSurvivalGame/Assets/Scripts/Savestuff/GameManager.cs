using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private string saveFileName;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        saveFileName = Application.persistentDataPath + "/gameSave.json";
    }

    public void StartNewGame()
    {
        int randomSeed = Random.Range(0, int.MaxValue);
        StartCoroutine(LoadGameScene("DemoScene", true, randomSeed));
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameScene("DemoScene", false, GetSavedSeed()));
    }

    private int GetSavedSeed()
    {
        if (File.Exists(saveFileName))
        {
            string jsonData = File.ReadAllText(saveFileName);
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonData);
            return loadedData.seed;
        }
        Debug.LogError("Save file not found. Using default seed.");
        return 0; // Default seed if no save file is found
    }

    private IEnumerator LoadGameScene(string sceneName, bool isNewGame, int seed)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => asyncLoad.isDone);

        MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
        Inventory inventory = FindObjectOfType<Inventory>();
        PrefabSpawner prefabSpawner = FindObjectOfType<PrefabSpawner>();

        if (mapGenerator != null && inventory != null && prefabSpawner != null)
        {
            if (isNewGame)
            {
                mapGenerator.RegenerateMap(mapGenerator.mapWidth, mapGenerator.mapHeight, seed);
                inventory.clearInventory();
                prefabSpawner.SpawnPrefabsForNewGame();
            }
            else
            {
                LoadGameState();
            }
        }
        else
        {
            Debug.LogError("Essential components not found in the loaded scene.");
        }
    }

    public void SaveGameWrapper()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        Inventory inventory = FindObjectOfType<Inventory>();
        MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();

        if (playerTransform != null && inventory != null && mapGenerator != null)
        {
            SaveGameState(playerTransform, mapGenerator.seed);
            inventory.SaveInventory();
        }
        else
        {
            Debug.LogError("Essential components for saving the game not found.");
        }
    }

  private void SaveGameState(Transform playerTransform, int seed)
{
    
    PrefabSpawner prefabSpawner = FindObjectOfType<PrefabSpawner>();
    if (prefabSpawner == null)
    {
        Debug.LogError("PrefabSpawner component not found.");
        return;
    }

   
    List<PrefabSpawnData> spawnDataList = prefabSpawner.GetSpawnData();

    
    GameData gameData = new GameData(playerTransform.position, playerTransform.rotation.eulerAngles, playerTransform.localScale, seed, spawnDataList);

    
    string jsonData = JsonUtility.ToJson(gameData);
    File.WriteAllText(saveFileName, jsonData);
}


    private void LoadGameState()
    {
        if (File.Exists(saveFileName))
        {
            string jsonData = File.ReadAllText(saveFileName);
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonData);

            Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            Inventory inventory = FindObjectOfType<Inventory>();
            PrefabSpawner prefabSpawner = FindObjectOfType<PrefabSpawner>();
            MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();

            if (playerTransform != null && inventory != null && prefabSpawner != null && mapGenerator != null)
            {
                playerTransform.position = loadedData.playerPosition;
                playerTransform.eulerAngles = loadedData.playerRotation;
                playerTransform.localScale = loadedData.playerScale;

                mapGenerator.RegenerateMap(mapGenerator.mapWidth, mapGenerator.mapHeight, loadedData.seed);
                inventory.loadInventory();
                prefabSpawner.LoadPrefabs(loadedData.prefabSpawnData); 
            }
        }
        else
        {
            Debug.LogError("Save file not found.");
        }
    }
}

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public Vector3 playerRotation;
    public Vector3 playerScale;
    public int seed;
    public List<PrefabSpawnData> prefabSpawnData;

    public GameData(Vector3 position, Vector3 rotation, Vector3 scale, int seed, List<PrefabSpawnData> prefabSpawnData)
    {
        this.playerPosition = position;
        this.playerRotation = rotation;
        this.playerScale = scale;
        this.seed = seed;
        this.prefabSpawnData = prefabSpawnData;
    }
}









