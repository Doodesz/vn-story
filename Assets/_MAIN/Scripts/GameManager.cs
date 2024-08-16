using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Debugging")]
    public string doorDestination;
    [SerializeField] Vector3 doorPos;
    public bool playerChangingMap = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void UpdateDoorDestinationPos(Vector3 pos)
    {
        doorPos = pos;
    }
}
