using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolsMove : MonoBehaviour
{
    [SerializeField] private RectTransform[] symbols;
    [SerializeField] GameConfig gameConfig;
    [SerializeField] private float movementCoordinate;
    [SerializeField] private int idReel;

    private Stack<Sprite> stopSprites = new Stack<Sprite>();
    private float symbolSize;
    private int countSymbols;
    private Dictionary<RectTransform, Image> symbolsImage = new Dictionary<RectTransform, Image>();
    private bool setRandom = true;
    private int curentFinalScreen = 0;

    void Awake()
    {
        var symbol = symbols[0];
        symbolSize = symbol.rect.height;
        countSymbols = symbols.Length;
        
        foreach (var symbolDT in symbols)
        {
            symbolsImage.Add(symbolDT, symbolDT.GetComponent<Image>());
        }

        SetWinSprites(idReel);

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
        var random = Random.Range(0,gameConfig.Symbols.Length);
        var symbolSprite = symbolsImage[symbol];
        symbolSprite.sprite = gameConfig.Symbols[random].SymbolImage;

    }
    void ChangeImageStop(RectTransform symbol)
    {
        var symbolSprite = symbolsImage[symbol];
        symbolSprite.sprite = stopSprites.Pop();
    }
    void SetWinSprites(int column)
    {
        stopSprites.Clear();
        for (int i = 0; i < countSymbols-1; i++)
        {
            stopSprites.Push(gameConfig.Symbols[gameConfig.FinalScreens[curentFinalScreen].FinalSymbol[column*(countSymbols-1) + i]].SymbolImage);
        }
        if (curentFinalScreen < gameConfig.FinalScreens.Length-1)
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
        SetWinSprites(idReel);
        setRandom = true;
        foreach (var symbol in symbols)
        {
            symbol.localPosition = new Vector3(symbol.localPosition.x, symbol.localPosition.y + changePositionOn);
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
}
