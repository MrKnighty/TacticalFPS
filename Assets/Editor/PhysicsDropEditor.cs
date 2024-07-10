using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;

public static class PhysicsDropContextMenu
{
    private const float defaultDropDuration = 5.0f;

    [MenuItem("CONTEXT/Transform/Drop Object with Physics")]
    private static void DropObject(MenuCommand menuCommand)
    {
        Transform objTransform = menuCommand.context as Transform;
        if (objTransform != null)
        {
            // Record the initial state for undo
            Undo.RegisterFullObjectHierarchyUndo(objTransform.gameObject, "Drop Object with Physics");

            // Start the coroutine to simulate physics
            EditorCoroutineUtility.StartCoroutine(SimulatePhysicsCoroutine(objTransform, defaultDropDuration), null);
        }
    }

    [System.Obsolete]
    private static IEnumerator SimulatePhysicsCoroutine(Transform objTransform, float duration)
    {
        // Temporarily disable automatic physics simulation
        bool previousSimulationMode = Physics.autoSimulation;
        Physics.autoSimulation = false;

        Rigidbody rb = objTransform.gameObject.GetComponent<Rigidbody>();
        bool addedRigidbody = false;

        // If no Rigidbody, add one
        if (rb == null)
        {
            rb = objTransform.gameObject.AddComponent<Rigidbody>();
            addedRigidbody = true;
        }

        rb.isKinematic = false;

        float timeStep = Time.fixedDeltaTime;
        int steps = Mathf.CeilToInt(duration / timeStep);

        // Perform the simulation in steps
        try
        {
            for (int i = 0; i < steps; i++)
            {
                Physics.Simulate(timeStep);
                yield return new EditorWaitForSeconds(timeStep);
            }
        }
        finally
        {
            // Clean up and restore physics simulation state
            if (addedRigidbody)
            {
                Object.DestroyImmediate(rb);
            }

            // Always restore autoSimulation to its previous state
            Physics.autoSimulation = previousSimulationMode;

            // Mark the scene as dirty to ensure changes are saved
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}

public static class EditorCoroutineUtility
{
    public class EditorCoroutine
    {
        public IEnumerator coroutine;
        private EditorWindow window;

        public EditorCoroutine(IEnumerator coroutine, EditorWindow window)
        {
            this.coroutine = coroutine;
            this.window = window;
        }

        public void Start()
        {
            EditorApplication.update += MoveNext;
        }

        public void Stop()
        {
            EditorApplication.update -= MoveNext;
        }

        private void MoveNext()
        {
            if (!coroutine.MoveNext())
            {
                Stop();
            }
        }
    }

    public static EditorCoroutine StartCoroutine(IEnumerator coroutine, EditorWindow window)
    {
        EditorCoroutine editorCoroutine = new EditorCoroutine(coroutine, window);
        editorCoroutine.Start();
        return editorCoroutine;
    }
}

public class EditorWaitForSeconds : CustomYieldInstruction
{
    private float waitTime;
    private float startTime;

    public EditorWaitForSeconds(float time)
    {
        waitTime = time;
        startTime = (float)EditorApplication.timeSinceStartup;
    }

    public override bool keepWaiting => (float)EditorApplication.timeSinceStartup - startTime < waitTime;
}
