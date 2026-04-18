using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// ちょっと待ってから消える
/// </summary>
public class DeactivateAfterDelay : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 0.2f;
    private CancellationTokenSource cancellationTokenSource;

    private void OnEnable()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        DeactivateAfterDelayAsync(cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        cancellationTokenSource?.Cancel();
    }

    private async UniTask DeactivateAfterDelayAsync(CancellationToken cancellationToken)
    {
        await UniTask.Delay((int)(delaySeconds * 1000), cancellationToken: cancellationToken);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        cancellationTokenSource?.Dispose();
    }
}