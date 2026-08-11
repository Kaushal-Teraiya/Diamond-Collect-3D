using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [SerializeField] private SpriteRenderer heartRenderer;
    [SerializeField] private Image heartImage;

    public void BlinkNDisable()
    {
        Debug.Log("BLINKING HEART: " + gameObject.name);
        StartCoroutine(BlinkHeart());
    }

    private IEnumerator BlinkHeart()
    {
        Color color;

        if (heartImage != null)
            color = heartImage.color;
        else
            color = heartRenderer.color;

        for (int i = 0; i < 3; i++)
        {
            color.a = 0.2f;

            if (heartImage != null)
                heartImage.color = color;
            else
                heartRenderer.color = color;

            yield return new WaitForSeconds(0.15f);

            color.a = 1f;

            if (heartImage != null)
                heartImage.color = color;
            else
                heartRenderer.color = color;

            yield return new WaitForSeconds(0.15f);
        }

        gameObject.SetActive(false);
    }
}