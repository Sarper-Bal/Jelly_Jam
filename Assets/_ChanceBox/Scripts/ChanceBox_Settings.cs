using UnityEngine;
using System.Collections.Generic;

// Bu enum, kutunun ne tür bir işlem yapacağını belirtir.
public enum ModifierOperation { Add, Subtract, Multiply }

// Her bir kutunun potansiyel değerini tutan yapı.
[System.Serializable]
public struct BoxModifier
{
    public ModifierOperation operation;

    // GÜNCELLEME: Değişkeni 'int' yerine 'float' yaptık. Artık 0.5 gibi ondalıklı değerler kullanabiliriz.
    public float value;

    // Butonun üzerinde ne yazacağını buradan belirleriz (örn: "+50", "x2", "/2").
    public string displayText;
}

// Bu satır, Unity'nin "Create" menüsüne yeni bir seçenek ekler.
[CreateAssetMenu(fileName = "NewChanceBoxSettings", menuName = "MiniGames/ChanceBox/Game Settings")]
public class ChanceBox_Settings : ScriptableObject
{
    [Header("Game Rules")]
    [Tooltip("Oyun toplam kaç tur sürecek?")]
    public int totalRounds = 3;

    [Tooltip("Her turda oyuncu kaç kutu açabilir?")]
    public int clicksPerRound = 3;

    [Header("Possible Modifiers")]
    [Tooltip("Her tur başında kutulara atanabilecek tüm olası değerler listesi.")]
    public List<BoxModifier> possibleModifiers;
}