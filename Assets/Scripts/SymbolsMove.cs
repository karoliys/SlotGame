using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SymbolsMove : MonoBehaviour
{
    [SerializeField] private ReelsScroll reel;
    [SerializeField] private RectTransform[] symbols;
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private float movementCoordinate;
    [SerializeField] private int idReel;
    [SerializeField] private LineRenderer winLine;
    [SerializeField] private float scaleAnimation;
    [SerializeField] private float timeAnimation;

    private Queue<Sprite> stopSprites = new Queue<Sprite>();
    private int? winSprite;
    private RectTransform winSymbol;
    private float symbolSize;
    private int countSymbols;
    private Dictionary<RectTransform, Image> symbolsImage = new Dictionary<RectTransform, Image>();
    private bool setRandom = true;
    private int curentFinalScreen = 0;
    private int curentWinSprite = 0;

    void Awake()
    {
        var symbol = symbols[0];
        symbolSize = symbol.rect.height;
        countSymbols = symbols.Length;

        foreach (var symbolDT in symbols)
        {
            symbolsImage.Add(symbolDT, symbolDT.GetComponent<Image>());
        }

    }
    void FixedUpdate()
    {
        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i].position.y <= movementCoordinate)
            {
                ChangePosition(symbols[i]);
                if (setRandom)
                {
                    ChangeImageRandom(symbols[i]);
                }
                else
                {
                    ChangeImageStop(symbols[i]);
                }
            }
        }
    }

    void ChangePosition(RectTransform symbol)
    {
        var newPositionY = symbol.localPosition.y + symbolSize * countSymbols;
        symbol.localPosition = new Vector3(symbol.localPosition.x, newPositionY);
    }

    void ChangeImageRandom(RectTransform symbol)
    {
        var random = Random.Range(0, gameConfig.Symbols.Length);
        var symbolSprite = symbolsImage[symbol];
        symbolSprite.sprite = gameConfig.Symbols[random].SymbolImage;

    }
    void ChangeImageStop(RectTransform symbol)
    {
        var symbolSprite = symbolsImage[symbol];
        symbolSprite.sprite = stopSprites.Dequeue();
        if (winSprite == curentWinSprite)
        {
            winSymbol = symbol;
            winLine.positionCount++;
            curentWinSprite++;
        }
        else
        {
            curentWinSprite++;
        }
    }

    public void SetWinSprites()
    {
        stopSprites.Clear();
        if (gameConfig.FinalScreens[curentFinalScreen].WinsSymbol.Length > 0)
        {
            winSprite = gameConfig.FinalScreens[curentFinalScreen].WinsSymbol[idReel];
        }
        else
        {
            winSprite = null;
        }
        for (int i = 0; i < countSymbols - 1; i++)
        {
            stopSprites.Enqueue(gameConfig.Symbols[gameConfig.FinalScreens[curentFinalScreen].FinalSymbol[idReel * (countSymbols - 1) + i]].SymbolImage);
        }
        if (curentFinalScreen < gameConfig.FinalScreens.Length - 1)
        {
            curentFinalScreen++;
        }
        else
        {
            curentFinalScreen = 0;
        }
    }

    public void ResetLocalPosition(float changePositionOn)
    {
        setRandom = true;
        curentWinSprite = 0;
        foreach (var symbol in symbols)
        {
            symbol.localPosition = new Vector3(symbol.localPosition.x, symbol.localPosition.y + changePositionOn);
        }
    }

    public void SetWinLine()
    {
        if (winSprite != null)
        {
            Vector3 vector = new Vector3(winSymbol.position.x, winSymbol.position.y);
            winLine.SetPosition(idReel, vector);
            AnimationPlay();
        }
        else
        {
            reel.PlayButtonOn();
        }
    }

    void AnimationPlay()
    {
        if (winSprite != null)
        {
            foreach (var symbol in symbols)
            {
                if (symbol != winSymbol)
                {
                    var symbolCL = symbolsImage[symbol];
                    Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    symbolCL.DOColor(color, timeAnimation);

                }
            }
            winSymbol.DOScale(scaleAnimation, timeAnimation)
            .OnComplete(() =>
            {
                winSymbol.DOScale(1.0f, timeAnimation)
                .OnComplete(()=> 
                {
                    winSymbol.DOScale(scaleAnimation, timeAnimation)
                    .OnComplete(() =>
                    {
                        winSymbol.DOScale(1.0f, timeAnimation)
                        .OnComplete(() =>
                        {
                            foreach (var symbol in symbols)
                            {
                                var symbolCL = symbolsImage[symbol];
                                Color color = new Color(1f, 1f, 1f, 1f);
                                symbolCL.DOColor(color, timeAnimation);
                            }
                            reel.PlayButtonOn();
                        });
                    });
                });
            });
        }
    }

    public void ClearWinLine()
    {
        winLine.positionCount = 0;
    }

    public float GetSymbolSize()
    {
        return symbolSize;
    }
    public int GetCountSymbols()
    {
        return countSymbols;
    }
    public void SetRandomFalse()
    {
        setRandom = false;
    }
}
