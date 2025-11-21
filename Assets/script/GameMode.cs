using UnityEngine;

public enum PlayMode
{
    CPU,
    TwoPlayers
}

public class GameMode : MonoBehaviour
{
    public static GameMode Instance;

    public PlayMode currentMode;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ÉVÅ[ÉìÇå◊Ç¢Ç≈Ç‡écÇÈ
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMode(PlayMode mode)
    {
        currentMode = mode;
    }
}
