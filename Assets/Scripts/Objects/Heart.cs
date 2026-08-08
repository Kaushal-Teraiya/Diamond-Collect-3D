using System.Collections;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] private SpriteRenderer heartRenderer;

    public void BlinkNDisable()
    {
        StartCoroutine(BlinkHeart());
    }

    private IEnumerator BlinkHeart()
    {
        Color color = heartRenderer.color;

        for (int i = 0; i < 3; i++)
        {
            color.a = 0.2f;
            heartRenderer.color = color;

            yield return new WaitForSeconds(0.15f);

            color.a = 1f;
            heartRenderer.color = color;

            yield return new WaitForSeconds(0.15f);
        }

        gameObject.SetActive(false);
    }
}

