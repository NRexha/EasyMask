Shader "Hidden/EasyMaskDebug"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "black" {}
        _ActiveChannel ("Active Channel", Int) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            int _ActiveChannel;

            struct vertData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(vertData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.uv);
                switch (_ActiveChannel)
                {
                    case 0 :
                        return texColor;
                    case 1:
                        return half4(texColor.xxx, 1);

                    case 2:
                        return half4(texColor.yyy, 1);

                    case 3:
                        return half4(texColor.zzz, 1);
                    
                    default:
                        return texColor;
                }
            }
            ENDCG
        }
    }
}
