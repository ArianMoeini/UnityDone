using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Sentis;
using UnityEngine;

public class zedcameraHandler : MonoBehaviour
{
    
    public ZEDManager zedManager;
    int frameCounter = 0;
    Queue<float[]> sequenceQueue = new Queue<float[]>();

    // Start is called before the first frame update
    void Start()
    {

        
        //Screen.SetResolution(1920/2, 1080/2, true);
        zedManager.OnZEDReady += ZedManager_OnZEDReady;
        zedManager.OnBodyTracking += ZedManager_OnBodyTracking;
        //zedManager.OnBodyTracking_SDKData += ZedManager_OnBodyTracking_SDKData;

    }

    private void ZedManager_OnBodyTracking_SDKData(sl.Bodies bodies)
    {
        //var hej = bodies;

       // Debug.Log(bodies.bodyList[0].keypoint2D[15]);

        throw new System.NotImplementedException();
    }

    private void ZedManager_OnBodyTracking(BodyTrackingFrame bodyFrame)
    {
        ModelAsset modelAsset = Resources.Load("modelfunkar") as ModelAsset;
        Model runtimeModel = ModelLoader.Load(modelAsset);
        IWorker worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, runtimeModel);

        
        int desiredFrameCount = 30;

        List<float> flattenedKeypoints = new List<float>();
        for (int i = 0; i < bodyFrame.rawbodies.bodyList[0].keypoint2D.Length; i++)
        {
            var hej = bodyFrame.rawbodies.bodyList[0].keypoint2D[i];
            Vector2 convertedPoint = ConvertZED2DToUnity(bodyFrame.rawbodies.bodyList[0].keypoint2D[i], Screen.height);
            flattenedKeypoints.Add(convertedPoint.x);
            flattenedKeypoints.Add(convertedPoint.y);
        }
        float[] singleFrameData = flattenedKeypoints.ToArray();

        sequenceQueue.Enqueue(singleFrameData);
        frameCounter++;
        while (sequenceQueue.Count > desiredFrameCount)
        {
            sequenceQueue.Dequeue();
        }

        if (frameCounter >= desiredFrameCount)
        {
            float[,] sequenceArray = new float[desiredFrameCount, 76]; // 76 = 38 keypoints * 2 (x, y)
            int frameIndex = 0;
            foreach (float[] frameData in sequenceQueue)
            {
                for (int i = 0; i < 76; i++)
                {
                    sequenceArray[frameIndex, i] = frameData[i];
                }
                frameIndex++;
            }


            //int keypointsPerFrame = 38 * 2; // 38 keypoints, each with an x and y value

            //float[,] sequenceArray2 = new float[desiredFrameCount, keypointsPerFrame];

            //for (int frame = 0; frame < desiredFrameCount; frame++)
            //{
            //for (int keypoint = 0; keypoint < keypointsPerFrame; keypoint++)
            //{
            //sequenceArray2[frame, keypoint] = frame + keypoint;
            //}
            //}


        // Flatten the sequenceArray if necessary, depending on your tensor creation method
        float[] flattenedArray = new float[desiredFrameCount * 76];
            Buffer.BlockCopy(sequenceArray, 0, flattenedArray, 0, flattenedArray.Length * sizeof(float));

            TensorShape shape = new TensorShape(1, desiredFrameCount, 76);
            // Create a new tensor from the flattened array
            TensorFloat tensor = new TensorFloat(shape, flattenedArray);

            // Assuming you have a method or model worker ready to execute the tensor
            if (tensor != null)
            {
                worker.Execute(tensor);
                // Process model output
            }
            else
            {
                Debug.LogError("Tensor is null");
            }


            TensorFloat outputTensor = worker.PeekOutput() as TensorFloat;

            outputTensor.MakeReadable();
            outputTensor.PrintDataPart(69);

            //UnityEngine.Debug.Log("output" + outputTensor);


            float[] tensorArray = outputTensor.ToReadOnlyArray();

        //UnityEngine.Debug.Log("output ARRAY" + tensorArray);


            List<float[]> outputs = new List<float[]>();
            var list = outputTensor.ToReadOnlyArray();

            for (int i = 0; i < list.Length; i += 3)
            {

                float[] group = new float[] { list[i], list[i + 1], list[i + 2] };
                outputs.Add(group);
            }

            //foreach(var group in outputs ) { foreach(var g in group) Debug.Log(g); }


            List<string> actions = new List<string> { "boxing", "notFighting", "kick" };

            // foreach (var group in outputs) { UnityEngine.Debug.Log(actions[group.IndexOf(group,group.Max())]); }

            foreach (var group in outputs)
            {
                int maxIndex = Array.IndexOf(group, group.Max());
                UnityEngine.Debug.Log(actions[maxIndex]);
            }

            //clear sequenceQueue and reset frameCounter
            sequenceQueue.Clear(); // clear
            //sequenceQueue = new Queue<float[]>(); // reset
            frameCounter = 0;
            tensor?.Dispose();
            worker?.Dispose();

        }
        else
        {
           // Debug.Log("Collecting frames: " + frameCounter + "/" + desiredFrameCount);
        }

    }




    // for (int i = 0; i < bodyFrame.rawbodies.bodyList[0].keypoint2D.Length; i ++)
    //{   
    //Debug.Log(zedManager);

    //Debug.Log(ConvertZED2DToUnity(bodyFrame.rawbodies.bodyList[0].keypoint2D[15],Screen.height));
    //}
    //Debug.Log(ConvertZED2DToUnity(bodyFrame.rawbodies.bodyList[0].keypoint2D[15],Screen.height));

    public static Vector2 ConvertZED2DToUnity(Vector2 zedPoint, float screenHeight)
    {
        // Example conversion: Inverting the Y coordinate for 2D points (e.g., screen or image coordinates)
        return new Vector2((zedPoint.x), (zedPoint.y)); // RESOLUTION HALF OF SCREEN, CHANE LATER IF NEEDED NOW MODEL TRAINED ON 720x1280 AND UNITY CAMERA ON 1920x1080
    }



    private void ZedManager_OnZEDReady()
    {
        Debug.Log("Camera ready");
    }

    // Update is called once per frame
    void Update()
    {

    }




}
