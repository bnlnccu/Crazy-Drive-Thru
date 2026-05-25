using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [Header("===== Animation =====")]
    [SerializeField] private Animator anim;

    [Header("===== DLC A: Drag Mode =====")]
    public bool isDragging = false;

    private bool isBeingDestroyed = false;

    private void OnMouseDown()
    {
        if (isBeingDestroyed) return;
        if (OrderManager.Instance != null && OrderManager.Instance.EnableDragMode) return;

        GameState state = OrderManager.Instance.CurrentState;
        bool isDistractor = false;

        if (state == GameState.StateA)
            isDistractor = !CompareTag("Fries");
        else
            isDistractor = !CompareTag("Nugget");

        ScoreManager sm = FindObjectOfType<ScoreManager>();

        if (isDistractor)
            sm.AddScore(5);
        else
            sm.SubtractScore(5);

        Destroy(gameObject);

        // ===== TODO F-3: Add animation before destroy =====
        // var move = GetComponent<MoveToTarget2D>(); if (move != null) move.enabled = false;
        // if (anim != null) { anim.enabled = true; anim.SetTrigger("Toss"); }
        // isBeingDestroyed = true;
        // string action = isDistractor ? "Discarded" : "FalseAlarm";
        // OrderManager.Instance.OnTrialComplete(gameObject.tag, action, isDistractor);
        // OrderManager.Instance.NotifyFoodDestroyed(gameObject);
        // Destroy(gameObject, 0.5f);
    }

    private void OnMouseDrag()
    {
        if (!OrderManager.Instance.EnableDragMode) return;
        if (isBeingDestroyed) return;

        // ===== TODO DLC A-1: isDragging = true + follow mouse =====
        // Camera.main.ScreenToWorldPoint(Input.mousePosition)
    }

    private void OnMouseUp()
    {
        if (!OrderManager.Instance.EnableDragMode) return;
        if (isBeingDestroyed) return;

        // ===== TODO DLC A-2: isDragging = false + Invoke("CheckIfDelivered", 0.1f) =====
    }

    private void CheckIfDelivered()
    {
        if (gameObject == null) return;
        isBeingDestroyed = true;
        ScoreManager sm = FindObjectOfType<ScoreManager>();
        if (sm != null) sm.SubtractScore(5);
        OrderManager.Instance.OnTrialComplete(gameObject.tag, "FalseAlarm", false);
        OrderManager.Instance.NotifyFoodDestroyed(gameObject);
        Destroy(gameObject);
    }
}
