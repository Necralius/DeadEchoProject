using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranscriptionView : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _subtitle;
    [SerializeField] private TextMeshProUGUI _content;
    [SerializeField] private TextMeshProUGUI _signature;
    [SerializeField] private TextMeshProUGUI _footer;

    public void OpenView(NodeItem item)
    {
        if (item    == null) return;
        if (_title  == null) return;

        gameObject.SetActive(true);

        if (item.Content != null)
        {
            item.Content.Title.     ChangeText(ref _title);
            item.Content.Subtitle.  ChangeText(ref _subtitle);
            item.Content.Content.   ChangeText(ref _content);
            item.Content.Signature. ChangeText(ref _signature);
            item.Content.Footer.    ChangeText(ref _footer);
        }
    }
}