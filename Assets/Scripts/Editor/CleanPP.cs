using UnityEditor;
using UnityEngine;

static class CleanPP
{
    [MenuItem("Tools/Clear All Player Prefs")]
    static void ClearPP()
    {
        Debug.Log("Cleared all PP.");

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
