using System.Collections.Generic;
using UnityEngine;
using System;

public class DelayAction : MonoBehaviour
{
    public static IEnumerator<WaitForSeconds> Delay(float second, Action action)
	{
		yield return new WaitForSeconds(second);
		action?.Invoke();
	}
}
