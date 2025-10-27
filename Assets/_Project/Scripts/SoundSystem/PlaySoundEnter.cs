using UnityEngine;

    public class PlaySoundEnter : StateMachineBehaviour
    {
        [SerializeField] private string soundName;
    [SerializeField, Range(0, 1)] private float volume = 1;
        public bool stopOnExit = false;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SoundManager.Instance.Play(soundName);     
        }
    }