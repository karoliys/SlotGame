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
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private LineRenderer winlinePrefab;

    private Queue<LineRenderer> gameObjectLine = new Queue<LineRenderer>();
    private Dictionary<RectTransform, SymbolsMove> symbolsOnReel = new Dictionary<RectTransform, SymbolsMove>();
    private Queue<WinLineData> winLines = new Queue<WinLineData>();
    private float symbolSize;
    private int symbolCount;
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
        ClearLine();
        playButtonRT.localScale = Vector3.zero;
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
        CheckWinSymbol();
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
                   SetWinLine();
                   DoAnimationReel();
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
    void ClearLine()
    {
        while (gameObjectLine.TryDequeue(out var newLine))
        {
            Destroy(newLine.gameObject);
        }
    }
    void CheckWinSymbol()
    {
        Dictionary<int, Sprite[]> allSymbols = new Dictionary<int, Sprite[]>();
        foreach (var reelRT in reels)
        {
            allSymbols.Add(symbolsOnReel[reelRT].GetIdReel(), symbolsOnReel[reelRT].GetWinSymbolOnReel());
        }
        foreach (var winLine in gameConfig.WinLines)
        {
            bool checkTrue = true;
            for (int i = 0; i < winLine.WinSymbol.Length - 1; i++)
            {
                Sprite[] first, second;
                first = allSymbols[i];
                second = allSymbols[i + 1];
                if (checkTrue == true && first[winLine.WinSymbol[i]] != second[winLine.WinSymbol[i+1]])
                {
                    checkTrue = false;
                }
            }
            if (checkTrue)
            {
                winLines.Enqueue(winLine);
            }
        }
    }

    void SetWinLine()
    {
        while (winLines.TryDequeue(out var winLine))
        {
            LineRenderer gameObjectWinLine = Instantiate(winlinePrefab, Vector3.zero, Quaternion.Euler(0f, 0f, 0f)) as LineRenderer;
            gameObjectLine.Enqueue(gameObjectWinLine);
            gameObjectWinLine.positionCount = reels.Length;
            for (int i = 0; i < reels.Length; i++)
            {
                symbolsOnReel[reels[i]].SetWinLinePos(gameObjectWinLine,winLine.WinSymbol[i]);
            }
        }
    }
    void DoAnimationReel()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            symbolsOnReel[reels[i]].AnimationPlay();
        }
    }
    public void PlayButtonOn()
    {
        playButtonRT.localScale = Vector3.one;
    }
}
