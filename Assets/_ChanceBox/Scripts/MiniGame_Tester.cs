using UnityEngine;
using UnityEngine.UI;

public class MiniGame_Tester : MonoBehaviour
{
    public ChanceBox_GameManager gameManager;
    public Button testButton;
    public float startingCoins = 100f;

    void Start()
    {
        testButton.onClick.AddListener(StartTest);
        // Sonucu dinlemek için event'e abone oluyoruz.
        ChanceBox_GameManager.OnMiniGameFinished += HandleGameFinished;
    }

    void StartTest()
    {
        Debug.Log("Mini oyun testini başlatıyorum...");
        gameManager.StartMiniGame(startingCoins);
        testButton.gameObject.SetActive(false); // Test butonunu gizle
    }

    void HandleGameFinished(float finalCoins)
    {
        Debug.Log($"Mini oyun bitti! Sonuç: {finalCoins} coin.");
        testButton.gameObject.SetActive(true); // Test butonunu tekrar göster
    }

    private void OnDestroy()
    {
        // Obje yok olduğunda event'ten aboneliği kaldırmak önemlidir.
        ChanceBox_GameManager.OnMiniGameFinished -= HandleGameFinished;
    }
}