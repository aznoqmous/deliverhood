using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public GameState State;
    
    [Header("Light")]
    [SerializeField] float _nightLightIntensity = 0.2f;
    [SerializeField] float _dayLightIntensity = 1f;
    [SerializeField] public Light2D Light;

    [Header("Time")]
    [SerializeField] float _gameSpeed = 2f;
    public float CurrentTime = 0;
    [SerializeField] float _dayDuration = 16;
    [SerializeField] float _nightDuration = 8;
    [SerializeField] TextMeshProUGUI _tmpCurrentTime;
    [SerializeField] TextMeshProUGUI _tmpCurrentDay;

    [SerializeField] float _taxPeriod = 5;
    float _lastTaxPaidTime = 0;

    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void Play()
    {
        Time.timeScale = _gameSpeed;
    }

    void Start()
    {
        Time.timeScale = _gameSpeed;
        MapManager.Instance.GenerateGrid();
        Player.Instance.EnableInteractableTiles();
        SetState(GameState.Paused);
        //SetState(GameState.Playing);
    }

    public void Reset()
    {
        UIManager.Instance.ShowGameOver(false);
        CurrentTime = 0;
        _lastTaxPaidTime = 0;
        UIManager.Instance.Clear();
        MapManager.Instance.Clear();
        Time.timeScale = _gameSpeed;
        MapManager.Instance.GenerateGrid();
        Player.Instance.Reset();
        Player.Instance.EnableInteractableTiles();
        SetState(GameState.Playing);
    }

    public void StartWave()
    {
        SetState(GameState.Playing);
    }

    void Update()
    {
        Light.pointLightOuterRadius = Mathf.Lerp(Light.pointLightOuterRadius, Player.Instance.GetLevelRadius()/2f, Time.deltaTime);
        switch (State)
        {
            case GameState.Pathing:
                if (Input.GetMouseButtonDown(1))
                {
                    PathManager.ActiveInstance.Clear();
                    PathManager.ActiveInstance.UpdateInteractableTiles();
                }
                break;
            case GameState.Playing:
                Player.Instance.MoveCouriers();
                Player.Instance.EnableInteractableTiles();
                SetTime(CurrentTime + Time.deltaTime);
                if(GetTaxTimeLeft() < 0f)
                {
                    if (!Player.Instance.RemoveCoins(GetTax())) {
                        Pause();
                        UIManager.Instance.ShowGameOver();
                    };
                    _lastTaxPaidTime = CurrentTime;
                    UIManager.Instance.ResetTaxSlider();
                }
                break;
            default:
                break;
        }
        UpdateLight();

    }

    public void SetTime(float value)
    {
        CurrentTime = value;
        _tmpCurrentTime.text = GetHumanReadableHour().ToString();
        _tmpCurrentDay.text = (GetCurrentDay() + 1).ToString();
    }

    public void SetState(GameState state)
    {
        switch (state) {
            case GameState.Pathing:
                Courier.ActiveInstance.PathManager.Clear();
                Courier.ActiveInstance.PathManager.UpdateInteractableTiles();
                break;
            case GameState.Playing:
                Courier.ActiveInstance = null;
                MapManager.Instance.ClearInteractable();
                //Player.Instance.SetTarget(PathManager.Instance.GetCurrentTarget());
                break;
            default:
                break;
        }
        State = state;
    }

    public int GetCurrentDay()
    {
        return Mathf.FloorToInt(CurrentTime / (_dayDuration + _nightDuration));
    }
    public float GetCurrentHour()
    {
        return CurrentTime % (_dayDuration + _nightDuration);
    }
    public string GetHumanReadableHour()
    {
        float currentHour = GetCurrentHour();
        int hours = Mathf.FloorToInt(currentHour);
        float minutes = Mathf.RoundToInt(currentHour % 1 * 60);
        return ((hours + 6) % 24).ToString("00") + "h" + minutes.ToString("00");
    }
    public float DaysToTime(float days) {
        return days * (_dayDuration + _nightDuration);
    }
    public float TimeToDays(float time)
    {
        return time / (_dayDuration + _nightDuration);
    }
    public void UpdateLight()
    {
        if (IsDayLight())
        {
            Light.intensity = Mathf.Lerp(Light.intensity, _dayLightIntensity, Time.deltaTime);
        }
        else
        {
            Light.intensity = Mathf.Lerp(Light.intensity, _nightLightIntensity, Time.deltaTime);
        }

    }
    public bool IsDayLight()
    {
        return GetCurrentHour() < _dayDuration;
    }

    public int GetTax()
    {
        return 5 + Mathf.FloorToInt(GetCurrentDay() / _taxPeriod);
    }
    public float GetTaxTimeLeft()
    {
        return DaysToTime(_taxPeriod) - (CurrentTime - _lastTaxPaidTime);
    }
    public float GetTaxRatioLeft()
    {
        return GetTaxTimeLeft() / DaysToTime(_taxPeriod);
    }
}

public enum GameState
{
    Pathing,
    Playing,
    Paused
}