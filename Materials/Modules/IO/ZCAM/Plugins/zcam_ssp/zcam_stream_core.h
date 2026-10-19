#pragma once
// 移植自 Materials/zcam-unity-ssp-windows/Native/stream_core.h（另一 Agent 交付，2026-09-28）
// Annex-B/AVCC NAL 拆包 + SPS 白名单检查 + 去 emulation prevention 的位读取器
#include <cstdint>
#include <vector>
#include <cstddef>
namespace zcam {
using Bytes = std::vector<uint8_t>;
inline size_t startCode(const uint8_t* p, size_t n, size_t i) {
    if(i+3<=n && p[i]==0 && p[i+1]==0) {
        if(p[i+2]==1) return 3;
        if(i+4<=n && p[i+2]==0 && p[i+3]==1) return 4;
    }
    return 0;
}
inline bool valid(const Bytes& b) { return !b.empty() && !(b[0]&128) && (b[0]&31)>0 && (b[0]&31)<24; }
inline std::vector<Bytes> splitNal(const uint8_t* p, size_t n) {
    std::vector<Bytes> out;
    if(!p || !n || n>8*1024*1024) return out;
    // A 4-byte AVCC length of 256..511 also starts with 00 00 01.
    // Prefer AVCC only if every length/header validates and consumes the entire input.
    if(n>=5 && p[0]==0) {
        std::vector<Bytes> candidate;size_t i=0;bool complete=true;
        while(i<n) {
            if(n-i<4){complete=false;break;}
            uint32_t len=(uint32_t(p[i])<<24)|(uint32_t(p[i+1])<<16)|(uint32_t(p[i+2])<<8)|p[i+3];i+=4;
            if(!len || len>n-i){complete=false;break;}
            Bytes b(p+i,p+i+len);if(!valid(b)){complete=false;break;}
            candidate.push_back(std::move(b));i+=len;
        }
        if(complete && i==n && !candidate.empty())return candidate;
    }
    if(startCode(p,n,0)) {
        size_t i=0;
        while(i<n) {
            auto sc=startCode(p,n,i); if(!sc) return {};
            size_t begin=i+sc, end=begin;
            while(end<n && !startCode(p,n,end)) ++end;
            size_t trim=end; while(trim>begin && p[trim-1]==0) --trim;
            Bytes b(p+begin,p+trim); if(!valid(b)) return {};
            out.push_back(std::move(b)); i=end;
        }
    } else if(p[0]==0) {
        size_t i=0;
        while(i<n) {
            if(n-i<4) return {};
            uint32_t len=(uint32_t(p[i])<<24)|(uint32_t(p[i+1])<<16)|(uint32_t(p[i+2])<<8)|p[i+3]; i+=4;
            if(!len || len>n-i) return {};
            Bytes b(p+i,p+i+len); if(!valid(b)) return {}; out.push_back(std::move(b)); i+=len;
        }
    } else { Bytes b(p,p+n); if(!valid(b)) return {}; out.push_back(std::move(b)); }
    return out;
}
struct Bits {
    Bytes b; size_t bit=0; bool ok=true;
    explicit Bits(const Bytes& nal) {
        int zeros=0;
        for(size_t i=1;i<nal.size();++i) { auto v=nal[i]; if(zeros==2 && v==3){zeros=0;continue;} b.push_back(v);zeros=v==0?zeros+1:0; }
    }
    uint32_t read(unsigned n) { uint32_t v=0; if(n>32 || bit+n>b.size()*8){ok=false;return 0;} while(n--){v=(v<<1)|((b[bit/8]>>(7-bit%8))&1);++bit;}return v; }
    uint32_t ue(){unsigned z=0;while(ok && !read(1)){if(++z>30){ok=false;return 0;}} auto v=read(z);return ((1u<<z)-1)+v;}
};
inline bool supportedSps(const Bytes& n) {
    if(n.size()<4) return false;
    Bits b(n);auto profile=b.read(8);b.read(16);b.ue();
    if(profile==100) {if(b.ue()!=1) return false;if(b.ue()!=0 || b.ue()!=0) return false;}
    else if(profile!=66 && profile!=77 && profile!=88) return false;
    return b.ok;
}
inline bool isBSlice(const Bytes& n) { Bits b(n);b.ue();auto t=b.ue();return !b.ok || t%5==1; }
}
