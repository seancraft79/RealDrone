using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSize
{
    int ScreenWidth, ScreenHeight;

    public static Vector2Int GetEditorGameViewSize()
    {
        //Taking game view using the method shown below	
        var gameView = GetMainGameView();
        var prop = gameView.GetType().GetProperty("currentGameViewSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var gvsize = prop.GetValue(gameView, new object[0] { });
        var gvSizeType = gvsize.GetType();

        //I have 2 instance variable which this function sets:
        int ScreenHeight = (int)gvSizeType.GetProperty("height", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0] { });
        int ScreenWidth = (int)gvSizeType.GetProperty("width", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0] { });
        return new Vector2Int(ScreenWidth, ScreenHeight);
    }

    static UnityEditor.EditorWindow GetMainGameView()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetMainGameView.Invoke(null, null);
        return (UnityEditor.EditorWindow)Res;
    }
}
