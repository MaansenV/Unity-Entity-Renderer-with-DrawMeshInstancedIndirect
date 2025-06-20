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
            StructuredBuffer<int> flipFlagsBuffer;

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
                int flipFlags = flipFlagsBuffer[instanceID];

                // Apply flipping to vertex position before rotation
                float3 vertexPos = v.vertex.xyz;
                if (flipFlags & 1) // FlipX
                    vertexPos.x = 1.0 - vertexPos.x;
                if (flipFlags & 2) // FlipY
                    vertexPos.y = 1.0 - vertexPos.y;

                // Rotate the vertex
                float4 rotatedVertex = mul(float4(vertexPos, 1.0) - float4(0.5, 0.5, 0, 0), rotationZMatrix(translationAndRot.w));

                // Scale it
                float scale = scaleBuffer[instanceID];
                float3 worldPosition = translationAndRot.xyz + (rotatedVertex.xyz * scale);

                v2f o;
                o.pos = UnityObjectToClipPos(float4(worldPosition, 1.0f));

                // Apply UV transformation with flipping
                float2 texCoord = v.texcoord;
                if (flipFlags & 1) // FlipX
                    texCoord.x = 1.0 - texCoord.x;
                if (flipFlags & 2) // FlipY
                    texCoord.y = 1.0 - texCoord.y;
                    
                o.uv = texCoord * uv.xy + uv.zw;
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