using IDosGames;
using UnityEngine;

public class FirstMoney : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int tokenReward = 50;   // amount of memcoin reward
    public int eventPoints = 1;   
    void Start()
    {
        SoundManager.Instance.Play("cave");
        ClaimRewardSystem.ClaimTokenReward(tokenReward, eventPoints);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
