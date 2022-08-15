using UnityEngine;

[CreateAssetMenu(fileName = "New Symbol Data", menuName = "Symbol Data")]
public class SymbolData : ScriptableObject
{
    [SerializeField] private Sprite symbolImage;

    public Sprite SymbolImage => symbolImage;
}
