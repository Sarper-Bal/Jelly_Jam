using UnityEngine;
using TMPro;
using DG.Tweening;

public class ChanceBox_Animator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float revealPunchAmount = 0.1f; // Kutunun açılırken ne kadar büyüyeceği
    [SerializeField] private float revealPunchDuration = 0.5f; // Kutunun açılma animasyon süresi
    [SerializeField] private float textUpdateDuration = 0.7f;
    [SerializeField] private float punchScaleAmount = 0.1f;
    [SerializeField] private float punchDuration = 0.2f;
    [SerializeField] private float feedbackFadeDuration = 1.0f;

    private Sequence _activeCoinSequence;

    /// <summary>
    /// Belirtilen kutu için yeni "punch" efektiyle bir açılma animasyonu oynatır.
    /// </summary>
    public void PlayRevealAnimation(ChanceBox_Box box)
    {
        // Önce kutunun içeriğini görünür yap.
        box.RevealContent();

        // PunchScale efekti uygulayarak kutunun açılma animasyonunu yap.
        // Geri dönüşte butonu tekrar tıklanabilir hale getir (GameManager'dan çağrıldığında)
        box.transform.DOPunchScale(Vector3.one * revealPunchAmount, revealPunchDuration, 1, 0.5f)
            .OnComplete(() =>
            {
                // Animasyon bitince GameManager'a kutunun tıklanma durumunu yönetmesi için izin ver.
                // Bu kısım normalde GameManager tarafından yönetileceği için burada pasif bırakıyorum.
                // box.GetComponent<UnityEngine.UI.Button>().interactable = true; 
            });

        // Bu animasyon sırasında butonu hala tıklanamaz tutmak önemli, GameManager daha sonra yönetecek.
    }

    public void UpdateCoinText(TextMeshProUGUI coinText, float startValue, float endValue)
    {
        _activeCoinSequence?.Complete();

        _activeCoinSequence = DOTween.Sequence();
        _activeCoinSequence.Append(DOTween.To(() => startValue, x => startValue = x, endValue, textUpdateDuration)
            .OnUpdate(() =>
            {
                coinText.text = startValue.ToString("N0");
            })
            .SetEase(Ease.OutCubic));
    }

    public void PlayPunchEffect(Transform targetTransform)
    {
        targetTransform.DOPunchScale(Vector3.one * punchScaleAmount, punchDuration, 1, 0.5f);
    }

    public void ShowFeedbackText(TextMeshProUGUI feedbackText, string message)
    {
        feedbackText.text = message;
        feedbackText.alpha = 1f;

        feedbackText.DOFade(0, feedbackFadeDuration).SetDelay(1f);
    }
    public float GetRevealPunchDuration()
    {
        return revealPunchDuration;
    }

}