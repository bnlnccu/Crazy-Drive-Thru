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

        // ===== TODO F-1: Destroy(gameObject); =====
        // Hint: just one line to make the food disappear on click

        // ===== TODO F-2: Replace F-1 with judgment logic =====
        // Use OrderManager.Instance.CurrentState + CompareTag() to determine:
        //   StateA -> Fries is correct, Nugget is distractor
        //   StateB -> Nugget is correct, Fries is distractor
        // Distractor: FindObjectOfType<ScoreManager>().AddScore(5)
        // Correct food (False Alarm): FindObjectOfType<ScoreManager>().SubtractScore(5)

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
