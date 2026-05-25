using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        FoodItem food = other.GetComponent<FoodItem>();
        if (food == null) return;

        // Determine if this was a correct food that the player missed
        GameState state = OrderManager.Instance.CurrentState;
        bool wasCorrectFood = false;

        if (state == GameState.StateA)
            wasCorrectFood = other.CompareTag("Fries");
        else
            wasCorrectFood = other.CompareTag("Nugget");

        if (wasCorrectFood)
        {
            // Miss: correct food slipped past = penalty
            ScoreManager sm = FindObjectOfType<ScoreManager>();
            if (sm != null) sm.SubtractScore(5);
            OrderManager.Instance.OnTrialComplete(other.gameObject.tag, "Miss", false);
        }
        else
        {
            // Distractor slipped past = no penalty, just clean up
            OrderManager.Instance.OnTrialComplete(other.gameObject.tag, "Miss", true);
        }

        OrderManager.Instance.NotifyFoodDestroyed(other.gameObject);
        Destroy(other.gameObject);
    }
}
