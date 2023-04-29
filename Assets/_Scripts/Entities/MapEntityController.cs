using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86;

public class MapEntityController : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;

    [Header("Action")]
    [SerializeField] GameObject _actionCanvas;
    [SerializeField] Image _consumedSprite;
    [SerializeField] Image _gainedSprite;
    [SerializeField] Image _transactionArrow;
    [SerializeField] TextMeshProUGUI _actionTmp;

    private List<ScriptableResource> _availableConsumedResources;
    private List<ScriptableResource> _availableGainedResources;

    private ScriptableResource _consumedResource;
    private ScriptableResource _gainedResource;
    [SerializeField] private float _cooldown;
    [SerializeField] private float _lastTime;
    [SerializeField] private int _deliveryMaxTime;
    private MapEntityType _type;

    public void Load(ScriptableMapEntity sme)
    {
        _spriteRenderer.sprite = sme.Sprite;

        _cooldown = sme.Cooldown;
        _type = sme.Type;
        _availableConsumedResources = sme.ConsumedResources;
        _availableGainedResources = sme.GainedResources;

        _lastTime = -_cooldown;
        Refresh();
    }

    public void Refresh()
    {
        _actionTmp.text = "";
        switch (_type)
        {
            case MapEntityType.City:
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

    public void SetRandomNeed(int days)
    {
        ScriptableResource sr = _availableConsumedResources[Random.Range(0, _availableConsumedResources.Count)];
        SetNeed(sr, days);
    }

    public void SetNeed(ScriptableResource sr, int days)
    {
        _deliveryMaxTime = GameManager.Instance.GetCurrentDay() + days;
        if (days == -1) _deliveryMaxTime = -1;
        _consumedResource = sr;
        _consumedSprite.sprite = _consumedResource.Sprite;
        _actionCanvas.SetActive(true);
        UpdateDeliveryTimeLeft();
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
        if (_deliveryMaxTime > 0)
        {
            _actionTmp.text = (_deliveryMaxTime - GameManager.Instance.GetCurrentDay()).ToString() + " days";
        }
        else
        {
            _actionTmp.text = "";
        }
    }

    public bool IsAvailable()
    {
        return
            !IsInCooldown() &&
            (_deliveryMaxTime == -1 || _deliveryMaxTime - GameManager.Instance.GetCurrentDay() > 0)
        ;
    }

    public bool IsInCooldown()
    {
        return _lastTime > 0 && GameManager.Instance.GetCurrentDay() - _lastTime < _cooldown;
    }

    public bool HasToRefresh()
    {
        return !(_deliveryMaxTime == -1 || _deliveryMaxTime - GameManager.Instance.GetCurrentDay() > 0)
            && (_cooldown <= 0 || GameManager.Instance.GetCurrentDay() - _lastTime > _cooldown)
        ;
    }

    /**
     * Triggered on entering new day
     */
    public void Tick()
    {
        UpdateDeliveryTimeLeft();
        _actionCanvas.SetActive(IsAvailable());
        if (HasToRefresh())
        {
            Refresh();
        }
    }

    /**
     * Triggered when player enter the cell
     */
    public void Activate()
    {
        if (IsAvailable())
        {
            if (Consume())
            {
                Debug.Log("Condition met !");
                switch (_type)
                {
                    case MapEntityType.City:
                        break;
                    case MapEntityType.Resource:
                        break;
                    case MapEntityType.Consumer:
                        break;
                    default:
                        break;
                }
                Dispatch();
                _lastTime = GameManager.Instance.GetCurrentDay();
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

    public void Dispatch()
    {
        if (_gainedResource != null) Player.Instance.GainResource(_gainedResource);
    }
}
