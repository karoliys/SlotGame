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
    [SerializeField] private Ease stopEase;
    [SerializeField] private Button playButton;
    [SerializeField] private RectTransform playButtonRT;
    [SerializeField] private Button stopButton;
    [SerializeField] private int countRotation;
    [SerializeField] private float timeRotation;
    private float symbolSize;
    private int symbolCount;
    private Dictionary<RectTransform, SymbolsMove> symbolsOnReel = new Dictionary<RectTransform, SymbolsMove>();
    private float speedRotation;

    private void Start()
    {
        playButton.interactable = true;
        stopButton.interactable = false;
        SymbolsMove reel = reels[0].GetComponent<SymbolsMove>();
        symbolSize = reel.GetSymbolSize();
        symbolCount = reel.GetCountSymbols();

        foreach (var reelRT in reels)
        {
            symbolsOnReel.Add(reelRT, reelRT.GetComponent<SymbolsMove>());
        }
        speedRotation = (countRotation * symbolSize * symbolCount) / timeRotation;

    }
    float StopPositionReel(RectTransform reel)
    {
        if (reel.localPosition.y % symbolSize != 0)
        {
            return (reel.localPosition.y) - (reel.localPosition.y % symbolSize) - symbolSize;
        }
        else
        {
            return reel.localPosition.y;
        }
    }
    public void StartMoveReels()
    {
        playButtonRT.localScale = Vector3.zero;
        symbolsOnReel[reels[0]].ClearWinLine();
        for (int i = 0; i < reels.Length; i++)
        {
            var reel = reels[i];
            symbolsOnReel[reel].SetWinSprites();
            reel.DOAnchorPosY(-1 * symbolSize * symbolCount, timeRotation / 10)
                .SetDelay(i * delayStep)
                .SetEase(startEase)
                .OnComplete(() =>
                {
                    LineMoveReel(reel, -1 * countRotation * symbolSize * symbolCount, timeRotation);
                    if (reel == reels[reels.Length - 1])
                    {
                        stopButton.interactable = true;
                    }
                });
        }
    }

    void LineMoveReel(RectTransform reel, float linePosition, float timeRotationReel)
    {
        reel.DOKill();
        reel.DOAnchorPosY(linePosition, timeRotationReel)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    float stopPosition = StopPositionReel(reel) - symbolSize * (symbolCount - 1);
                    StopMoveReel(reel, stopPosition, timeRotation / 10);
                });
    }

    void StopMoveReel(RectTransform reel, float stopPosition, float timeRotationReel)
    {
        stopButton.interactable = false;
        reel.DOKill();
        symbolsOnReel[reel].SetRandomFalse();
        reel.DOAnchorPosY(stopPosition, timeRotationReel)
           .SetEase(stopEase)
           .OnComplete(() =>
           {
               ResetGame(reel, stopPosition);

               if (reel == reels[reels.Length - 1])
               {
                   for (int i = 0; i < reels.Length; i++)
                   {
                       symbolsOnReel[reels[i]].SetWinLine();
                   }
               }
           });
    }
    public void ForceStopScroll()
    {
        stopButton.interactable = false;
        for (int i = 0; i < reels.Length; i++)
        {
            float stopPosition = StopPositionReel(reels[i]);
            float timeRotationRT = -1 * (stopPosition - reels[i].localPosition.y) / speedRotation;
            LineMoveReel(reels[i], stopPosition, timeRotationRT);
        }
    }
    void ResetGame(RectTransform reel, float stopPosition)
    {

        symbolsOnReel[reel].ResetLocalPosition(stopPosition);
        reel.localPosition = new Vector3(0, 0);

    }
    public void PlayButtonOn()
    {
        playButtonRT.localScale = Vector3.one;
    }
}
