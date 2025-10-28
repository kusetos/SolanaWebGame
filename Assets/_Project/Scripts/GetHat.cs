using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using AnnulusGames.SceneSystem;
using IDosGames;
public class GetHat : MonoBehaviour
{
    public SpriteRenderer image;
    private void Start()
    {
        //ClaimRewardSystem.ClaimCoinReward(1000, 200);
        //ClaimRewardSystem.ClaimTokenReward(1000, 200);
        Debug.Log("asdkfl;jafsldk;j");
        var inventory = IGSUserData.UserInventory;
        if (inventory == null)
        {
            Debug.Log("Инвентарь ещё не загрузился.");
            return;
        }
        bool equipped = false;
        foreach (var item in inventory.Inventory)
        {
            // Check IsEquipped field OR Equipped property (covers both SDK versions)

            var field = item.GetType().GetField("IsEquipped");
            var prop = item.GetType().GetProperty("Equipped");

            if (field != null) equipped = (bool)field.GetValue(item);
            else if (prop != null) equipped = (bool)prop.GetValue(item);
            Debug.Log(field);
            Debug.Log(prop);
            if (equipped && item.ItemClass == "Hat")
                equipped = true;
        }
        // Теперь можно фильтровать через LINQ
        if (equipped){
            
            var hats = inventory.Inventory.Where(i => i.ItemClass == "Hat").ToList();

            foreach (var hat in hats)
            {
                Debug.Log(hat.ToJson());
                Sprite sp = Resources.Load<Sprite>($"Hats/{hat.ItemId}");
                image.sprite = sp;
            }
            Debug.Log("asdkfl;jafsldk;j");
        }
    }

}

