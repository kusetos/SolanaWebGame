using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class PageAnimator : BasePageAnimation
{
    [SerializeField] private bool hasIdleAnimation;

    private Animator animator;
    private bool isAnimationRunning;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void PlayEntryAnimation(UnityAction onAnimationStart, UnityAction onAnimationEnd)
    {
        if (isAnimationRunning) return;

        isAnimationRunning = true;
        animator.SetBool("entry", true);
        onAnimationStart?.Invoke();

        StartCoroutine(WaitAnimationEnd("entry", onAnimationEnd));
    }

    public override void PlayExitAnimation(UnityAction onAnimationStart, UnityAction onAnimationEnd)
    {
        if (isAnimationRunning) return;

        isAnimationRunning = true;
        animator.SetBool("exit", true);
        onAnimationStart?.Invoke();

        StartCoroutine(WaitAnimationEnd("exit", onAnimationEnd));
    }

    private IEnumerator WaitAnimationEnd(string animationName, UnityAction onAnimationEnd)
    {
        float counter = 0;
        float waitTime = animator.GetCurrentAnimatorStateInfo(0).length;

        //Wait until the current state is done playing
        while (counter < waitTime)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Animation Done playing. Do something below!
        onAnimationEnd?.Invoke();
        isAnimationRunning = false;
        animator.SetBool(animationName, false);
        if (hasIdleAnimation && animationName == "entry")
        {
            animator.SetBool("idle", true);
        }
        else if(hasIdleAnimation)
        {
            animator.SetBool("idle", false);
        }
    }
}
