using System.Collections;
using Solana.Unity.Soar.Accounts;
using UnityEngine;

public class CallMover : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public WaypointMover mover;
    public GameObject ob;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            mover.GoToNextWaypoint();
            StartCoroutine(NextLevelAfterTime());
        }

    }
    
    IEnumerator NextLevelAfterTime()
    {
        yield return new WaitForSeconds(1f);
        FadePanel.Instance.FadeIn(() =>
        {
            ob.SetActive(true);
            SoundManager.Instance.Play("haha");
        });
    }
}
