#pragma once
// 移植自 Materials/zcam-unity-ssp-windows/Native/windows/h264_config.h（另一 Agent 交付，2026-09-28）
// SPS+VUI 解析：分辨率/矩阵/范围/逐行等元数据。原版对不支持配置返回 false（拒收）；
// 本项目用作"取元数据"，解析失败时回退默认（BT.709 limited），不拒流。
#include "zcam_stream_core.h"
namespace zcam {
struct H264Config {
 uint32_t width=0,height=0,matrix=0,transfer=2,primaries=2;
 // -1 means absent, 0 limited, 1 full. A missing VUI is never guessed from resolution.
 int fullRange=-1;bool progressive=false;
};
inline int32_t signedExp(Bits& b){auto n=b.ue();return (n&1)?int32_t((n+1)/2):-int32_t(n/2);}
inline bool parseConfig(const Bytes& nal,H264Config& out){
 if(!supportedSps(nal))return false;
 Bits b(nal);uint32_t profile=b.read(8);b.read(16);b.ue();
 if(profile==100){
  if(b.ue()!=1 || b.ue()!=0 || b.ue()!=0)return false;b.read(1);
  if(b.read(1))for(int i=0;i<8;++i)if(b.read(1)){
   int last=8,next=8;for(int j=0;j<(i<6?16:64);++j){if(next)next=(int64_t(last)+signedExp(b)+256)%256;last=next?next:last;}
  }
 }
 b.ue();auto poc=b.ue();
 if(poc==0)b.ue();else if(poc==1){b.read(1);signedExp(b);signedExp(b);auto n=b.ue();if(n>255)return false;for(uint32_t i=0;i<n;++i)signedExp(b);}else if(poc!=2)return false;
 b.ue();b.read(1);uint64_t wm=b.ue(),hm=b.ue();if(wm>255 || hm>143)return false;
 out.progressive=b.read(1)!=0;if(!out.progressive)return false;b.read(1);
 uint64_t left=0,right=0,top=0,bottom=0;
 if(b.read(1)){left=b.ue();right=b.ue();top=b.ue();bottom=b.ue();}
 uint64_t w=(wm+1)*16,h=(hm+1)*16;if(2*(left+right)>=w || 2*(top+bottom)>=h)return false;
 // Only zero left/top crop is supported by the GPU sampling path.
 if(left || top)return false;
 out.width=(uint32_t)(w-2*(left+right));out.height=(uint32_t)(h-2*(top+bottom));
 if(b.read(1)){
  if(b.read(1)){auto aspect=b.read(8);if(aspect==255){auto sw=b.read(16),sh=b.read(16);if(!sw || sw!=sh)return false;}else if(aspect!=0 && aspect!=1)return false;}
  if(b.read(1))b.read(1);
  if(b.read(1)){b.read(3);out.fullRange=(int)b.read(1);if(b.read(1)){out.primaries=b.read(8);out.transfer=b.read(8);auto m=b.read(8);if(m!=1 && m!=2 && m!=5 && m!=6)return false;out.matrix=m==1?1:((m==5 || m==6)?2:0);}}
  if(b.read(1)){auto topLocation=b.ue(),bottomLocation=b.ue();if(topLocation!=0 || bottomLocation!=0)return false;}
 }
 if(out.transfer!=1 && out.transfer!=6 && out.transfer!=13 && out.transfer!=2)return false;
 if(out.primaries!=1 && out.primaries!=5 && out.primaries!=6 && out.primaries!=2)return false;
 return b.ok && out.width>=48 && out.height>=48;
}
}
