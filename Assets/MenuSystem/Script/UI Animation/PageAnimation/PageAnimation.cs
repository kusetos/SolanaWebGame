using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//For this class propery working, the animation clip on the animation component need to be set as Legacy.
//animation clip can be set as Legacy by change the inspector to debug mode and check the animation clip as Legacy.
[RequireComponent(typeof(Animation))]
public class PageAnimation : BasePageAnimation
{
    [SerializeField] private string entryAnimationName;
    [SerializeField] private string exitAnimationName;

    private Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public override void PlayEntryAnimation(UnityAction onAnimationStart, UnityAction onAnimationEnd)
    {
        anim.Play(entryAnimationName);
        onAnimationStart?.Invoke();
        StartCoroutine(WaitAnimationEnd(entryAnimationName, onAnimationEnd));
    }

    public override void PlayExitAnimation(UnityAction onAnimationStart, UnityAction onAnimationEnd)
    {
        anim.Play(exitAnimationName);
        onAnimationStart?.Invoke();
        StartCoroutine(WaitAnimationEnd(exitAnimationName, onAnimationEnd));
    }

    private IEnumerator WaitAnimationEnd(string animationName, UnityAction onAnimationEnd)
    {
        while (anim.IsPlaying(animationName))
        {
            yield return null;
        }
        //Animation Complete
        onAnimationEnd?.Invoke();
    }
}
