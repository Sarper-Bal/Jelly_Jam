using UnityEngine;
using System.Collections;
using TMPro;

public class ChanceBox_GameManager : MonoBehaviour
{
    public static event System.Action<float> OnMiniGameFinished;

    [Header("Core Components")]
    [SerializeField] private ChanceBox_Settings settings;
    [SerializeField] private ChanceBox_GridController gridController;
    [SerializeField] private ChanceBox_Animator animator;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject miniGameCanvas;

    private float _currentCoins;
    private int _currentRound;
    private int _clicksThisRound;

    private void Awake()
    {
        gridController.OnBoxSelected += HandleBoxSelection;
    }

    public void StartMiniGame(float initialCoins)
    {
        _currentCoins = initialCoins;
        coinText.text = _currentCoins.ToString("N0");
        _currentRound = 0;
        miniGameCanvas.SetActive(true);

        // İlk tur başlamadan önce tüm kutuları sıfırla.
        gridController.SetupNewRound();

        StartCoroutine(StartNextRoundWithDelay());
    }

    private void HandleBoxSelection(ChanceBox_Box selectedBox)
    {
        // Butona tıklandığında hemen animasyonu başlat.
        animator.PlayRevealAnimation(selectedBox);

        _clicksThisRound++;
        float previousCoins = _currentCoins;

        BoxModifier modifier = selectedBox.GetModifier();

        switch (modifier.operation)
        {
            case ModifierOperation.Add:
                _currentCoins += modifier.value;
                break;
            case ModifierOperation.Subtract:
                _currentCoins -= modifier.value;
                break;
            case ModifierOperation.Multiply:
                _currentCoins *= modifier.value;
                break;
        }

        if (_currentCoins < 0)
        {
            _currentCoins = 0;
        }

        animator.UpdateCoinText(coinText, previousCoins, _currentCoins);

        if (_clicksThisRound >= settings.clicksPerRound)
        {
            gridController.DisableAllBoxes(); // Tıklama hakkı bitti, tüm kutuları devre dışı bırak
            StartCoroutine(StartNextRoundWithDelay());
        }
    }

    private IEnumerator StartNextRoundWithDelay()
    {
        // Kutuların açılma animasyonlarının bitmesini bekliyoruz.
        yield return new WaitForSeconds(animator.GetRevealPunchDuration()); // Yeni bir getter eklememiz gerekecek!

        _currentRound++;

        if (_currentRound > settings.totalRounds)
        {
            EndMiniGame();
        }
        else
        {
            _clicksThisRound = 0;
            roundText.text = $"TUR {_currentRound}/{settings.totalRounds}";
            animator.ShowFeedbackText(feedbackText, $"TUR {_currentRound}");
            gridController.SetupNewRound();
            // Yeni tur başladığında kutuları tekrar tıklanabilir yap.
            gridController.EnableAllBoxes(); // GÜNCELLEME: Yeni eklenen metot.
        }
    }

    private void EndMiniGame()
    {
        animator.ShowFeedbackText(feedbackText, "OYUN BİTTİ!");
        OnMiniGameFinished?.Invoke(_currentCoins);
        StartCoroutine(DeactivateCanvasAfterDelay(2.0f));
    }

    private IEnumerator DeactivateCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        miniGameCanvas.SetActive(false);
    }
}