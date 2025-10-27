using UnityEngine;

    public class PlaySoundExit : StateMachineBehaviour
    {
        [SerializeField] private string soundName;
        [SerializeField, Range(0, 1)] private float volume = 1;
        public bool stopOnExit;
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stopOnExit)
                SoundManager.Instance.Stop(soundName);
        }   
    }
