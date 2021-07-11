using UnityEngine;
using System.Collections;

public class KeepSceneAlive : MonoBehaviour
{
    //if this checked, the scene is kept active instead of automatically, annoyingly switching to game view
    public bool KeepSceneViewActive;

    void Start()
    {
#if UNITY_EDITOR
        if (this.KeepSceneViewActive && Application.isEditor)
        {
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }
#endif
    }
}