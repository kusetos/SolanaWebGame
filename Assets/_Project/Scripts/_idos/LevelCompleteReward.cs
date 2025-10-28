using UnityEngine;
using IDosGames;
using UnityEngine.Rendering;
using System.Collections;
using System;  // <- for ClaimRewardSystem

public class LevelCompleteReward : MonoBehaviour
{

        public int tokenReward = 50;   // amount of memcoin reward
        public int eventPoints = 1;    // optional, for event system tracking

    bool flag = true;
    // private IEnumerator Start()
    // {
    //     // Wait until SDK is fully initialized
    //     yield return new WaitUntil(() => IGSUserData.Currency != null);

    //     Debug.Log("SDK Ready – giving reward...");
    //     Debug.Log("✅ Player received hat_001!");
    // }

    private void GiveReward()
    {
        try
        {
            ClaimRewardSystem.ClaimTokenReward(tokenReward, eventPoints);
            Debug.Log($"✅ Player rewarded {tokenReward} memcoins!");
            Debug.Log("Current token balance: " + IGSUserData.Currency.CurrencyData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Reward failed: " + ex.Message);
        }
    }
}
