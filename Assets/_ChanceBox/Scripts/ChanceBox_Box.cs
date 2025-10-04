using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // Animasyon için DoTween eklendi

[RequireComponent(typeof(Button))]
public class ChanceBox_Box : MonoBehaviour
{
    public event System.Action<ChanceBox_Box> OnBoxClicked;

    [SerializeField] private Button boxButton;
    [SerializeField] private TextMeshProUGUI modifierText;
    [SerializeField] private GameObject coverObject;

    private BoxModifier _assignedModifier;
    private bool _isClicked = false;

    private void Awake()
    {
        boxButton.onClick.AddListener(HandleClick);
    }

    public bool GetIsClicked()
    {
        return _isClicked;
    }


    public void Setup(BoxModifier modifier)
    {
        _assignedModifier = modifier;
        modifierText.text = _assignedModifier.displayText;

        // Kutuyu başlangıç durumuna sıfırla ve metni gizle
        ResetVisuals();
    }

    /// <summary>
    /// Kutunun içeriğini gösterir (kapağı kaldırır, metni görünür yapar).
    /// Animasyon yöneticisi tarafından çağrılır.
    /// </summary>
    public void RevealContent() // Metodun adı 'Reveal' yerine 'RevealContent' olarak değiştirildi karışıklığı önlemek için.
    {
        if (coverObject != null)
        {
            coverObject.SetActive(false);
        }
        modifierText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Kutunun görsellerini başlangıç durumuna döndürür (kapak açık, metin gizli, tıklanabilir).
    /// </summary>
    public void ResetVisuals()
    {
        _isClicked = false;
        boxButton.interactable = true;

        if (coverObject != null)
        {
            coverObject.SetActive(true); // Kapak açık
        }
        modifierText.gameObject.SetActive(false); // Metin gizli
    }

    private void HandleClick()
    {
        if (_isClicked || !boxButton.interactable)
        {
            return;
        }

        _isClicked = true;
        // Animasyon oynarken tekrar tıklanmasını engellemek için geçici olarak kapat
        boxButton.interactable = false;

        OnBoxClicked?.Invoke(this);
    }

    public BoxModifier GetModifier()
    {
        return _assignedModifier;
    }
}