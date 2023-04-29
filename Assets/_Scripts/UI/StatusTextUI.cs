using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusTextUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _tmp;
    [SerializeField] Image _image;

    public void SetText(string text)
    {
        _tmp.text = text;
    }

    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
        _image.gameObject.SetActive(sprite != null);
    }

    public IEnumerator DestroyAfter(float seconds=1)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
