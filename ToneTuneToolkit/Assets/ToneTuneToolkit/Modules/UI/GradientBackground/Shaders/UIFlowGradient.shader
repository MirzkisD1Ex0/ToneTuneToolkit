// Copyright (c) 2026 MirzkisD1Ex0 All rights reserved.
// UI 流动网格渐变 Shader —— 用 4 个品牌色作为游动的色点，程序化生成流动渐变。
// 贴图（如纯白 bgimg）只提供形状/透明度（alpha），颜色完全由本 Shader 生成。

Shader "ToneTune/UIFlowGradient"
{
  Properties
  {
    [PerRendererData] _MainTex ("Sprite Texture (shape/alpha)", 2D) = "white" {}

    // 4 个品牌色（默认 Google 主色）
    _Color0 ("Color 0", Color) = (0.9843, 0.2314, 0.1529, 1) // #FB3B27 红
    _Color1 ("Color 1", Color) = (0.2353, 0.5255, 0.9882, 1) // #3C86FC 蓝
    _Color2 ("Color 2", Color) = (0.9961, 0.7333, 0.0000, 1) // #FEBB00 黄
    _Color3 ("Color 3", Color) = (0.0588, 0.6392, 0.3255, 1) // #0FA353 绿

    _FlowSpeed ("Flow Speed", Range(0,3)) = 0.35   // 流动速度
    _FlowScale ("Flow Amplitude", Range(0,0.5)) = 0.28 // 色点游动幅度
    _Sharpness ("Blend Sharpness", Range(1,8)) = 3.0   // 混色锐度，越大颜色分区越清晰
    _EdgeSoftness ("Edge AA Width", Range(0.2,3)) = 1.0 // 边缘抗锯齿带宽，糊了调小，仍有锯齿调大
    _Alpha ("Master Alpha", Range(0,1)) = 1

    _StencilComp ("Stencil Comparison", Float) = 8
    _Stencil ("Stencil ID", Float) = 0
    _StencilOp ("Stencil Operation", Float) = 0
    _StencilWriteMask ("Stencil Write Mask", Float) = 255
    _StencilReadMask ("Stencil Read Mask", Float) = 255
    _ColorMask ("Color Mask", Float) = 15

    [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
  }

  SubShader
  {
    Tags
    {
      "Queue"="Transparent"
      "IgnoreProjector"="True"
      "RenderType"="Transparent"
      "PreviewType"="Plane"
      "CanUseSpriteAtlas"="True"
    }

    Stencil
    {
      Ref [_Stencil]
      Comp [_StencilComp]
      Pass [_StencilOp]
      ReadMask [_StencilReadMask]
      WriteMask [_StencilWriteMask]
    }

    Cull Off
    Lighting Off
    ZWrite Off
    ZTest [unity_GUIZTestMode]
    Blend SrcAlpha OneMinusSrcAlpha
    ColorMask [_ColorMask]

    Pass
    {
      Name "Default"
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 2.0

      #include "UnityCG.cginc"
      #include "UnityUI.cginc"

      #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
      #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

      struct appdata_t
      {
        float4 vertex   : POSITION;
        float4 color    : COLOR;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct v2f
      {
        float4 vertex        : SV_POSITION;
        fixed4 color         : COLOR;
        float2 texcoord      : TEXCOORD0;
        float4 worldPosition : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
      };

      sampler2D _MainTex;
      float4 _MainTex_ST;
      float4 _ClipRect;

      fixed4 _Color0, _Color1, _Color2, _Color3;
      float _FlowSpeed, _FlowScale, _Sharpness, _EdgeSoftness, _Alpha;

      // 计算某个色点当前的游动位置
      float2 FlowPoint(float2 basePos, float t, float2 freq, float2 phase)
      {
        return basePos + _FlowScale * float2(sin(t * freq.x + phase.x),
                                             cos(t * freq.y + phase.y));
      }

      // 反距离加权：越近的色点权重越高
      float Weight(float2 uv, float2 p)
      {
        float d = distance(uv, p);
        return 1.0 / (pow(d, _Sharpness) + 1e-4);
      }

      v2f vert(appdata_t v)
      {
        v2f OUT;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
        OUT.worldPosition = v.vertex;
        OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
        OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
        OUT.color = v.color;
        return OUT;
      }

      fixed4 frag(v2f IN) : SV_Target
      {
        float2 uv = IN.texcoord;
        float t = _Time.y * _FlowSpeed;

        // 4 个色点在四个象限附近各自游动，频率/相位错开 → 无规律的流动感
        float2 p0 = FlowPoint(float2(0.25, 0.30), t, float2(0.90, 1.10), float2(0.0, 0.0));
        float2 p1 = FlowPoint(float2(0.75, 0.25), t, float2(1.20, 0.80), float2(1.7, 3.1));
        float2 p2 = FlowPoint(float2(0.30, 0.75), t, float2(0.70, 1.30), float2(4.2, 1.3));
        float2 p3 = FlowPoint(float2(0.75, 0.75), t, float2(1.10, 0.95), float2(2.5, 5.0));

        float w0 = Weight(uv, p0);
        float w1 = Weight(uv, p1);
        float w2 = Weight(uv, p2);
        float w3 = Weight(uv, p3);
        float wsum = w0 + w1 + w2 + w3;

        float3 grad = (_Color0.rgb * w0 + _Color1.rgb * w1 +
                       _Color2.rgb * w2 + _Color3.rgb * w3) / wsum;

        // 贴图只取 alpha 作形状；对边缘做解析抗锯齿，任意旋转/缩放都平滑无锯齿
        half rawA = tex2D(_MainTex, uv).a;
        half aaW = fwidth(rawA) * _EdgeSoftness + 1e-5;     // 边缘过渡带宽（按屏幕像素斜率自适应）
        half texAlpha = smoothstep(0.5 - aaW, 0.5 + aaW, rawA);

        // 顶点色 alpha 支持 CanvasGroup 淡入淡出
        half4 color = half4(grad, texAlpha * IN.color.a * _Alpha);

        #ifdef UNITY_UI_CLIP_RECT
        color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
        #endif

        #ifdef UNITY_UI_ALPHACLIP
        clip(color.a - 0.001);
        #endif

        return color;
      }
      ENDCG
    }
  }
}
