float warp(float impfunc, float beta)
{
  const float alpha = 0.1;
  float expbeta = exp(-beta);
  return (alpha*expbeta+impfunc*(1.0-alpha-alpha*expbeta)) / (alpha+impfunc*(expbeta-alpha-alpha*expbeta));
}

float3 coloredDescriptor(float c, float s)
{
  float3 rgb;
  float3 convMax = float3(0.1,0.1,0.8);
  float3 convMin = float3(0.2,0.2,0.6);
  float3 concMax = float3(0.8,0.1,0.1);
  float3 concMin = float3(0.6,0.2,0.2);
  float3 plane   = float3(0.7,0.7,0.2);
  float t = 0.02;
  float a;

  if(c<-t) {
    a = (-c-t)/(1.0-t);
    rgb = lerp(concMin,concMax,a);
  } else if(c>t) {
    a = (c-t)/(1.0-t);
    rgb = lerp(convMin,convMax,a);
  } else if(c<0.0) {
    a = -c/t;
    rgb = lerp(plane,concMin,a);
  } else {
    a = c/t;
    rgb = lerp(plane,convMin,a);
  }

  if(s<1.0)
    rgb = float3(0.2, 0.2, 0.2);

  return rgb;
}

float3 greyDescriptor(float c, float s)
{
  return (c*0.5+0.5)-(1.0-s);
}

float silhouetteWeight(float s)
{
  const float ts = 0.07;
  const float t2 = 0.9+ts;
  const float t1 = t2-0.01;

  return smoothstep(t1,t2,max(1.0-s,0.9));
}

float tanh(float c, float en)
{
  float cmax = en*15.0;
  const float tanhmax = 3.11622;

  float x = ((c*cmax*1.0)/tanhmax);
  float e = exp(-2.0*x);
  float t = clamp((1.0-e)/(1.0+e),-1.0,1.0);

  return t;
}

float curvature(float w, float3 h, float e)
{
  float c = tanh(-(h.x+h.y)/2.0,e);
  return true ? -c*max(w-0.5,0.0) : c*max(w-0.5,0.0);
}