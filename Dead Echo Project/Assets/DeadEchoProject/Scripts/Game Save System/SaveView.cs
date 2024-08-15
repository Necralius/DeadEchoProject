using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveView : MonoBehaviour
{
    [SerializeField] private Image               _lastScreenshot = null;
    [SerializeField] private TextMeshProUGUI     _saveName       = null;
    [SerializeField] private TextMeshProUGUI     _lastQuest      = null;
    [SerializeField] private TextMeshProUGUI     _saveHour       = null;

    [SerializeField] private SaveData _gameData = null;

    private UnityEvent _loadSave = new UnityEvent();

    private Button _btn;

    private void Start()
    {
        _loadSave   = new UnityEvent();
        _btn = GetComponent<Button>();

        _btn.onClick.AddListener(delegate { SelectSave(); });

        SetUp(_gameData);
    }

    private void SelectSave() => SaveScreen.Instance?.SelectSave(_gameData);

    private void ButtonAction()
    {
        if (_gameData == null) return;

        FadeSystemManager.Instance.CallFadeAction(_loadSave);
    }

    /// <summary>
    /// This method setup the save on the UI view, setting his data on the correct texts and setting the last screenshot on the UI.
    /// </summary>
    /// <param name="gameData">This represents the game save data that will carry all the informationns, for clear the save view pass 
    /// a null game save. </param>
    public void SetUp(SaveData gameData) 
    {
        if (gameData == null || gameData.saveName.Equals(string.Empty))
        {
            _saveName.text          = "Empty Save";
            _lastScreenshot.sprite  = GameStateManager.Instance?.noSaveImage;

            _saveName.gameObject.SetActive(true);
            _saveHour.gameObject.SetActive(false);

            return;
        }

        _gameData = gameData;

        _saveName.text = _gameData.saveName;
        _saveHour.text = $"Game Saved at: {_gameData.saveTime.Hour}:{_gameData.saveTime.Minute} - {_gameData.saveTime.Day}/{_gameData.saveTime.Month}/{_gameData.saveTime.Year}";

        Texture2D lastScreenshot = gameData.GetImage();

        if (lastScreenshot == null) return;
        else
        {
            Rect rect = new Rect(0, 0, lastScreenshot.width, lastScreenshot.height);

            Sprite lastScreenshotSprite = Sprite.Create(lastScreenshot, rect, new Vector2(0, 0), 1);
            _lastScreenshot.sprite = lastScreenshotSprite;
        }
    }
}