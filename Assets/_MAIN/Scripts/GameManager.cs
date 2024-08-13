using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Debugging")]
    public string doorDestination;
    [SerializeField] Vector3 doorPos;
    public bool playerChangingMap = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Found more than one Game Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void UpdateDoorDestinationPos(Vector3 pos)
    {
        doorPos = pos;
    }
}
