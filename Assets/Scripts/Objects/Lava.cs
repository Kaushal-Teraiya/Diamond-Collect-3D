using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour, IClickable
{
    [SerializeField] private List<Animator> animators = new();
    [SerializeField] private List<SpriteRenderer> bubbleRenderers = new();

    void Awake()
    {
        for (int i = 0; i < bubbleRenderers.Count; i++)
        {
            bubbleRenderers[i].enabled = false;
            animators[i].enabled = false;
        }
    }
    public void OnClicked()
    {
        if (!GameManager.Instance.IsInState(GameState.Running)) return;

        DamagePlayer();

        Debug.Log("Clicked Lava");
    }

    private void DamagePlayer()
    {
        //Play lava damage sfx
        AudioManager.Instance.PlayWrongClick();
        AudioManager.Instance.PlayLavaDamage();

        StartCoroutine(HandleLavaBubbleLifetime());
        FindFirstObjectByType<UIManager>().ShowFireBorder();
        GameManager.Instance.TakeDamage();
    }

    private IEnumerator HandleLavaBubbleLifetime()
    {
        EnableLavaBubbles();
        yield return new WaitForSeconds(3f);
        DisableLavaBubbles();
    }
    private void EnableLavaBubbles()
    {
        for (int i = 0; i < bubbleRenderers.Count; i++)
        {
            bubbleRenderers[i].enabled = true;
            animators[i].enabled = true;
        }
    }
    private void DisableLavaBubbles()
    {
        for (int i = 0; i < bubbleRenderers.Count; i++)
        {
            bubbleRenderers[i].enabled = false;
            animators[i].enabled = false;
        }
    }
}
