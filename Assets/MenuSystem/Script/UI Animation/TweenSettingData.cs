using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TweenSettingData
{
    public TweenTypes TweenType;
    public bool UsingCustomValue;
    public Vector3 From;
    public Vector3 To;
    public TweenDirectionType TweenDirection;
    public TweenModeType TweenMode;
    public Ease TweenEase;
    public float Duration;
    public float StartDelay;
    public UnityEvent OnTweenStart;
    public UnityEvent OnTweenComplete;
}

public enum TweenTypes
{
    None, Slide, Scale, ScaleX, ScaleY, Fade
}

public enum TweenDirectionType
{
    UP, RIGHT, LEFT, DOWN
}

public enum TweenModeType
{
    IN, OUT
}