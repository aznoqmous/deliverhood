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
    public float CurrentTime = 0;
    [SerializeField] float _dayDuration = 16;
    [SerializeField] float _nightDuration = 8;
    [Header("Light")]
    [SerializeField] float _nightLightIntensity = 0.2f;
    [SerializeField] float _dayLightIntensity = 1f;
    [SerializeField] public Light2D Light;
    [SerializeField] float _gameSpeed = 2f;

    [SerializeField] TextMeshProUGUI _tmpCurrentTime;

    [SerializeField] TextMeshProUGUI _tmpCurrentDay;

    void Start()
    {
        Time.timeScale = _gameSpeed;
        MapManager.Instance.GenerateGrid();
        MapManager.Instance.GenerateMapEntities();
        Player.Instance.transform.position = MapManager.Instance.GetStartPosition();
        PathManager.Instance.UpdateInteractableTiles();
    }

    public void StartWave()
    {
       
        if(GameState.Playing == State) PathManager.Instance.ClearAfterIndex();
        else SetState(GameState.Playing);
    }

    void Update()
    {
        switch (State)
        {
            case GameState.Pathing:
                if (Input.GetMouseButtonDown(1))
                {
                    PathManager.Instance.Clear();
                    PathManager.Instance.UpdateInteractableTiles();
                }
                break;
            case GameState.Playing:
                Player.Instance.MoveTowardTarget();
                SetTime(CurrentTime + Time.deltaTime);
                break;
            default:
                break;
        }
        UpdateLight();

    }

    public void Tick()
    {
        MapManager.Instance.TickMapEntities();
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
                PathManager.Instance.Clear();
                PathManager.Instance.UpdateInteractableTiles();
                break;
            case GameState.Playing:
                MapManager.Instance.ClearInteractable();
                Player.Instance.SetTarget(PathManager.Instance.GetCurrentTarget());
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
}

public enum GameState
{
    Pathing,
    Playing
}