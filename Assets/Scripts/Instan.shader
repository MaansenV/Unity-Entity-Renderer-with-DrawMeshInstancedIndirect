Shader "Instanced/SpriteRendererIndexedUv" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }

    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "IgnoreProjector"="True"
            "RenderType"="TransparentCutout"
        }
        Cull Back
        Lighting Off
        ZWrite On
        AlphaTest Greater 0
        Blend SrcAlpha OneMinusSrcAlpha
        Pass {
            CGPROGRAM
            #pragma exclude_renderers gles
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed _Cutoff;

            StructuredBuffer<float4> translationAndRotationBuffer;
            StructuredBuffer<float> scaleBuffer;
            StructuredBuffer<float4> colorsBuffer;
            StructuredBuffer<float4> uvBuffer;
            StructuredBuffer<int> uvIndexBuffer;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
            };

            float4x4 rotationZMatrix(float zRotRadians) {
                float c = cos(zRotRadians);
                float s = sin(zRotRadians);
                return float4x4(
                    c,  s, 0,  0,
                    -s, c, 0,  0,
                    0,  0, 1,  0,
                    0,  0, 0,  1
                );
            }

            v2f vert(appdata_full v, uint instanceID : SV_InstanceID) {
                float4 translationAndRot = translationAndRotationBuffer[instanceID];
                int uvIndex = uvIndexBuffer[instanceID];
                float4 uv = uvBuffer[uvIndex];

                // Rotate the vertex
                v.vertex = mul(v.vertex - float4(0.5, 0.5, 0, 0), rotationZMatrix(translationAndRot.w));

                // Scale it
                float scale = scaleBuffer[instanceID];
                float3 worldPosition = translationAndRot.xyz + (v.vertex.xyz * scale);

                v2f o;
                o.pos = UnityObjectToClipPos(float4(worldPosition, 1.0f));

                // Apply UV transformation
                o.uv = v.texcoord * uv.xy + uv.zw;
                o.color = colorsBuffer[instanceID];
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                clip(col.a - _Cutoff);
                col.rgb *= col.a;

                return col;
            }
            ENDCG
        }
    }
}
