using UnityEngine;

public class Diamond : MonoBehaviour, IClickable
{
    [SerializeField] private ParticleSystem ambientParticles;
    [SerializeField] private ParticleSystem pickupEffect;
    [SerializeField] private ParticleSystem PlusOneEffect;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider diamondCollider;

    bool collected;

    public void OnClicked()
    {
        if (!GameManager.Instance.IsInState(GameState.Running)) return;

        Collect();
        Debug.Log("Diamond Clicked");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER COLLECTING DIAMOND");
            Collect();
        }
    }

    private void Collect()
    {
        if (collected) return;
        collected = true;

        AudioManager.Instance.PlayDiamondPickup();

        if (ambientParticles != null)
            ambientParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (pickupEffect != null)
            pickupEffect.Play();

        if (PlusOneEffect != null)
            PlusOneEffect.Play();

        if (diamondCollider != null)
            diamondCollider.enabled = false;

        if (animator != null)
            animator.enabled = false;

        GameManager.Instance.CollectDiamond();

        float destroyTime = pickupEffect != null ? pickupEffect.main.duration : 0f;
        Destroy(gameObject, destroyTime);
    }
}