using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ChanceBox_Box : MonoBehaviour
{
    public event System.Action<ChanceBox_Box> OnBoxClicked;

    [Header("Component References")]
    [SerializeField] private Button boxButton;
    [SerializeField] private GameObject coverObject;

    [Header("Content Display")]
    [SerializeField] private TextMeshProUGUI modifierText;
    [SerializeField] private Image itemIconImage; // --- YENİ ALAN: Eşya ikonunu gösterecek Image component'i.

    private BoxContent _assignedContent; // Artık BoxModifier yerine BoxContent tutuyoruz.
    private bool _isClicked = false;

    private void Awake()
    {
        boxButton.onClick.AddListener(HandleClick);
    }

    public void Setup(BoxContent content)
    {
        _assignedContent = content;
        ResetVisuals(); // Görselleri başlangıç durumuna ayarla
    }

    public void RevealContent()
    {
        if (coverObject != null)
        {
            coverObject.SetActive(false);
        }

        // İçeriğin türüne göre ya metni ya da ikonu göster.
        if (_assignedContent.contentType == ContentType.Modifier)
        {
            modifierText.text = _assignedContent.modifier.displayText;
            modifierText.gameObject.SetActive(true);
            itemIconImage.gameObject.SetActive(false);
        }
        else if (_assignedContent.contentType == ContentType.SpecialItem)
        {
            itemIconImage.sprite = _assignedContent.specialItem.itemIcon;
            modifierText.gameObject.SetActive(false);
            itemIconImage.gameObject.SetActive(true);
        }
    }

    public void ResetVisuals()
    {
        _isClicked = false;
        boxButton.interactable = true;

        if (coverObject != null)
        {
            coverObject.SetActive(true);
        }
        modifierText.gameObject.SetActive(false);
        itemIconImage.gameObject.SetActive(false); // Başlangıçta ikonu da gizle.
    }

    private void HandleClick()
    {
        if (_isClicked || !boxButton.interactable) return;
        _isClicked = true;
        boxButton.interactable = false;
        OnBoxClicked?.Invoke(this);
    }

    public BoxContent GetContent() // Metodun adı ve dönüş türü değişti.
    {
        return _assignedContent;
    }

    public bool GetIsClicked()
    {
        return _isClicked;
    }
}