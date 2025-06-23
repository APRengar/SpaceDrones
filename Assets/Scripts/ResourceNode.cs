using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [Header("Resource Settings")]
    [SerializeField] private float gatherTime = 2f;

    private bool isBeingCollected = false;

    public bool IsAvailable => !isBeingCollected;

    public void StartCollecting(System.Action onCollectedCallback)
    {
        if (isBeingCollected) return;

        isBeingCollected = true;
        StartCoroutine(CollectCoroutine(onCollectedCallback));
    }

    private IEnumerator CollectCoroutine(System.Action onCollectedCallback)
    {
        yield return new WaitForSeconds(gatherTime);

        onCollectedCallback?.Invoke();
        Destroy(gameObject); // Удаляем ресурс после сбора
    }

    private void OnDestroy()
    {
        StopAllCoroutines(); // на случай, если удалён вручную
        // Чистим флаги на случай, если кто-то ещё на него ссылался
        isBeingCollected = false;
    }
}
