using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
#pragma warning disable 0649

public enum GameTimeSpeed
{
    Normal,
    X2,
    X4,
    X8,
        
    Count
}

public class GameTimeManager : MonoSingleton<GameTimeManager>
{
    [SerializeField] 
    private int minutesPerSecond = 1;
    
    [SerializeField]
    private int startMinute = 0;
    
    [SerializeField]
    private int startHour = 0;
    
    [SerializeField]
    private int startDay = 1;
    
    [SerializeField]
    private GameTimeSpeed startGameSpeed = GameTimeSpeed.Normal;
    
    //[SerializeField]
    //private GameTime currentGameTime;
    //public GameTime GameTime => currentGameTime;

    [ReadOnly]
    private GameTimeSpeed currentGameTimeSpeed = GameTimeSpeed.Normal;
    
    private readonly Dictionary<GameTimeSpeed, float> gameTimeSpeedFactors = new Dictionary<GameTimeSpeed, float>()
    {
        {GameTimeSpeed.Normal, 1f},
        {GameTimeSpeed.X2, 2f},
        {GameTimeSpeed.X4, 4f},
        {GameTimeSpeed.X8, 8f}
    };

    private float cumDeltaTime;
    [SerializeField]
    //private BoolVariable isPaused;
    //public bool GameIsPaused => isPaused.Value;

    public static event Action<int> OnMinutePassed = delegate { };
    public static event Action<int> OnHourPassed = delegate { };
    public static event Action<int> OnDayChange = delegate { };
    
    public static event Action<float> OnGameSpeedChanged = delegate {  };
    public static event Action<bool> OnGamePauseToggle = delegate {  };
    
    
    [SerializeField]
    [ReadOnly]
    private float dayClockValueNormalized;
    public float DayClockValueNormalized => dayClockValueNormalized;


    //public float DeltaTime => GameIsPaused ? 0f : Time.deltaTime * gameTimeSpeedFactors[currentGameTimeSpeed];
    public float DeltaTime;


    public void Init()
    {
        //currentGameTime = new GameTime(startMinute, startHour, startDay);
        ChangeGameTimeSpeed((int)startGameSpeed, true);
        CheckChangesAndSendEvents(true);
    }


    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Plus) || Input.GetKeyUp(KeyCode.KeypadPlus))
        {
            ChangeGameTimeSpeed(1);
        }
        
        if (Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.KeypadMinus))
        {
            ChangeGameTimeSpeed(-1);
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //TogglePause();
        }
        
        
        //cumDeltaTime += Time.deltaTime * gameTimeSpeedFactors[currentGameTimeSpeed] * (isPaused.Value ? 0 : 1);

        if (cumDeltaTime >= 1f)
        {
            //currentGameTime.AddMinute(minutesPerSecond);
            //OnMinutePassed(currentGameTime.Minutes);
            CheckChangesAndSendEvents();
        
            cumDeltaTime = 1f - cumDeltaTime;
        }

        //dayClockValueNormalized = currentGameTime.Hour / 24f + (currentGameTime.Minutes + cumDeltaTime * minutesPerSecond) / 1440f;
    }

    public void CheckChangesAndSendEvents(bool force = false)
    {
    //    if (force)
    //        OnMinutePassed(currentGameTime.Minutes);
        
    //    if (currentGameTime.Minutes == 0 || force)
    //        OnHourPassed(currentGameTime.Hour);
        
    //    if (currentGameTime.Hour == 0 || force)
    //    {
    //        OnDayChange(currentGameTime.Day);
    //    }
    }

    public void ChangeGameTimeSpeed(int v, bool force = false)
    {
        GameTimeSpeed newSpeed = (GameTimeSpeed)Mathf.Clamp((int)currentGameTimeSpeed + v,
            (int)GameTimeSpeed.Normal,
            (int)GameTimeSpeed.Count - 1);
        if (newSpeed != currentGameTimeSpeed || force)
        {
            currentGameTimeSpeed = newSpeed;
            OnGameSpeedChanged(gameTimeSpeedFactors[currentGameTimeSpeed]);
        }
    }

    //public void TogglePause(bool overwriteValue = false, bool value = false)
    //{
    //    if (overwriteValue)
    //        isPaused.Value = value;
    //    else
    //        isPaused.Value = !isPaused.Value;
    //    OnGamePauseToggle(isPaused);
    //}
}



