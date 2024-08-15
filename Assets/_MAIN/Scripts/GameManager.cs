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
