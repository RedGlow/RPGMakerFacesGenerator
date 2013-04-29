sampler2D input : register(s0);

// new HLSL shader
// modify the comment parameters to reflect your shader parameters

/// <summary>Shift in Hue.</summary>
/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0</defaultValue>
float HueShift : register(C0);

/// <summary>Multiplication factor in Saturation.</summary>
/// <minValue>0</minValue>
/// <maxValue>2</maxValue>
/// <defaultValue>1</defaultValue>
float SaturationFactor : register(C1);

/// <summary>Multiplication factor in Value.</summary>
/// <minValue>0</minValue>
/// <maxValue>2</maxValue>
/// <defaultValue>1</defaultValue>
float ValueFactor : register(C2);

float3 hsv_to_rgb(float3 HSV)
{
	float3 RGB = HSV.z;
	float var_h = HSV.x * 6;
	float var_i = floor(var_h);
	float var_1 = HSV.z * (1.0 - HSV.y);
	float var_2 = HSV.z * (1.0 - HSV.y * (var_h-var_i));
	float var_3 = HSV.z * (1.0 - HSV.y * (1-(var_h-var_i)));
	if      (var_i == 0) { RGB = float3(HSV.z, var_3, var_1); }
	else if (var_i == 1) { RGB = float3(var_2, HSV.z, var_1); }
	else if (var_i == 2) { RGB = float3(var_1, HSV.z, var_3); }
	else if (var_i == 3) { RGB = float3(var_1, var_2, HSV.z); }
	else if (var_i == 4) { RGB = float3(var_3, var_1, HSV.z); }
	else                 { RGB = float3(HSV.z, var_1, var_2); }
	return (RGB);
}

float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
	float4 col = tex2D(input, uv.xy);
	float3 hsv = col.xyz;
	if(hsv.x >= 0.45 && hsv.x <= 0.55)
  {
		hsv.x += HueShift;
		hsv.y *= SaturationFactor;
		hsv.z *= ValueFactor;
	}
	if ( hsv.x > 1.0 ) { hsv.x -= 1.0; }
	return float4(hsv_to_rgb(hsv), col.w);
}