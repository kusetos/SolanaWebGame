using UnityEngine;
using UnityEngine.Events;

public abstract class BasePageAnimation : MonoBehaviour
{
    public abstract void PlayEntryAnimation(UnityAction onAnimationStart, UnityAction onAnimationEnd);
    public abstract void PlayExitAnimation(UnityAction onAnimationStart, UnityAction onAnimationEnd);
}
