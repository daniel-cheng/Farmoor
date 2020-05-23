//https://github.com/przemyslawzaworski/Unity3D-CG-programming/blob/master/deferred_metallic_gloss.shader

Shader "Unlit/Block"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Tags {"LightMode" = "Deferred"}

			CGPROGRAM
			#pragma vertex vertex_shader
			#pragma fragment pixel_shader
			#pragma exclude_renderers nomrt
			#pragma multi_compile ___ UNITY_HDR_ON
			#pragma multi_compile TEXTURES_ON TEXTURES_OFF
			#pragma multi_compile VIEW_NORMALS_OFF VIEW_NORMALS_ON
			#pragma multi_compile VIEW_UV_OFF VIEW_UV_ON
			#pragma multi_compile LIGHTING_ON LIGHTING_OFF

			#pragma target 3.0
			#include "UnityPBSLighting.cginc"

			float4 _Color;
			float _Metallic;
			float _Gloss;

			struct structureVS
			{
				float4 screen_vertex : SV_POSITION;
				float4 world_vertex : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float2 uv : TEXCOORD2;
				float light : COLOR;
			};

			struct structurePS
			{
				half4 albedo : SV_Target0;
				half4 specular : SV_Target1;
				half4 normal : SV_Target2;
				half4 emission : SV_Target3;
			};

			//normals 0 front; 1 back; 2 top; 3 bottom; 4 left; 5 right

			structureVS vertex_shader(float4 vertex : POSITION, uint4 color : COLOR)
			{
				structureVS vs;
				float4 vertexFloat = (float4)vertex;
				vs.screen_vertex = UnityObjectToClipPos(vertexFloat);
				vs.world_vertex = mul(unity_ObjectToWorld, vertexFloat);
				int n = color.b;
				switch (n)
				{
					default:
					case 0: 
						vs.normal = UnityObjectToWorldNormal(float3(0, 0, 1));
						break;
					case 1:
						vs.normal = UnityObjectToWorldNormal(float3(0, 0, -1));
						break;
					case 2:
						vs.normal = UnityObjectToWorldNormal(float3(0, 1, 0));
						break;
					case 3:
						vs.normal = UnityObjectToWorldNormal(float3(0, -1, 0));
						break;
					case 4:
						vs.normal = UnityObjectToWorldNormal(float3(-1, 0, 0));
						break;
					case 5:
						vs.normal = UnityObjectToWorldNormal(float3(1, 0, 0));
						break;
				}
				
				
				//vs.normal = UnityObjectToWorldNormal(normal);
#if TEXTURES_ON
				vs.uv= color.rg / 128.0 * 16;
#else
				vs.uv = color.rg / 16.0 * 16;
#endif
				vs.uv.y = 1 - vs.uv.y;
				vs.light = color.a / 15.0;
				//vs.uv = uv;
				return vs;
			}

			uniform fixed4 _SkyColorHorizon, _SkyColorTop, _SkyColorBottom;
			fixed4 GetSkyColor(float3 viewDir)
			{
				float2 lat = atan2((abs(viewDir.y)), sqrt(viewDir.x*viewDir.x + viewDir.z*viewDir.z));
				float height = pow(2 * lat / 3.141592, 1);
				return lerp(_SkyColorHorizon, lerp(_SkyColorBottom, _SkyColorTop, saturate(sign(viewDir.y))), height);
			}

			uniform sampler2D _BlockTextures, _UVTexture;
			uniform float _MinLightLevel;
			uniform int _RenderDistance;
			structurePS pixel_shader(structureVS vs)
			{
				structurePS ps;
				float3 normalDirection = normalize(vs.normal);
				half3 specular;
				half specularMonochrome;
				half3 diffuseColor = DiffuseAndSpecularFromMetallic(_Color.rgb, _Metallic, specular, specularMonochrome);
				
#if TEXTURES_ON
				fixed4 c = tex2D(_BlockTextures, vs.uv);
#else
				fixed4 c = tex2D(_UVTexture, vs.uv);

#endif
#if VIEW_NORMALS_ON
				c.rgb = vs.normal*0.5 + 0.5f;
#endif
				//c.rgb = 1;
				//c.rgb = float3(vs.uv % 1.0, 0);
				clip(c.a - 0.25);

				fixed4 sky = GetSkyColor(vs.world_vertex - _WorldSpaceCameraPos);

				float fade = saturate(pow(distance(_WorldSpaceCameraPos.xz, vs.world_vertex.xz) / 16.0 / (_RenderDistance - 1.0), 12));
				//float4 vertexColor = vs.color;
				//c.rgb *= vs.normal.rgb*0.5+0.5;

			

				float lightLevel = vs.light;
				float light = lerp(_MinLightLevel, 1, lightLevel);
#if LIGHTING_OFF
				light = 1;
#endif
				//float light = 1;
				c *= light;
				c.rgb += diffuseColor;
				c.a = 1;
				

				ps.albedo = 0;
				ps.specular = half4(specular, 0);
				ps.normal = half4(normalDirection * 0.5 + 0.5, 1.0);
				ps.emission = lerp( c,sky, fade);
				#ifndef UNITY_HDR_ON
					ps.emission.rgb = exp2(-ps.emission.rgb);
				#endif
				return ps;
			}
            ENDCG
        }
    }
}
