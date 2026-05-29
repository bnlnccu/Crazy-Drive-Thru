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

        Destroy(gameObject);
        OrderManager.Instance.OnTrialComplete(gameObject.tag, "Discarded", true);
        OrderManager.Instance.NotifyFoodDestroyed(gameObject);

        // =============================================================
        // TODO F-2: Add this block ABOVE the F-1 lines (don't delete F-1)
        // =============================================================
        // GameState state = OrderManager.Instance.CurrentState;
        // bool isDistractor = false;
        //
        // if (state == GameState.StateA)
        //     isDistractor = !CompareTag("Fries");
        // else
        //     isDistractor = !CompareTag("Nugget");
        //
        // ScoreManager sm = FindObjectOfType<ScoreManager>();
        //
        // if (isDistractor)
        //     sm.AddScore(5);
        // else
        //     sm.SubtractScore(5);

        // =============================================================
        // TODO F-3: Replace the F-1 "Destroy(gameObject);" with this block
        // =============================================================
        // var move = GetComponent<MoveToTarget2D>();
        // if (move != null) move.enabled = false;
        //
        // if (anim != null)
        // {
        //     anim.enabled = true;
        //     anim.SetTrigger("Toss");
        // }
        // isBeingDestroyed = true;
        // Destroy(gameObject, 0.5f);
    }

    private void OnMouseDrag()
    {
        if (!OrderManager.Instance.EnableDragMode) return;
        if (isBeingDestroyed) return;

        // =============================================================
        // TODO DLC A-1: Drag food with mouse + stop conveyor
        // =============================================================
        // isDragging = true;
        // var move = GetComponent<MoveToTarget2D>();
        // if (move != null) move.enabled = false;
        // Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
    }

    private void OnMouseUp()
    {
        if (!OrderManager.Instance.EnableDragMode) return;
        if (isBeingDestroyed) return;

        // =============================================================
        // TODO DLC A-2: Release = discard (same judgment as click mode)
        // =============================================================
        // isDragging = false;
        // isBeingDestroyed = true;
        //
        // GameState state = OrderManager.Instance.CurrentState;
        // bool isDistractor = false;
        //
        // if (state == GameState.StateA)
        //     isDistractor = !CompareTag("Fries");
        // else
        //     isDistractor = !CompareTag("Nugget");
        //
        // ScoreManager sm = FindObjectOfType<ScoreManager>();
        // if (isDistractor)
        //     sm.AddScore(5);
        // else
        //     sm.SubtractScore(5);
        //
        // string action = isDistractor ? "Discarded" : "FalseAlarm";
        // OrderManager.Instance.OnTrialComplete(gameObject.tag, action, isDistractor);
        // OrderManager.Instance.NotifyFoodDestroyed(gameObject);
        // Destroy(gameObject);
    }
}
