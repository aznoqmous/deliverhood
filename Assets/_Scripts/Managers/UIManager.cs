using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] GameObject _gameOverObject;
    [SerializeField] ResourceUI _resourceUIPrefab;
    [SerializeField] Transform _resourcesContainer;
    [SerializeField] Dictionary<ScriptableResource, ResourceUI> _resources = new Dictionary<ScriptableResource, ResourceUI>();
    
    [SerializeField] TextMeshProUGUI _goldText;

    [SerializeField] TextMeshProUGUI _taxText;
    [SerializeField] TextMeshProUGUI _taxAmountText;
    [SerializeField] Slider _taxSlider;
    [SerializeField] Image _taxSliderFill;
    [SerializeField] Color _colorGreen;
    [SerializeField] Color _colorRed;

    [Header("Level")]
    [SerializeField] TextMeshProUGUI _levelTmp;
    [SerializeField] Slider _satisfactionSlider;
    [SerializeField] TextMeshProUGUI _satisfactionTmp;

    [Header("Statuses")]
    [SerializeField] StatusTextUI _statuxTextPrefab;
    [SerializeField] RatingUI _ratingPrefab;
    [SerializeField] Transform _reviewsContainer;

    [Header("Reviews")]
    List<float> _ratings = new List<float>();
    [SerializeField] TextMeshProUGUI _totalRatingsTmp;


    public void Update()
    {
        if(GameManager.Instance.State == GameState.Playing)
        {
            _satisfactionSlider.value = Mathf.Lerp(_satisfactionSlider.value, Player.Instance.CurrentSatisfaction / Player.Instance.MaxSatisfaction, Time.deltaTime);
            _satisfactionTmp.text = Mathf.FloorToInt(Player.Instance.CurrentSatisfaction).ToString();
            UpdateTaxText();
            UpdateTaxSlider();
        }
    }
    public void Clear()
    {
        _resources.Clear();
        foreach (Transform t in _reviewsContainer) Destroy(t.gameObject);
        _ratings.Clear();
        foreach (Transform t in _resourcesContainer) Destroy(t.gameObject);
    }

    public void UpdateResource(ScriptableResource resource, int amount)
    {
        if (resource.IsCoin)
        {
            _goldText.text = amount.ToString();
            return;
        }

        _resources.TryGetValue(resource, out ResourceUI resourceUI);
        if(resourceUI == null)
        {
            resourceUI = Instantiate(_resourceUIPrefab, _resourcesContainer);
            resourceUI.SetSprite(resource.Sprite);
            _resources.Add(resource, resourceUI);
        }
        resourceUI.SetCount(amount);
    }

    public void UpdatePlayerResources()
    {
        foreach(var item in Player.Instance.Resources)
        {
            UpdateResource(item.Key, item.Value);
        }
    }

    public void AddStatusText(Vector3 worldPosition, string text, Sprite sprite=null)
    {
        StatusTextUI statusText = Instantiate(_statuxTextPrefab, Camera.main.WorldToScreenPoint(worldPosition), Quaternion.identity, transform);
        statusText.SetText(text);
        statusText.SetSprite(sprite);
        StartCoroutine(statusText.DestroyAfter(2f));
    }

    public void AddRating(Vector3 worldPosition, float value=5f)
    {
        RatingUI rating = Instantiate(_ratingPrefab, Camera.main.WorldToScreenPoint(worldPosition), Quaternion.identity, transform);
        rating.SetRating(value);
       
        RatingUI completedRating = Instantiate(rating, _reviewsContainer);
        completedRating.transform.SetAsFirstSibling();
        completedRating.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        completedRating.ShowBackground();
        if(_reviewsContainer.childCount > 5) Destroy(_reviewsContainer.GetChild(_reviewsContainer.childCount - 1).gameObject);

        rating.HideDescription();
        rating.PlayAnimation();
        StartCoroutine(rating.DestroyAfter(7f));

        _ratings.Add(Mathf.FloorToInt(value));
        UpdateTotalRating();
    }

    public void UpdatePlayerLevel()
    {
        _levelTmp.text = (Player.Instance.Level+1).ToString();
    }

    public void ResetSatisfaction()
    {
        _satisfactionSlider.value = 0;
    }

    public void UpdateTaxText()
    {
        float days = Mathf.FloorToInt(GameManager.Instance.TimeToDays(GameManager.Instance.GetTaxTimeLeft()));
        int amount = GameManager.Instance.GetTax();
        _taxText.text = $"due in {days} days";
        _taxAmountText.text = amount.ToString();
    }
    public void UpdateTaxSlider()
    {
        _taxSlider.value = Mathf.Lerp(_taxSlider.value, GameManager.Instance.GetTaxRatioLeft(), Time.deltaTime);
        _taxSliderFill.color = Color.Lerp(_taxSliderFill.color, GameManager.Instance.GetTax() < Player.Instance.GetCurrentCoins() ? _colorGreen : _colorRed, Time.deltaTime);
    }
    public void ResetTaxSlider()
    {
        _taxSlider.value = 1;
    }

    public void UpdateTotalRating()
    {
        if(_ratings.Count <= 0)
        {
            _totalRatingsTmp.text = "";
            return;
        }
        float total = 0;
        foreach (float rating in _ratings) total += rating;
        string strTotal = (total / _ratings.Count).ToString("0.##");
        _totalRatingsTmp.text = $"{strTotal} over {_ratings.Count} reviews"; 
    }

    public void ShowGameOver(bool state = true)
    {
        _gameOverObject.SetActive(state);
    }
}
