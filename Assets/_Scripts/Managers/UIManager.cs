using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] ResourceUI _resourceUIPrefab;
    [SerializeField] GameObject _resourcesContainer;
    [SerializeField] Dictionary<ScriptableResource, ResourceUI> _resources = new Dictionary<ScriptableResource, ResourceUI>();
    [SerializeField] StatusTextUI _statuxTextPrefab;

    public void UpdateResource(ScriptableResource resource, int amount)
    {
        _resources.TryGetValue(resource, out ResourceUI resourceUI);
        if(resourceUI == null)
        {
            resourceUI = Instantiate(_resourceUIPrefab, _resourcesContainer.transform);
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
}
