using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleableActive : MonoBehaviour
{
    //this is used by buttons to toggle a thing being active
    public void ToggleActive() => gameObject.SetActive(!gameObject.activeSelf);
}
