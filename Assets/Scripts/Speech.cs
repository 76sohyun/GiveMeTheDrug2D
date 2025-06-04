using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speech : MonoBehaviour
{
    [SerializeField] private TMP_Text TargetText;
    [SerializeField] private string fullText;
    [SerializeField] private float delay;
    private void OnEnable()
    {
        TargetText.text = "";
        StartCoroutine(TextCoroutine());
    }

    private IEnumerator TextCoroutine()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            TargetText.text += fullText[i];
            yield return new WaitForSeconds(delay);
        }
    }
}
