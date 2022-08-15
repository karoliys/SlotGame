using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolsMove : MonoBehaviour
{
    [SerializeField] private RectTransform[] Symbols;
    [SerializeField] Sprite[] Sprites;
    [SerializeField] private float movement—oordinate;

    private float symbolSize;
    private int countSymbols;
    private Dictionary<RectTransform, Image> symbolsImage = new Dictionary<RectTransform, Image>();

    void Awake()
    {
        var Symbol = Symbols[0];
        symbolSize = Symbol.rect.height;
        countSymbols = Symbols.Length;

        foreach (RectTransform SymbolDT in Symbols)
        {
            symbolsImage.Add(SymbolDT, SymbolDT.GetComponent<Image>());
        }
    }
    void Update()
    {
        for (int i = 0; i < Symbols.Length; i++)
        {
            if (Symbols[i].position.y <= movement—oordinate)
            {
                ChangePosition(Symbols[i]);
                ChangeImage(Symbols[i]);
            }
        }
    }

    void ChangePosition(RectTransform Symbol)
    {
        var newPositionY = Symbol.localPosition.y + symbolSize * countSymbols;
        Symbol.localPosition = new Vector3(Symbol.localPosition.x, newPositionY);
    }

    void ChangeImage(RectTransform Symbol)
    {
        var random = Random.Range(0,Sprites.Length);
        var symbolSprite = symbolsImage[Symbol];
        symbolSprite.sprite = Sprites[random];

    }

    public void ResetLocalPosition(float ChangePositionOn)
    {
        foreach (RectTransform Symbol in Symbols)
        {
            Symbol.localPosition = new Vector3(Symbol.localPosition.x, Symbol.localPosition.y + ChangePositionOn);
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

}
