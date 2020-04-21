using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceTexture : MonoBehaviour
{

    public ComputeShader shader;
    public int TexResolution = 256;
    private int kernelHandle;

    Renderer rend;
    RenderTexture myRt;

    // Start is called before the first frame update
    void Start()
    {
        int kernelHandle = shader.FindKernel("CSMain");
        myRt = new RenderTexture(TexResolution, TexResolution, 24);
        myRt.enableRandomWrite = true;
        myRt.Create();
        rend = gameObject.GetComponent<Renderer>();
  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            UpdateTextureFromCompute();
        }
    }


    private void UpdateTextureFromCompute()
    {
        shader.SetInt("RandOffset", (int)(Time.time * 100));
        shader.SetTexture(kernelHandle, "Result", myRt);
        shader.Dispatch(kernelHandle, TexResolution / 8, TexResolution / 8, 1);

        rend.material.SetTexture("_BaseColorMap", myRt);

        /*
        var buf = myRt.colorBuffer.GetNativeRenderBufferPtr();
        
        for(int i = 0; i < 10; i++){
            Debug.Log(buf);
        }

        Debug.Log("HEI");
        */
    }

}
