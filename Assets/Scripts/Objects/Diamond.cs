using UnityEngine;

public class Diamond : MonoBehaviour, IClickable
{
    [SerializeField] private ParticleSystem ambientParticles;
    [SerializeField] private ParticleSystem pickupEffect;
    [SerializeField] private ParticleSystem PlusOneEffect;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider diamondCollider;
   // [SerializeField] private SpriteRenderer spriteRenderer;


    public void OnClicked()
    {
        if (!GameManager.Instance.IsInState(GameState.Running)) return;

        Collect();

        Debug.Log("Diamond Clicked");
    }

    private void Collect()
    {
        AudioManager.Instance.PlayDiamondPickup(); //Play Sfx
        
        //Disable individual components so that pickup Burst effect can be played before destroying the whole object.
        ambientParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        pickupEffect.Play();
        PlusOneEffect.Play();

        diamondCollider.enabled = false;
        //spriteRenderer.enabled = false;
        animator.enabled = false;

        GameManager.Instance.CollectDiamond(); // Update Diamond count in Game Manager.
        Destroy(gameObject, pickupEffect.main.duration);
    }
}
