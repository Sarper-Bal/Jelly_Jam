using UnityEngine;

// Eşyanın genel kategorisini belirlemek için bir enum.
// Collectible: Biriktirilebilir (Yıldız, Anahtar)
// InstantEffect: Anında etki eden (Kalkan)
public enum ItemType { Collectible, InstantEffect }

[CreateAssetMenu(fileName = "NewSpecialItem", menuName = "MiniGames/ChanceBox/Special Item")]
public class SpecialItem_SO : ScriptableObject
{
    [Header("Info")]
    public string itemName; // Eşyanın adı (örn: "Star")
    public Sprite itemIcon; // Kutuda ve UI'da göstereceğimiz ikon.
    [TextArea] public string itemDescription; // Eşyanın açıklaması.

    [Header("Logic")]
    public ItemType type; // Eşyanın türü.

    // Eğer türü 'Collectible' ise, etkinin tetiklenmesi için kaç tane toplanması gerektiği.
    [Tooltip("Eğer tür 'Collectible' ise, etki için kaç tane biriktirilmeli?")]
    public int requiredStack = 3;
}