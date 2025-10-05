using UnityEngine;

public static class ClipboardUtility
{
    public static void CopyToClipboard(string text)
    {
#if UNITY_EDITOR
        GUIUtility.systemCopyBuffer = text;
#elif UNITY_WEBGL
        CopyToClipboardWebGL(text);
#else
        GUIUtility.systemCopyBuffer = text;
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void CopyToClipboardWebGL(string str);
#endif
}
