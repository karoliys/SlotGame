using UnityEngine;

[CreateAssetMenu(fileName = "New Game Config", menuName = "Game Config")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private SymbolData[] symbols;
    [SerializeField] private FinalScreenData[] finalScreens;
    [SerializeField] private WinLineData[] winLines;


    public SymbolData[] Symbols => symbols;

    public FinalScreenData[] FinalScreens => finalScreens;

    public WinLineData[] WinLines => winLines;
}
