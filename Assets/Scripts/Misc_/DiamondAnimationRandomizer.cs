using UnityEngine;

public class DiamondAnimationController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        animator.Play(0, 0, UnityEngine.Random.value);
    }
}
