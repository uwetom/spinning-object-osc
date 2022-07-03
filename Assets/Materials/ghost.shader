Shader "Custom/ghost" {
     Properties{
         _Color("Main Color", Color) = (1,1,1,1)
         _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
     }
     SubShader{
         Tags {"RenderType" = "Transparent" "Queue" = "Transparent" }
         Blend SrcAlpha OneMinusSrcAlpha
         ZTest Always
         Pass {
             ColorMask 0
         }

         UsePass "Transparent/Diffuse/FORWARD"
     }
     Fallback "Transparent/VertexLit"
 }