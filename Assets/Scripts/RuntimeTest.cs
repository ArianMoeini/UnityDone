/*
using Python.Runtime;
using UnityEditor;
using UnityEditor.Scripting.Python;
using UnityEngine;

public class RuntimeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PythonRunner.EnsureInitialized();
        using (Py.GIL()) // Grab the GIL
        {
            try
            {
                dynamic sys = Py.Import("sys"); // Import the sys module
                Debug.Log("python version: " + sys.version); // Log the Python version
                Debug.Log(Application.dataPath);
                PythonRunner.RunFile($"{Application.dataPath}/Scripts/test.py");

            }
            catch (PythonException e)
            {
                Debug.LogException(e); // Log any Python exceptions
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
*/