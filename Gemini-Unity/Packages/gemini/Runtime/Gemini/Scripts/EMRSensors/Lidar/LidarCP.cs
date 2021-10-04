using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Gemini.EMRS.Lidar
{
    class LidarCP : CustomPass
    {
        public GameObject lidar;
        private Camera[] cameras;
        private ShaderTagId[] shaderTags;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            shaderTags = new ShaderTagId[2]{
                new ShaderTagId("DepthOnly"),
                new ShaderTagId("DepthForwardOnly"),
            };
            cameras = lidar.GetComponent<LidarScript>().lidarCameras;
        }

    
        protected override void Execute(CustomPassContext ctx)
        {
            RenderTexture targetTexture;
            Camera bakingCamera;            

            for (int i = 0; i < cameras.Length; i++)
            {
                bakingCamera = cameras[i];
                targetTexture = bakingCamera.targetTexture;

                ScriptableCullingParameters cullingParams;
                bakingCamera.TryGetCullingParameters(out cullingParams);
                cullingParams.cullingOptions = CullingOptions.ShadowCasters;
                var result = new RendererListDesc(shaderTags, ctx.renderContext.Cull(ref cullingParams), bakingCamera)
                {
                    rendererConfiguration = PerObjectData.None,
                    renderQueueRange = RenderQueueRange.all,
                    sortingCriteria = SortingCriteria.BackToFront,
                    excludeObjectMotionVectors = false,
                    layerMask = -1,
                };

                Matrix4x4 cameraProjMatrix = cameras[0].projectionMatrix;
                var p = GL.GetGPUProjectionMatrix(cameraProjMatrix, true);

                Matrix4x4 scaleMatrix = Matrix4x4.identity;
                scaleMatrix.m22 = -1.0f; // flip z component
                var v = scaleMatrix * bakingCamera.transform.localToWorldMatrix.inverse; // world to local, with z-component flipped
                // i.e. v is a transform from a left handed (unity) world, to a right handed (openGL) local frame
                var vp = p * v; // this makes vp a transform from a left handed unity world, to a right handed openGL clip space

                
                ctx.cmd.SetGlobalMatrix("_ViewMatrix", v);
                ctx.cmd.SetGlobalMatrix("_InvViewMatrix", v.inverse);
                ctx.cmd.SetGlobalMatrix("_ProjMatrix", p);
                ctx.cmd.SetGlobalMatrix("_InvProjMatrix", p.inverse);
                ctx.cmd.SetGlobalMatrix("_ViewProjMatrix", vp);
                ctx.cmd.SetGlobalMatrix("_InvViewProjMatrix", vp.inverse);
                ctx.cmd.SetGlobalMatrix("_CameraViewProjMatrix", vp);
                ctx.cmd.SetGlobalVector("_WorldSpaceCameraPos", Vector3.zero);

                ctx.cmd.SetRenderTarget(targetTexture, 0, CubemapFace.Unknown, i);
                // TODO: Clear here? 
                // CoreUtils.ClearRenderTarget(ctx.cmd, clearFlags, Color.black);
                CoreUtils.DrawRendererList(ctx.renderContext, ctx.cmd, RendererList.Create(result));
            }
        }
    }
}