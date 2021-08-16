Shader "FullScreen/MapPass"
{
    Properties{
        _SeaLevel("Sea Level", float) = 0
        _ViewDistance("View Distance", float) = 99.99
        [MaterialToggle]_DrawSea("Draw Sea", int) = 1
		_SeaColor("Sea Color", Color) = (0.05, 0.05, 0.78, 1.0)
		_LowColor("Min Contour Color", Color) = (0, 0, 0, 1)
		_HighColor("Max Contour Color", Color) = (1, 1, 1, 1)
		_LineColor("Line Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_LineWidth("Line Width", float) = 0.1
		_NLevels("Contour Levels", int) = 10
		_Blend("Blend", Range(0.0, 1.0)) = 1.0
	}

    HLSLINCLUDE

    #pragma vertex Vert

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

	float4 _SeaColor, _LowColor, _HighColor, _LineColor;
    float _SeaLevel, _ViewDistance;
	float _LineWidth, _Blend;
	int _NLevels;
    bool _DrawSea;

    float4 FullScreenPass(Varyings varyings) : SV_Target
    {
		float3 color = LoadCameraColor(varyings.positionCS.xy);
        float depth = LoadCameraDepth(varyings.positionCS.xy);

        if (depth <= _SeaLevel / _ViewDistance) {
            if (_DrawSea) return _SeaColor;
            else return float4(color, 1);
        }

		float contour = floor(depth * _NLevels);

		if (_LineWidth > 0) {
			float diff = abs(depth * _NLevels - contour);
			if (diff < _LineWidth) return _LineColor;
		}

		return lerp(float4(color, 1), lerp(_LowColor, _HighColor, contour / _NLevels), _Blend);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Minimap"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment FullScreenPass
            ENDHLSL
        }
    }
    Fallback Off
}
