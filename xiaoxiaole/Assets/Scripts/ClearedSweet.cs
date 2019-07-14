using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearedSweet : MonoBehaviour
{
    public AnimationClip clearAnimation;

    private bool isClear;

    public bool IsClear
    {
        get
        {
            return isClear;
        }
    }

    protected GameSweet sweet;

    public virtual void Clear()
    {
        isClear = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if(animator!=null)
        {
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);
        }
    }
}
