using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Win Line", menuName = "Win Line")]
public class WinLineData : ScriptableObject
{
    [SerializeField] private int[] winSymbol;
    public int[] WinSymbol => winSymbol;
}