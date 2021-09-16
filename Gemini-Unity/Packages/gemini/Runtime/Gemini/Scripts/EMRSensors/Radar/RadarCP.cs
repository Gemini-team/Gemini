﻿using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using Gemini.EMRS.Radar;

class RadarCP : CustomPass{
    public GameObject radar;
    private Camera[] cameras;
    private ShaderTagId[] shaderTags;

    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd){
        shaderTags = new ShaderTagId[2]{
            new ShaderTagId("DepthOnly"),
            new ShaderTagId("DepthForwardOnly"),
        };
        cameras = radar.GetComponent<RadarScript>().radarCameras;
}

protected override void Execute(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera camera, CullingResults cullingResult) {
        for (int i = 0; i < cameras.Length; i++) {
            Camera bakingCamera = cameras[i];
            RenderTexture targetTexture = bakingCamera.targetTexture;

            bakingCamera.TryGetCullingParameters(out var cullingParams);
            cullingParams.cullingOptions = CullingOptions.ShadowCasters;
            cullingResult = renderContext.Cull(ref cullingParams);
            var result = new RendererListDesc(shaderTags, cullingResult, bakingCamera){
                rendererConfiguration = PerObjectData.None,
                renderQueueRange = RenderQueueRange.all,
                sortingCriteria = SortingCriteria.BackToFront,
                excludeObjectMotionVectors = false,
                layerMask = -1,
            };
            Matrix4x4 cameraProjMatrix = bakingCamera.projectionMatrix;
            var p = GL.GetGPUProjectionMatrix(cameraProjMatrix, true);
            Matrix4x4 scaleMatrix = Matrix4x4.identity;
            scaleMatrix.m22 = -1.0f;
            var v = scaleMatrix * bakingCamera.transform.localToWorldMatrix.inverse;
            var vp = p * v;

            cmd.SetGlobalMatrix("_ViewMatrix", v);
            cmd.SetGlobalMatrix("_InvViewMatrix", v.inverse);
            cmd.SetGlobalMatrix("_ProjMatrix", p);
            cmd.SetGlobalMatrix("_InvProjMatrix", p.inverse);
            cmd.SetGlobalMatrix("_ViewProjMatrix", vp);
            cmd.SetGlobalMatrix("_InvViewProjMatrix", vp.inverse);
            cmd.SetGlobalMatrix("_CameraViewProjMatrix", vp);
            cmd.SetGlobalVector("_WorldSpaceCameraPos", Vector3.zero);

            CoreUtils.SetRenderTarget(cmd, targetTexture, ClearFlag.Depth);
            HDUtils.DrawRendererList(renderContext, cmd, RendererList.Create(result));
        }
    }
}