using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
/// <summary>
/// ちょっとアニメーションを遅らせたいときに使用
/// 今回はRareFishのエフェクトで使用している
/// </summary>
public class DelayedAnimation : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 2f;
    private Animator animator;
    private CancellationTokenSource cancellationTokenSource;
    
    [SerializeField] private GameObject[] disableObjects;

    private void OnEnable()
    {
        // 前のタスクをキャンセル
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();

        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
            EnableAnimatorAfterDelayAsync(cancellationTokenSource.Token).Forget();
        }
    }

    private void OnDisable()
    {
        if(disableObjects != null)
        {
            foreach (var obj in disableObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    private async UniTask EnableAnimatorAfterDelayAsync(CancellationToken cancellationToken)
    {
        await UniTask.Delay((int)(delaySeconds * 1000), cancellationToken: cancellationToken);
        if (animator != null)
        {
            animator.enabled = true;
        }
    }

    private void OnDestroy()
    {
        cancellationTokenSource?.Dispose();
    }
}