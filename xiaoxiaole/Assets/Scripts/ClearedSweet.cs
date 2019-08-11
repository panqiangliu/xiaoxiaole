using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearedSweet : MonoBehaviour
{
    public AnimationClip clearAnimation;

    private bool isClear;
    public AudioClip destoryAudio;

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
            GameManager.Instance.playerScore++;
            AudioSource.PlayClipAtPoint(destoryAudio, transform.position);
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);
        }
    }
}
