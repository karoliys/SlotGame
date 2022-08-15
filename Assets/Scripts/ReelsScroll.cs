using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ReelsScroll : MonoBehaviour
{
    [SerializeField] private RectTransform[] reels;
    [SerializeField] private float delayStep;
    [SerializeField] private Ease startEase;
    [SerializeField] private Button playButton;
    [SerializeField] private int countRotation;
    [SerializeField] private float timeRotation;
    [SerializeField] private float symbolSize;
    [SerializeField] private int symbolCount;
    private Dictionary<int, SymbolsMove> symbolsOnReel = new Dictionary<int, SymbolsMove>();

    private void Start()
    {
        SymbolsMove reel = reels[0].GetComponent<SymbolsMove>();

        symbolSize = reel.GetSymbolSize();
        symbolCount = reel.GetCountSymbols();

        for (int i = 0; i < reels.Length; i++)
        {
            symbolsOnReel.Add(i, reels[i].GetComponent<SymbolsMove>());
        }
        
    }
    public void DoMoveReel()
    {
        playButton.interactable = false;

        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].DOAnchorPosY(-1 * countRotation * symbolSize * symbolCount, timeRotation)
                .SetDelay(i * delayStep)
                .SetEase(startEase)
                .OnComplete(() =>
                {
                    ResetGame(-1 * countRotation * symbolSize * symbolCount);
                });
        }
    }

    void ResetGame(float stopPosition)
    {
        if (reels[reels.Length - 1].localPosition.y <= stopPosition + 1)
        {
            for (int i = 0; i < reels.Length; i++)
            {
                symbolsOnReel[i].ResetLocalPosition(stopPosition);
                reels[i].localPosition = new Vector3(0, 0);
            }
            playButton.interactable = true;
        }

    }
}
