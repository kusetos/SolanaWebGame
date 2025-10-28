using UnityEngine;
using IDosGames;   // SDK namespace
using System.Linq; // for LINQ methods like .FirstOrDefault()

public class HatManager : MonoBehaviour
{
    [Header("Player Hat Setup")]
    public Transform hatAttachPoint; // Assign in Inspector
    public GameObject hatPrefab;     // Optional fallback hat prefab
    private GameObject currentHat;

    private void Start()
    {
        TryLoadPlayerHat();
    }

    private void TryLoadPlayerHat()
    {
        // ✅ Step 1: Ensure player is logged in
        // if (!IGSUserData.IsAuthorized)
        // {
        //     Debug.LogWarning("❌ Player not logged in. Cannot load hat.");
        //     return;
        // }

        // ✅ Step 2: Get the player's inventory
        var inventory = IGSUserData.UserInventory;
        if (inventory == null || inventory.Inventory == null)
        {
            Debug.LogWarning("⚠️ Inventory is empty or not loaded yet.");
            return;
        }

        // ✅ Step 3: Check if player owns 'hat_001'
        var hatItem = inventory.Inventory.FirstOrDefault(i => i.ItemId == "hat_001");
        if (hatItem == null)
        {
            Debug.Log("🧢 Player doesn't have hat_001.");
            return;
        }

        Debug.Log("🎩 Player owns hat_001. Loading...");

        // ✅ Step 4: Get the hat sprite from item data (CustomData or Image URL)
        //string imageUrl = hatItem.ImageUrl; // iDos stores this if set in LiveOps

        // if (!string.IsNullOrEmpty(imageUrl))
        //     StartCoroutine(LoadAndAttachHat(imageUrl));
        // else if (hatPrefab != null)
        //     InstantiateHatPrefab();
        // else
        //     Debug.LogWarning("No hat image or prefab found for hat_001.");
    }

    private System.Collections.IEnumerator LoadAndAttachHat(string imageUrl)
    {
        Debug.Log($"📦 Loading hat sprite from {imageUrl}");
        using (var www = new UnityEngine.Networking.UnityWebRequest(imageUrl))
        {
            www.downloadHandler = new UnityEngine.Networking.DownloadHandlerTexture();
            yield return www.SendWebRequest();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Failed to load hat image: " + www.error);
                yield break;
            }

            Texture2D tex = ((UnityEngine.Networking.DownloadHandlerTexture)www.downloadHandler).texture;
            var hatSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            // ✅ Step 5: Create hat object and attach
            var hatObj = new GameObject("PlayerHat");
            var sr = hatObj.AddComponent<SpriteRenderer>();
            sr.sprite = hatSprite;

            hatObj.transform.SetParent(hatAttachPoint, false);
            hatObj.transform.localPosition = Vector3.zero;
            currentHat = hatObj;

            Debug.Log("✅ Hat successfully attached to player!");
        }
    }

    private void InstantiateHatPrefab()
    {
        currentHat = Instantiate(hatPrefab, hatAttachPoint.position, Quaternion.identity, hatAttachPoint);
        Debug.Log("✅ Default hat prefab attached!");
    }
}
