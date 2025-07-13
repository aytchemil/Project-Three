using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public static class WaitExtension 
{
   public static void Wait(this MonoBehaviour mono, float delay, UnityAction callback)
    {
        mono.StartCoroutine(ExecuteAction(delay, callback));
    }

    private static IEnumerator ExecuteAction(float delay, UnityAction callback)
    {
        yield return new WaitForSecondsRealtime(delay);
        callback?.Invoke();
        yield break;
    }
}
