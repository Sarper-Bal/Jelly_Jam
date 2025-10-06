using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Dictionary için gerekli
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

    // --- YENİ ALANLAR: Envanter ve Oyun Durumu ---
    private Dictionary<SpecialItem_SO, int> _collectedItems = new Dictionary<SpecialItem_SO, int>();
    private int _extraClicksNextRound = 0;
    // ---------------------------------------------

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

        // Envanteri ve bonusları temizle
        _collectedItems.Clear();
        _extraClicksNextRound = 0;

        StartCoroutine(StartNextRoundWithDelay());
    }

    private void HandleBoxSelection(ChanceBox_Box selectedBox)
    {
        animator.PlayRevealAnimation(selectedBox);
        _clicksThisRound++;

        // --- GÜNCELLEME: Artık GetContent() kullanıyoruz ---
        BoxContent content = selectedBox.GetContent();

        if (content.contentType == ContentType.Modifier)
        {
            ProcessModifier(content.modifier);
        }
        else if (content.contentType == ContentType.SpecialItem)
        {
            ProcessSpecialItem(content.specialItem);
        }
        // ----------------------------------------------------

        // Tıklama hakkı bitti mi? (Ekstra hakları da hesaba kat)
        int clicksAllowedThisRound = settings.clicksPerRound + _extraClicksNextRound;
        if (_clicksThisRound >= clicksAllowedThisRound)
        {
            gridController.DisableAllBoxes();
            StartCoroutine(StartNextRoundWithDelay());
        }
    }

    private void ProcessModifier(BoxModifier modifier)
    {
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

        // Envantere ekle
        if (_collectedItems.ContainsKey(item))
        {
            _collectedItems[item]++;
        }
        else
        {
            _collectedItems.Add(item, 1);
        }

        // Eşya efekti tetiklendi mi diye kontrol et
        if (item.type == ItemType.Collectible && _collectedItems[item] >= item.requiredStack)
        {
            // Efekti tetikle!
            TriggerItemEffect(item);
            // Stack'i sıfırla
            _collectedItems[item] -= item.requiredStack;
        }
    }

    private void TriggerItemEffect(SpecialItem_SO item)
    {
        Debug.Log($"{item.itemName} efekti tetiklendi!");
        animator.ShowFeedbackText(feedbackText, $"{item.itemName} ETKİSİ!");

        // --- YILDIZ EFEKTİ BURADA TANIMLANIYOR ---
        if (item.itemName == "Yıldız") // İsme göre kontrol ediyoruz.
        {
            _extraClicksNextRound++;
        }
        // Gelecekteki diğer eşyalar için buraya "else if" eklenebilir.
    }

    private IEnumerator StartNextRoundWithDelay()
    {
        yield return new WaitForSeconds(1.5f);
        _currentRound++;

        if (_currentRound > settings.totalRounds)
        {
            EndMiniGame();
            yield break; // Coroutine'i burada durdur.
        }

        // Yeni tur başlıyor
        int clicksAllowedThisRound = settings.clicksPerRound + _extraClicksNextRound;
        _clicksThisRound = 0;

        roundText.text = $"TUR {_currentRound}/{settings.totalRounds} (+{_extraClicksNextRound} HAK)";
        animator.ShowFeedbackText(feedbackText, $"TUR {_currentRound}");

        // Yeni tur için kutuları hazırla
        gridController.SetupNewRound();

        // Bir sonraki tur için ekstra hakları sıfırla (eğer tek kullanımlıksa)
        // Eğer biriksin istiyorsak bu satırı sileriz. Şimdilik sıfırlayalım.
        _extraClicksNextRound = 0;
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