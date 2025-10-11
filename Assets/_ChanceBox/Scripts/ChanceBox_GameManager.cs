using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject shieldStatusIcon; // Kalkan UI ikonu
    [SerializeField] private TextMeshProUGUI keyCounterText; // Anahtar sayacı metni

    [Header("Special Item Definitions")]
    [Tooltip("Hangi item'ın 'Yıldız' olduğunu buraya sürükleyin.")]
    [SerializeField] private SpecialItem_SO starItemDefinition;
    [Tooltip("Hangi item'ın 'Kalkan' olduğunu buraya sürükleyin.")]
    [SerializeField] private SpecialItem_SO shieldItemDefinition;
    [Tooltip("Hangi item'ın 'Anahtar' olduğunu buraya sürükleyin.")]
    [SerializeField] private SpecialItem_SO keyItemDefinition;

    // --- Oyun Durumu Değişkenleri ---
    private Dictionary<SpecialItem_SO, int> _collectedItems = new Dictionary<SpecialItem_SO, int>();
    private int _extraClicksNextRound = 0;
    private bool _isShieldActive = false;
    private bool _hasBonusChestKey = false;

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

        // Tüm durumları sıfırla
        _collectedItems.Clear();
        _extraClicksNextRound = 0;
        _isShieldActive = false;
        _hasBonusChestKey = false;
        UpdateUI();

        StartCoroutine(StartNextRoundWithDelay());
    }

    private void HandleBoxSelection(ChanceBox_Box selectedBox)
    {
        animator.PlayRevealAnimation(selectedBox);
        _clicksThisRound++;

        BoxContent content = selectedBox.GetContent();

        if (content.contentType == ContentType.Modifier)
        {
            ProcessModifier(content.modifier);
        }
        else if (content.contentType == ContentType.SpecialItem)
        {
            ProcessSpecialItem(content.specialItem);
        }

        int clicksAllowedThisRound = settings.clicksPerRound + _extraClicksNextRound;
        if (_clicksThisRound >= clicksAllowedThisRound)
        {
            gridController.DisableAllBoxes();
            StartCoroutine(StartNextRoundWithDelay());
        }
    }

    private void ProcessModifier(BoxModifier modifier)
    {
        // Kalkan aktif mi ve negatif bir etki mi geldi?
        if (_isShieldActive && (modifier.operation == ModifierOperation.Subtract || (modifier.operation == ModifierOperation.Multiply && modifier.value < 1.0f)))
        {
            _isShieldActive = false;
            UpdateUI();
            animator.ShowFeedbackText(feedbackText, "KALKAN KORUDU!");
            Debug.Log("Kalkan negatif etkiyi engelledi!");
            return; // Hesaplamanın geri kalanını atla
        }

        float previousCoins = _currentCoins;
        switch (modifier.operation)
        {
            case ModifierOperation.Add: _currentCoins += modifier.value; break;
            case ModifierOperation.Subtract: _currentCoins -= modifier.value; break;
            case ModifierOperation.Multiply: _currentCoins *= modifier.value; break;
        }
        if (_currentCoins < 0) _currentCoins = 0;
        animator.UpdateCoinText(coinText, previousCoins, _currentCoins);
    }

    private void ProcessSpecialItem(SpecialItem_SO item)
    {
        Debug.Log($"{item.itemName} adlı özel eşya bulundu!");
        if (!_collectedItems.ContainsKey(item)) _collectedItems.Add(item, 0);
        _collectedItems[item]++;
        UpdateUI();

        if ((item.type == ItemType.Collectible && _collectedItems[item] >= item.requiredStack) || item.type == ItemType.InstantEffect)
        {
            TriggerItemEffect(item);
            if (item.type == ItemType.Collectible) _collectedItems[item] -= item.requiredStack;
        }
    }

    private void TriggerItemEffect(SpecialItem_SO item)
    {
        Debug.Log($"{item.itemName} efekti tetiklendi!");
        animator.ShowFeedbackText(feedbackText, $"{item.itemName} ETKİSİ!");

        if (item == starItemDefinition)
        {
            _extraClicksNextRound++;
        }
        else if (item == shieldItemDefinition)
        {
            _isShieldActive = true;
        }
        else if (item == keyItemDefinition)
        {
            _hasBonusChestKey = true;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Kalkan ikonunu güncelle
        if (shieldStatusIcon != null) shieldStatusIcon.SetActive(_isShieldActive);

        // Anahtar sayacını güncelle
        if (keyCounterText != null && keyItemDefinition != null)
        {
            int keyCount = _collectedItems.ContainsKey(keyItemDefinition) ? _collectedItems[keyItemDefinition] : 0;
            keyCounterText.text = $"Anahtar: {keyCount}/{keyItemDefinition.requiredStack}";
        }

        // Tur metnini güncelle (ekstra hakları da göster)
        int clicksAllowedThisRound = settings.clicksPerRound + _extraClicksNextRound;
        roundText.text = $"TUR {_currentRound}/{settings.totalRounds} | HAK: {clicksAllowedThisRound}";
    }

    private IEnumerator StartNextRoundWithDelay()
    {
        yield return new WaitForSeconds(1.5f);
        _currentRound++;
        UpdateUI(); // Tur metnini güncellemek için

        if (_currentRound > settings.totalRounds)
        {
            EndMiniGame();
            yield break;
        }

        _clicksThisRound = 0;
        animator.ShowFeedbackText(feedbackText, $"TUR {_currentRound}");
        gridController.SetupNewRound();
        _extraClicksNextRound = 0;
    }

    private void EndMiniGame()
    {
        // Anahtar varsa bonusu uygula
        if (_hasBonusChestKey)
        {
            float bonusAmount = 500; // Örnek bonus miktar
            animator.ShowFeedbackText(feedbackText, $"BONUS SANDIK! +{bonusAmount}");
            ProcessModifier(new BoxModifier { operation = ModifierOperation.Add, value = bonusAmount });
        }

        OnMiniGameFinished?.Invoke(_currentCoins);
        StartCoroutine(DeactivateCanvasAfterDelay(2.5f));
    }

    private IEnumerator DeactivateCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        miniGameCanvas.SetActive(false);
    }
}