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
    [SerializeField] private float scaleAnimation;
    [SerializeField] private int countAnimation;
    [SerializeField] private float timeAnimation;

    private Queue<Sprite> stopSprites = new Queue<Sprite>();
    private Queue<int> nubmerOfWinSymbol = new Queue<int>();
    private float symbolSize;
    private int countSymbols;
    private Dictionary<RectTransform, Image> symbolsImage = new Dictionary<RectTransform, Image>();
    private Dictionary<int, RectTransform> symbolsRectTransform = new Dictionary<int, RectTransform>();
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
        symbolsRectTransform.Add(curentWinSprite, symbol);
        var symbolSprite = symbolsImage[symbol];
        symbolSprite.sprite = stopSprites.Dequeue();
        curentWinSprite++;
    }

    public void SetWinSprites()
    {
        stopSprites.Clear();
        curentWinSprite = 0;
        symbolsRectTransform.Clear();
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
        foreach (var symbol in symbols)
        {
            symbol.localPosition = new Vector3(symbol.localPosition.x, symbol.localPosition.y + changePositionOn);
        }
    }


    public void AnimationPlay()
    {
        bool checkTrue = false;
        while (nubmerOfWinSymbol.TryDequeue(out var numWinSym))
        {
            checkTrue = true;
            var winSymbol = symbolsRectTransform[numWinSym];
            symbolsRectTransform.Remove(numWinSym);
            Vector3 vector = new Vector3(scaleAnimation, scaleAnimation, scaleAnimation);
            winSymbol.DOPunchScale(vector,countAnimation, countAnimation,timeAnimation)
            //winSymbol.DOShakeScale(scaleAnimation, 1,10,0,false)
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
        }
        for (int i = 0; i < 3; i++)
        {
            if (symbolsRectTransform.ContainsKey(i) && checkTrue)
            {
                var symbolCL = symbolsImage[symbolsRectTransform[i]];
                Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                symbolCL.DOColor(color, timeAnimation);
            }
        }
        if (!checkTrue)
        {
            reel.PlayButtonOn();
        }
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
    public Sprite[] GetWinSymbolOnReel()
    {
        return stopSprites.ToArray();
    }
    public int GetIdReel()
    {
        return idReel;
    }
    public void SetWinLinePos(LineRenderer line,int winSprite)
    {
        var winSymbol = symbolsRectTransform[winSprite];
        if (!nubmerOfWinSymbol.Contains(winSprite))
        {
            nubmerOfWinSymbol.Enqueue(winSprite);
            print("id: " + idReel + " winSprite: " + winSprite);
        }
        Vector3 vector = new Vector3(winSymbol.position.x, winSymbol.position.y);
        line.SetPosition(idReel, vector);
    }
}
