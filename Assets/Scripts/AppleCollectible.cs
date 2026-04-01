using UnityEngine;

public class AppleCollectible : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private int points = 1;

    private bool collected;

    private void Awake()
    {
        Debug.Log($"[Apple] Awake on {name}", this);

        var col2d = GetComponent<Collider2D>();
        var rb2d  = GetComponent<Rigidbody2D>();

        Debug.Log($"[Apple] Collider2D={(col2d ? col2d.GetType().Name : "MISSING")} IsTrigger={(col2d ? col2d.isTrigger : false)}", this);
        Debug.Log($"[Apple] Rigidbody2D={(rb2d ? rb2d.bodyType.ToString() : "MISSING")}", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        bool isPlayer = other.CompareTag(playerTag) || other.transform.root.CompareTag(playerTag);
        Debug.Log($"[Apple] Trigger with {other.name} tag={other.tag} rootTag={other.transform.root.tag} isPlayer={isPlayer}", this);

        if (!isPlayer) return;

        collected = true;
        Debug.Log("[Apple] Collected -> adding score and destroying", this);

        // Add score via HUD (no GameManager needed)
        var hud = FindFirstObjectByType<HUDScoreUI>();
        if (hud != null)
        {
            hud.AddScore(points);
            Debug.Log($"[Apple] Added {points}. HUD Score now {hud.Score}", this);
        }
        else
        {
            Debug.LogWarning("[Apple] HUDScoreUI not found in scene. Score did NOT increase.", this);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogWarning($"[Apple] OnCollisionEnter2D fired. This usually means IsTrigger is OFF. Hit {collision.collider.name}", this);
    }
}