using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using Unity.Sentis;
using System.Linq;
using System;
using System.Xml.Linq;

public class aiScript : MonoBehaviour
{
    public TextAsset text;

    // Start is called before the first frame update
    void Start()
    {

 

    ModelAsset modelAsset = Resources.Load("model") as ModelAsset;
    Model runtimeModel = ModelLoader.Load(modelAsset);
    IWorker worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);



    string allText = text.text;
    var lines = allText.Split('\n');    

    var data = new List<float>();

    foreach (var line in allText)
    {
        // Assuming each line contains multiple values separated by spaces
        var values = allText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            print(values);
    foreach (var value in values)
        {
            try
            {
                decimal d = Decimal.Parse(value, System.Globalization.NumberStyles.Float);
                data.Add((float)d);
            }
            catch
            {
                Debug.Log("Cant parse: " + value);
                    break;
            }
        }
    }

    // Reshape the data into a 30x76 array
    print(data);
    int rows = 30;
    int cols = 76;
     float[,] reshapedData = new float[rows, cols];

    if (data.Count == rows * cols)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                reshapedData[i, j] = data[i * cols + j];
            }
        }
    }

    // Assuming reshapedData is a float[30, 76] 2D array
    float[] flattenedData = new float[30 * 76];
    int index = 0;
    for (int i = 0; i < 30; i++)
    {
        for (int j = 0; j < 76; j++)
        {
            flattenedData[index++] = reshapedData[i, j];
        }
        }

    // Create a 3D tensor shape according to the input shape of the model
    TensorShape shape = new TensorShape(1, 30, 76);


    // Create a new tensor from the flattened array
    TensorFloat tensor = new TensorFloat(shape, flattenedData);

     // Create a 3D tensor shape according to the input shape of the model
      //TensorShape shape = new TensorShape(1, 30, 76);

    // Create a new tensor from the array
    //TensorFloat tensor = new TensorFloat(shape, data);

    UnityEngine.Debug.Log(tensor);

    
    if (tensor != null)
    {
        worker.Execute(tensor);
    }
    else
    {
        Debug.LogError("Tensor is null");
    }

    TensorFloat outputTensor = worker.PeekOutput() as TensorFloat;

    outputTensor.MakeReadable();
    outputTensor.PrintDataPart(69);

    UnityEngine.Debug.Log("output" + outputTensor);


    float[] tensorArray = outputTensor.ToReadOnlyArray();
    
    UnityEngine.Debug.Log("output ARRAY" + tensorArray);


    List < float[]>outputs = new List<float[]>();
        var list = outputTensor.ToReadOnlyArray();

    for (int i = 0; i < list.Length; i += 3)
    {

        float[] group = new float[] { list[i], list[i + 1], list[i + 2] };
        outputs.Add(group);
    }

    //foreach(var group in outputs ) { foreach(var g in group) Debug.Log(g); }


    List<string> actions = new List<string> { "uppercut", "notboxing", "jabb" };

   // foreach (var group in outputs) { UnityEngine.Debug.Log(actions[group.IndexOf(group,group.Max())]); }

    foreach (var group in outputs)
    {
        int maxIndex = Array.IndexOf(group, group.Max());
        UnityEngine.Debug.Log(actions[maxIndex]);
    }



        tensor?.Dispose();
    worker?.Dispose();

    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
