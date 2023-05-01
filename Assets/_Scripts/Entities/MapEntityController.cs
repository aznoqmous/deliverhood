using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapEntityController : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;

    [Header("Action")]
    [SerializeField] GameObject _actionCanvas;
    [SerializeField] Image _consumedSprite;
    [SerializeField] Image _gainedSprite;
    [SerializeField] Image _transactionArrow;
    [SerializeField] Image _countDownImage;
    [SerializeField] GameObject _countDownContainer;

    private List<ScriptableResource> _availableConsumedResources;
    private List<ScriptableResource> _availableGainedResources;

    private ScriptableResource _consumedResource;
    private ScriptableResource _gainedResource;
    [SerializeField] private float _cooldown;
    [SerializeField] private float _lastTime;
    [SerializeField] private float _deliveryMaxTime;
    [SerializeField] private float _deliveryTotalTime;
    private MapEntityType _type;

    public void Load(ScriptableMapEntity sme)
    {
        _spriteRenderer.sprite = sme.Sprite;

        _cooldown = sme.Cooldown;
        _type = sme.Type;
        _availableConsumedResources = sme.ConsumedResources;
        _availableGainedResources = sme.GainedResources;
        Refresh();
    }

    private void Update()
    {
        if(GameState.Playing == GameManager.Instance.State)
        {
            _actionCanvas.SetActive(IsAvailable());
            _countDownContainer.SetActive(_deliveryMaxTime > 0||IsInCooldown());
            UpdateDeliveryTimeLeft();
            UpdateCooldown();
            if (HasToRefresh()) Refresh();
            switch (_type)
            {
                case MapEntityType.City:
                    if (IsAvailable()) Player.Instance.RemoveSatisfaction(Time.deltaTime/50f);
                    break;
                case MapEntityType.Resource:
                    break;
                case MapEntityType.Trade:
                    break;
                case MapEntityType.Consumer:
                    break;
                default:
                    break;
            }
        }
    }

    public void Refresh()
    {
        _lastTime = -_cooldown;
        switch (_type)
        {
            case MapEntityType.City:
                _availableConsumedResources = Player.Instance.AvailableResources.Where(r => !r.IsCoin).ToList();
                SetRandomNeed(Random.Range(1, 3));
                SetRandomGain();
                break;
            case MapEntityType.Trade:
                SetRandomNeed(-1);
                SetRandomGain();
                break;
            case MapEntityType.Resource:
                _transactionArrow.gameObject.SetActive(false);
                _consumedSprite.gameObject.SetActive(false);
                _deliveryMaxTime = -1;
                SetRandomGain();
                break;
            default:
                break;
        }
        _gainedSprite.sprite = _gainedResource != null ? _gainedResource.Sprite : null;
        _consumedSprite.sprite = _consumedResource != null ? _consumedResource.Sprite : null;

    }

    public void SetRandomNeed(float days)
    {
        ScriptableResource sr = _availableConsumedResources[Random.Range(0, _availableConsumedResources.Count)];
        SetNeed(sr, days);
    }

    public void SetNeed(ScriptableResource sr, float days)
    {
        _deliveryTotalTime = GameManager.Instance.DaysToTime(days);
        _deliveryMaxTime = GameManager.Instance.CurrentTime + _deliveryTotalTime;
        if (days == -1) _deliveryMaxTime = -1;
        _consumedResource = sr;
        _consumedSprite.sprite = _consumedResource.Sprite;
    }

    public void SetRandomGain()
    {
        ScriptableResource sr = _availableGainedResources[Random.Range(0, _availableGainedResources.Count)];
        SetGain(sr);
    }

    public void SetGain(ScriptableResource sr)
    {
        _gainedResource = sr;
        _gainedSprite.sprite = _gainedResource.Sprite;
    }



    public void UpdateDeliveryTimeLeft()
    {
        if (IsAvailable())
        {
            _countDownImage.fillAmount = (_deliveryMaxTime - GameManager.Instance.CurrentTime) / _deliveryTotalTime;
            _countDownImage.color = Color.HSVToRGB(_countDownImage.fillAmount * 135f / 360f, 0.7f,0.7f);
        }
        
    }
    public void UpdateCooldown()
    {
        if (IsInCooldown())
        {
            _countDownImage.fillAmount = GameManager.Instance.TimeToDays(GameManager.Instance.CurrentTime - _lastTime) / _cooldown;
            _countDownImage.color = new Color(255, 255, 255, 0.3f);
        }
    }

    public bool IsAvailable()
    {
        return
            !IsInCooldown() &&
            (_deliveryMaxTime == -1 || _deliveryMaxTime - GameManager.Instance.CurrentTime > 0)
        ;
    }

    public bool IsInCooldown()
    {
        return _lastTime > 0 && GameManager.Instance.TimeToDays(GameManager.Instance.CurrentTime - _lastTime) < _cooldown;
    }

    public bool HasToRefresh()
    {
        return !(_deliveryMaxTime == -1 || _deliveryMaxTime - GameManager.Instance.CurrentTime > 0)
            && (_cooldown <= 0 || GameManager.Instance.TimeToDays(GameManager.Instance.CurrentTime - _lastTime) > _cooldown)
        ;
    }

    /**
     * Triggered on entering new day
     */
    public void Tick()
    {
  
    }

    /**
     * Triggered when player enter the cell
     */
    public void Activate(Courier Courier)
    {
        if (IsAvailable())
        {
            if (Consume())
            {
                switch (_type)
                {
                    case MapEntityType.City:
                        Player.Instance.AddSatisfaction(1f);
                        UIManager.Instance.AddRating(transform.position, GetDeliveryRating());
                        break;
                    case MapEntityType.Resource:
                        break;
                    case MapEntityType.Consumer:
                        break;
                    default:
                        break;
                }
                Dispatch(Courier);
                _lastTime = GameManager.Instance.CurrentTime;
                _deliveryMaxTime = 0;
            }
        }
        
    }

    /**
     * Returns true if requirements are met, else false
     */
    public bool Consume()
    {
        
        if (_consumedResource != null && !Player.Instance.ConsumeResource(_consumedResource)) return false;
        return true;
    }

    public void Dispatch(Courier Courier)
    {
        if (_gainedResource != null) Courier.GainResource(_gainedResource);
    }

    public float GetDeliveryRating()
    {
        return (_deliveryMaxTime - GameManager.Instance.CurrentTime) / _deliveryTotalTime * 5f + 1f;
    }
}
