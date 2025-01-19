Shader "Hidden/EasyMaskBrush"
{
    Properties
    {
        [NoScaleOffset] _BrushShape("Brush Shape", 2D) = "white" {}
        _BrushParams("Brush Parameters", Vector) = (1, 1, 1)
    }
    SubShader
    {
        Lighting Off
        Blend One Zero
        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            sampler2D _BrushShape;
            float3 _BrushParams;
            
            fixed4 frag(v2f_customrendertexture IN) : COLOR
            {
                float smoothness = _BrushParams.r;
                float opacity = _BrushParams.g;
                int tiling = _BrushParams.b;
                
                float brushShape = tex2D(_BrushShape, IN.localTexcoord.xy * tiling).r;
                float softenedBrush = pow(brushShape,smoothness*2);

                return softenedBrush * opacity;
            }
            ENDCG
        }
    }
}
