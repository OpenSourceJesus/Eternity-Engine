#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class SwapTransformsPositions
{
    [MenuItem("Tools/Swap Transforms %&s")]
    static void Do ()
    {
        Transform trs = Selection.transforms[0];
        Transform trs2 = Selection.transforms[1];
        Vector3 prevPos = trs.position;
        trs.position = trs2.position;
        trs2.position = prevPos;
    }
}
#endif