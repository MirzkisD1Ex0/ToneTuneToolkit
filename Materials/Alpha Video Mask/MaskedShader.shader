Shader "Custom/MaskedShader" 
{
    Properties 
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaVideo ("Alpha Video", 2D) = "white" {}
    }
    SubShader 
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
 
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            sampler2D _AlphaVideo;
            float4 _MainTex_ST;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed alpha = tex2D(_AlphaVideo, i.uv).r; // Assuming the alpha is in the red channel
                return fixed4(col.rgb, alpha);
            }
            ENDCG
        }
    }
}