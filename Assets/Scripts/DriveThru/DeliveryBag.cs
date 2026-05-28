using UnityEngine;
using System.Collections;

public class DeliveryBag : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        FoodItem food = other.GetComponent<FoodItem>();
        if (food == null) return;
        if (food.isDragging) return;

        var move = other.GetComponent<MoveToTarget2D>();
        if (move != null) move.enabled = false;

        // =============================================================
        // TODO D-1: Use CompareTag to check against current state
        // =============================================================
        // GameState state = OrderManager.Instance.CurrentState;
        // bool correct = false;
        //
        // if (state == GameState.StateA)
        //     correct = other.CompareTag("Fries");
        // else
        //     correct = other.CompareTag("Nugget");
        //
        // ScoreManager sm = FindObjectOfType<ScoreManager>();
        // if (correct)
        //     sm.AddScore(10);
        // else
        //     sm.SubtractScore(5);
        //
        // OrderManager.Instance.OnTrialComplete(other.gameObject.tag, "Delivered", correct);
        // OrderManager.Instance.NotifyFoodDestroyed(other.gameObject);

        StartCoroutine(FallIntoBag(other.gameObject));
    }

    private IEnumerator FallIntoBag(GameObject food)
    {
        var rb = food.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        Vector3 startPos = food.transform.position;
        Vector3 bagCenter = transform.position;
        Vector3 aboveBag = new Vector3(bagCenter.x, startPos.y, startPos.z);

        float slideTime = 0.15f;
        float elapsed = 0f;
        while (elapsed < slideTime && food != null)
        {
            elapsed += Time.deltaTime;
            food.transform.position = Vector3.Lerp(startPos, aboveBag, elapsed / slideTime);
            yield return null;
        }

        if (food == null) yield break;
        Vector3 dropStart = food.transform.position;
        Vector3 dropEnd = new Vector3(bagCenter.x, bagCenter.y - 0.3f, startPos.z);
        Vector3 startScale = food.transform.localScale;

        float dropTime = 0.25f;
        elapsed = 0f;
        while (elapsed < dropTime && food != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dropTime;
            food.transform.position = Vector3.Lerp(dropStart, dropEnd, t);
            food.transform.localScale = Vector3.Lerp(startScale, startScale * 0.3f, t);
            yield return null;
        }

        if (food != null) Destroy(food);
    }
}
