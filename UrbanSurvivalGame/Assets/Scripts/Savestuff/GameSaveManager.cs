using UnityEngine;

using System.IO;

public class GameSaveManager : MonoBehaviour
{
    public Transform playerTransform; 
    public MapGenerator mapGenerator; 
    private string saveFileName;


    private GameSaveManager saveManager;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        saveManager = FindObjectOfType<GameSaveManager>();
    }

    void Start()
    {
        saveFileName = Application.persistentDataPath + "/gameSave.json";
        LoadGame();
    }

    public void SaveGame()
    {
        string mapState = mapGenerator.mapWidth.ToString() + ";" + mapGenerator.mapHeight.ToString() + ";" + mapGenerator.seed.ToString();
        PlayerAndMapState state = new PlayerAndMapState(playerTransform, mapState);

        string jsonData = JsonUtility.ToJson(state);
        File.WriteAllText(saveFileName, jsonData);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFileName))
        {
            string jsonData = File.ReadAllText(saveFileName);
            PlayerAndMapState loadedState = JsonUtility.FromJson<PlayerAndMapState>(jsonData);

            playerTransform.position = loadedState.playerPosition.ToVector3();
            playerTransform.eulerAngles = loadedState.playerRotation.ToVector3();
            playerTransform.localScale = loadedState.playerScale.ToVector3();

            SetMapState(loadedState.mapState);
        }
    }

    private void SetMapState(string mapState)
    {
        string[] parameters = mapState.Split(';');
        if (parameters.Length >= 3)
        {
            int mapWidth = int.Parse(parameters[0]);
            int mapHeight = int.Parse(parameters[1]);
            int seed = int.Parse(parameters[2]);

            mapGenerator.RegenerateMap(mapWidth, mapHeight, seed);
        }
        else
        {
            Debug.LogError("Invalid map state data.");
        }
    }
}

[System.Serializable]
public class PlayerAndMapState
{
    public SerializableVector3 playerPosition;
    public SerializableVector3 playerRotation;
    public SerializableVector3 playerScale;
    public string mapState;

    public PlayerAndMapState(Transform playerTransform, string mapState)
    {
        playerPosition = new SerializableVector3(playerTransform.position);
        playerRotation = new SerializableVector3(playerTransform.eulerAngles);
        playerScale = new SerializableVector3(playerTransform.localScale);
        this.mapState = mapState;
    }
}

[System.Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}


