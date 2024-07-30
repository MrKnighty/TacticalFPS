using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SelectStaticMeshObjects : EditorWindow
{
    [MenuItem("Tools/Select Static Mesh Objects")]
    public static void ShowWindow()
    {
        GetWindow<SelectStaticMeshObjects>("Select Static Mesh Objects");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Select Static Mesh Objects"))
        {
            SelectStaticMeshes();
        }
    }

    private static void SelectStaticMeshes()
    {
        List<GameObject> staticMeshObjects = new List<GameObject>();

        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            if (obj.isStatic && obj.GetComponent<MeshRenderer>() != null)
            {
                staticMeshObjects.Add(obj);
            }
        }

        Selection.objects = staticMeshObjects.ToArray();
        Debug.Log(staticMeshObjects.Count + " static mesh objects selected.");
    }
}
