using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] List<GameObject> _pages = new List<GameObject>();
    [SerializeField] int _currentIndex = 0;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void NextPage()
    {
        _pages[_currentIndex].SetActive(false);
        _currentIndex++;
        if (_currentIndex >= _pages.Count) Close();
        _pages[_currentIndex].SetActive(true);
    }

    public void PreviousPage()
    {
        _pages[_currentIndex].SetActive(false);
        _currentIndex = Mathf.Max(0, _currentIndex- 1);
        _pages[_currentIndex].SetActive(true);
        if (_currentIndex >= _pages.Count) Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        GameManager.Instance.SetState(GameState.Playing);
    }
}
