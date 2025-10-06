using UnityEngine;
using System.Collections.Generic;
using System.Linq; // LINQ kütüphanesini rastgele seçim için ekliyoruz.

public class ChanceBox_GridController : MonoBehaviour
{
    public event System.Action<ChanceBox_Box> OnBoxSelected;

    [SerializeField] private ChanceBox_Settings settings;
    [SerializeField] private List<ChanceBox_Box> boxes;

    private void Awake()
    {
        foreach (var box in boxes)
        {
            box.OnBoxClicked += HandleBoxClicked;
        }
    }

    public void SetupNewRound()
    {
        foreach (var box in boxes)
        {
            box.ResetVisuals();
        }

        // Tur için kullanılacak modifier'ları hazırla
        List<BoxModifier> availableModifiers = new List<BoxModifier>(settings.possibleModifiers);
        Shuffle(availableModifiers);

        // Her bir kutu için içerik belirle
        for (int i = 0; i < boxes.Count; i++)
        {
            BoxContent newContent = new BoxContent();

            // Zar at: Özel eşya mı, modifier mı?
            if (settings.possibleItems.Any() && Random.value < settings.itemDropChance)
            {
                // Özel Eşya seçildi
                newContent.contentType = ContentType.SpecialItem;
                // Rastgele bir özel eşya seç
                newContent.specialItem = settings.possibleItems[Random.Range(0, settings.possibleItems.Count)];
            }
            else
            {
                // Modifier seçildi
                newContent.contentType = ContentType.Modifier;
                // Listeden sıradaki modifier'ı al
                if (i < availableModifiers.Count)
                {
                    newContent.modifier = availableModifiers[i];
                }
                else
                {
                    // Eğer kutu sayısı modifier sayısından fazlaysa, en baştan rastgele bir tane daha ata
                    newContent.modifier = availableModifiers[Random.Range(0, availableModifiers.Count)];
                }
            }

            // Kutuyu bu yeni içerikle kur
            boxes[i].Setup(newContent);
        }
    }

    private void HandleBoxClicked(ChanceBox_Box clickedBox)
    {
        OnBoxSelected?.Invoke(clickedBox);
    }

    public void DisableAllBoxes()
    {
        foreach (var box in boxes)
        {
            box.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }

    public void EnableAllBoxes()
    {
        foreach (var box in boxes)
        {
            if (!box.GetIsClicked())
            {
                box.GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}