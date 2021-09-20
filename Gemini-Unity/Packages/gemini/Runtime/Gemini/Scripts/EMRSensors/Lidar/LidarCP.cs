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

        //public Texture2d someTexture1;
        //public Texture2d someTexture2;
        //public Texture2d someTexture3;
        //public Texture2d someTexture4;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            shaderTags = new ShaderTagId[2]{
                new ShaderTagId("DepthOnly"),
                new ShaderTagId("DepthForwardOnly"),
            };
            cameras = lidar.GetComponent<LidarScript>().lidarCameras;
        }

        protected override void Execute(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera camera, CullingResults cullingResult)
        {

            for (int i = 0; i < cameras.Length; i++)
            {
                Camera bakingCamera = cameras[i];
                RenderTexture targetTexture = bakingCamera.targetTexture;

                ScriptableCullingParameters cullingParams;
                bakingCamera.TryGetCullingParameters(out cullingParams);
                cullingParams.cullingOptions = CullingOptions.ShadowCasters;
                cullingResult = renderContext.Cull(ref cullingParams);
                var result = new RendererListDesc(shaderTags, cullingResult, bakingCamera)
                {
                    rendererConfiguration = PerObjectData.None,
                    renderQueueRange = RenderQueueRange.all,
                    sortingCriteria = SortingCriteria.BackToFront,
                    excludeObjectMotionVectors = false,
                    layerMask = -1,
                };

                Matrix4x4 cameraProjMatrix = cameras[0].projectionMatrix;
                //cameraProjMatrix.m22 *= -1;
                //cameraProjMatrix.m23 *= -1;
                //cameraProjMatrix.m32 *= -1;
                //bakingCamera.projectionMatrix = cameraProjMatrix;
                //var p = cameraProjMatrix;
                var p = GL.GetGPUProjectionMatrix(cameraProjMatrix, true);
                //Debug.Log(p);
                //Debug.Log(cameraProjMatrix);

                Matrix4x4 scaleMatrix = Matrix4x4.identity;
                scaleMatrix.m22 = -1.0f; // flip z component
                var v = scaleMatrix * bakingCamera.transform.localToWorldMatrix.inverse; // world to local, with z-component flipped
                // i.e. v is a transform from a left handed (unity) world, to a right handed (openGL) local frame
                var vp = p * v; // this makes vp a transform from a left handed unity world, to a right handed openGL clip space

                cmd.SetGlobalMatrix("_ViewMatrix", v);
                cmd.SetGlobalMatrix("_InvViewMatrix", v.inverse);
                cmd.SetGlobalMatrix("_ProjMatrix", p);
                cmd.SetGlobalMatrix("_InvProjMatrix", p.inverse);
                cmd.SetGlobalMatrix("_ViewProjMatrix", vp);
                cmd.SetGlobalMatrix("_InvViewProjMatrix", vp.inverse);
                cmd.SetGlobalMatrix("_CameraViewProjMatrix", vp);
                cmd.SetGlobalVector("_WorldSpaceCameraPos", Vector3.zero);

                cmd.SetGlobalTexture("depthImages", targetTexture);
                // cmd.SetRenderTarget(targetTexture, ClearFlag.Depth, CubemapFace.Unknown, i);
                // RenderTexture _texture = new RenderTexture(targetTexture.width, targetTexture.height, 0, RenderTextureFormat.ARGB32);
                // _texture.dimension = UnityEngine.Rendering.TextureDimension.Tex2DArray;
                // _texture.volumeDepth = 4;

                // cmd.SetGlobalTexture("colorImages", _texture);


                // cmd.SetRenderTarget(_texture, targetTexture, 0, CubemapFace.Unknown, i);
                // cmd.SetTargetTexture(cmd, "depthImage" + i, targetTexture);
                // CoreUtils.SetRenderTarget(cmd, targetTexture, ClearFlag.Depth);

                // this one seems to set the targettexture, but sets the same texture for every camera
                //RenderTexture currentActiveRT = RenderTexture.active;
                cmd.SetRenderTarget(targetTexture, 0, CubemapFace.Unknown, i);
                HDUtils.DrawRendererList(renderContext, cmd, RendererList.Create(result));
                //RenderTexture.active = currentActiveRT;

            }
        }
    }
}