/* square value */                        float sqr(float v) { return v * v; }
/* cube   value */                        float cube(float v) { return v * v * v; }
/* smooth step within [min,max] */        float sstep(float min, float max, float width) { return smoothstep(min - width, min + width, max); }
/* remap value from [min,max] to [0,1] */ float remap01(float v, float min, float max) { return (v - min) / (max - min); }
/* saturate or clamp to [0,1] */          float sat(float v) { return saturate(v); }


// https://www.iquilezles.org/www/articles/functions/functions.htm
float exp_impulse( float k, float x )
{
    const float h = k*x;
    return h*exp(1.0-h);
}
float qua_impulse(float k, float x) { return 2.0*sqrt(k)*x/(1.0+k*x*x); }
float poly_impulse(float k, float n, float x) { return (n/(n-1.0))*pow((n-1.0)*k,1.0/n) * x/(1.0+k*pow(x,n));}