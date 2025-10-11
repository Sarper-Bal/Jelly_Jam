using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField][Range(0, 1)] private float inactiveAlpha = 0.4f; // Yarı saydamlık değeri

    private SpecialItem_SO _itemData;

    /// <summary>
    /// Bu UI elemanını belirli bir eşya verisiyle kurar.
    /// </summary>
    public void Initialize(SpecialItem_SO itemData)
    {
        _itemData = itemData;
        itemIcon.sprite = _itemData.itemIcon;
        UpdateDisplay(0, false); // Başlangıç durumu: 0 adet, inaktif.
    }

    /// <summary>
    /// Göstergeyi güncel (adet ve aktiflik) durumuna göre günceller.
    /// </summary>
    public void UpdateDisplay(int currentAmount, bool isActive)
    {
        // Metni güncelle
        if (_itemData.type == ItemType.Collectible)
        {
            progressText.text = $"{currentAmount}/{_itemData.requiredStack}";
        }
        else // InstantEffect ise sayaca gerek yok.
        {
            progressText.text = "";
        }

        // Saydamlığı güncelle
        bool shouldBeOpaque = (currentAmount > 0) || isActive;
        Color iconColor = itemIcon.color;
        iconColor.a = shouldBeOpaque ? 1.0f : inactiveAlpha;
        itemIcon.color = iconColor;
    }
}