using UnityEngine;

public class CharacterStateManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterAIController character;

    public static CharacterStateManager Instance;

    public Transform target;
    private CharacterState state;
    private void Start()
    {

        if (Instance == null)
            Instance = this;

        state = CharacterState.Chais;
        character.ChaseTarget(target);
    }
    public void SetCharacterState(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Wait:
                Debug.Log("STOP MOEVEMENT + IDLE");
                character.Stop();   
                break;
            case CharacterState.Chais:
                Debug.Log("GOOO MOEVEMENT + MOVE");
                character.ChaseTarget(target);
                break;
        }
        //this.state = state;
    }
}

public enum CharacterState
{
    Wait,
    Chais,
}