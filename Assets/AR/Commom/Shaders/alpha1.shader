Shader "Custom/alpha1" {

    Properties{
         _MainTex("Color (RGB)", 2D) = "white"
    }
        SubShader{
             Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
        Cull Off
             CGPROGRAM
             #pragma surface surf NoLighting alpha 
             fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
                  fixed4 c;
                  c.rgb = s.Albedo;
                  c.a = s.Alpha;
                  return c;
             }
             struct Input {
                  float2 uv_MainTex;
             };
             sampler2D _MainTex;
             void surf(Input IN, inout SurfaceOutput o) {
                  o.Emission = tex2D(_MainTex, IN.uv_MainTex).rgb;

                 if (IN.uv_MainTex.y <= 0.5) {
                    o.Alpha = 0;
                 }
                    

                 else {
                      o.Alpha = tex2D(_MainTex, float2(IN.uv_MainTex.x, IN.uv_MainTex.y - 0.5)).rgb;
                 }

             }

             ENDCG
    }
}