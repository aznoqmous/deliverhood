using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _countTmp;

    public void SetCount(int amount) {
        _countTmp.text = amount.ToString();
    }

    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }
}
