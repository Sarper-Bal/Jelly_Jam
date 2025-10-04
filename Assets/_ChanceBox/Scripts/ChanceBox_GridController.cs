using UnityEngine;
using System.Collections.Generic;

public class ChanceBox_GridController : MonoBehaviour
{
    public event System.Action<ChanceBox_Box> OnBoxSelected;

    [SerializeField] private ChanceBox_Settings settings;
    [SerializeField] private List<ChanceBox_Box> boxes;

    private List<BoxModifier> _roundModifiers = new List<BoxModifier>();

    private void Awake()
    {
        foreach (var box in boxes)
        {
            box.OnBoxClicked += HandleBoxClicked;
        }
    }

    public void SetupNewRound()
    {
        // GÜNCELLEME: Yeni tur başlamadan önce tüm kutuları sıfırla.
        foreach (var box in boxes)
        {
            box.ResetVisuals();
        }

        _roundModifiers = new List<BoxModifier>(settings.possibleModifiers);
        Shuffle(_roundModifiers);

        for (int i = 0; i < boxes.Count; i++)
        {
            if (i >= _roundModifiers.Count)
            {
                Debug.LogError("Yeterli 'possibleModifier' yok! Kutulara atanacak veri bulunamadı.");
                return;
            }
            boxes[i].Setup(_roundModifiers[i]);
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

    /// <summary>
    /// Tüm kutuların tıklanabilirliğini açar. GameManager tarafından çağrılacak.
    /// </summary>
    public void EnableAllBoxes() // Yeni metod
    {
        foreach (var box in boxes)
        {
            // Sadece tıklanmamış kutuları aç (tıklananlar zaten disabled kalmalı)
            if (!box.gameObject.GetComponent<ChanceBox_Box>().GetIsClicked()) // GetIsClicked eklenmeli
            {
                box.GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
        }
    }
    // _isClicked değeri için public bir getter eklemeliyiz ChanceBox_Box script'ine.

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