using UnityEngine;
using System.Collections.Generic;

// --- YENİ VERİ YAPISI ---
// Bir kutunun içeriğinin ne olabileceğini tanımlar.
// Ya bir Modifier'dır ya da bir Special Item.
public enum ContentType { Modifier, SpecialItem }

[System.Serializable]
public struct BoxContent
{
    public ContentType contentType;
    public BoxModifier modifier;
    public SpecialItem_SO specialItem;
}
// ----------------------


public enum ModifierOperation { Add, Subtract, Multiply }

[System.Serializable]
public struct BoxModifier
{
    public ModifierOperation operation;
    public float value;
    public string displayText;
}


[CreateAssetMenu(fileName = "NewChanceBoxSettings", menuName = "MiniGames/ChanceBox/Game Settings")]
public class ChanceBox_Settings : ScriptableObject
{
    [Header("Game Rules")]
    public int totalRounds = 3;
    public int clicksPerRound = 3;

    [Header("Modifiers")]
    public List<BoxModifier> possibleModifiers;

    [Header("Special Items")]
    // --- YENİ ALANLAR ---
    [Tooltip("Kutulara atanabilecek tüm özel eşyaların listesi.")]
    public List<SpecialItem_SO> possibleItems;

    [Tooltip("Bir kutudan Modifier yerine Special Item çıkma olasılığı (0 ile 1 arasında).")]
    [Range(0, 1)]
    public float itemDropChance = 0.2f; // %20 şans
    // ----------------------
}