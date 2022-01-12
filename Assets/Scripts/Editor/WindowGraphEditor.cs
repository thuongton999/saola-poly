using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WindowGraph))]
public class WindowGraphEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WindowGraph windowGraph = (WindowGraph)target;
        if (GUILayout.Button("Generate"))
        {
            windowGraph.ShowGraph();
        }
    }
}