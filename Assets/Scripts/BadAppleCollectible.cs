using UnityEngine;
using UnityEngine.SceneManagement;

public class BadAppleCollectible : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private bool triggered;

    private void Awake()
    {
        Debug.Log($"[BadApple] Awake on {name}", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        bool isPlayer = other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag);
        Debug.Log($"[BadApple] Trigger with {other.name} tag={other.tag} rootTag={other.transform.root.tag} isPlayer={isPlayer}", this);

        if (!isPlayer) return;

        triggered = true;
        Debug.Log("[BadApple] Player hit bad apple -> resetting level!", this);

        // Reload current scene (full reset)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}