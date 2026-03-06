#ifndef EPSILON
	#define EPSILON 0.0000001
#endif

uniform uint _ViewSpecularEnabled;
uniform fixed _ViewSpecular;
uniform fixed _ViewSpecularGain;
uniform fixed _ViewSpecularSpecSaturation;
uniform fixed _ViewSpecularSpecValue;
uniform fixed _ViewSpecularRoughnessTerm;
uniform fixed _ViewSpecularColorMixing;
uniform fixed4 _ViewSpecularColor;
uniform fixed _RimLightingPower;
uniform fixed _RimLightingStrength;
uniform fixed _RimLightingDiffuseInfluence;
uniform fixed _RimLightingColorInfluence;
uniform fixed4 _RimLightingColor;
uniform fixed _ACELDiffuseStrength;
uniform fixed _ACELSpecularStrength;
uniform fixed _ACELAmbientDiffuseStrength;
uniform fixed _ACELAmbientSpecularStrength;
uniform fixed _ACELOutlineStrength;
uniform fixed _ACELOutlineThreshold;

float3 OrenNayarDiffuse(float LdotV, float NdotL, float NdotV, float roughness, float3 albedo)
{
	float surface = LdotV - NdotL * NdotV;
    float ti = lerp(1.0, max(NdotL, NdotV), step(0.0, surface));

	float roughSqr = roughness * roughness;
	float A = 1.0 + roughSqr * (albedo / (roughSqr + 0.13) + 0.5 / (roughSqr + 0.33));
	float B = 0.45 * roughSqr / (roughSqr + 0.09);

	return albedo * max(0.0, NdotL) * (A + B * surface / ti) / UNITY_PI;
}

// Thank you to jvo3dc
// https://discussions.unity.com/t/am-i-calculating-my-physically-based-shader-correctly/661225/3
float OrenNayar(float3 n, float3 v, float3 l, float roughness)
{
   float roughness2 = roughness * roughness;
   float2 oren_nayar_fraction = roughness2 / (roughness2 + float2(0.33, 0.09));
   float2 oren_nayar = float2(1, 0) + float2(-0.5, 0.45) * oren_nayar_fraction;
   // Theta and phi
   float2 cos_theta = saturate(float2(dot(n, l), dot(n, v)));
   float2 cos_theta2 = cos_theta * cos_theta;
   float sin_theta = sqrt((1-cos_theta2.x) * (1-cos_theta2.y));
   float3 light_plane = normalize(l - cos_theta.x * n);
   float3 view_plane = normalize(v - cos_theta.y * n);
   float cos_phi = saturate(dot(light_plane, view_plane));
   // Composition
   float diffuse_oren_nayar = cos_phi * sin_theta / max(cos_theta.x, cos_theta.y);
   float diffuse = cos_theta.x * (oren_nayar.x + oren_nayar.y * diffuse_oren_nayar);
   return diffuse;
}

float CookTorrance_GF (float NdotL, float NdotV, float VdotH, float NdotH)
{
	return min(1.0, min(2*NdotH*NdotV / VdotH, 2*NdotH*NdotL / VdotH));
}

float GGX_DF(float3 normal, float3 halfDir, float roughness)
{
	float NdotIH = dot(normal, -halfDir);
	float rough2 = roughness * roughness;
	float Nh2 = NdotIH * NdotIH;
	float density = Nh2 * rough2 + (1 - Nh2);
	return (step(NdotIH, EPSILON) * rough2) / (UNITY_PI * density * density);
}

float GGX_GF(float3 normal, float3 halfDir, float3 viewDir, float roughness)
{
	float IVdotIH = dot(viewDir, -halfDir);
	float IVDotN = dot(viewDir, normal);
	float chi = step(IVdotIH / IVDotN, EPSILON);
	float Vh2 = IVdotIH * IVdotIH;
	float tan2 = (1.0 - Vh2) / Vh2;
	return (chi * 2.0) / (1.0 + sqrt(1.0 + roughness * roughness * tan2));
}

//Unity Standard BRDF1 lighting model with alpha support
half4 AuroraBRDF4(half3 albedo, half3 diffColor, half3 specColor, half alpha, half oneMinusReflectivity, half metallic, half rough,
			half occlusion, float3 normal, float3 viewDir, UnityLight light, UnityIndirect gi, int lightingBypass, float giBoost,  float radianceScaling)
{
	half3 halfDir = Unity_SafeNormalize(light.dir + viewDir);
	//This abs() should help eliminate artifacts
	half NdotV = abs(dot(normal, viewDir));
	float NdotL = saturate(dot(normal, light.dir));
	float NdotH = saturate(dot(normal, halfDir));
	half LdotV = saturate(dot(light.dir, viewDir));
	half LdotH = saturate(dot(light.dir, halfDir));

	rough = max(1.055 * pow(rough, 0.416666667) - 0.055, 0); //linear to sgrb conversion ??
	float perceptualRoughness = SmoothnessToPerceptualRoughness (1.0 - rough);
	half diffuseTerm = DisneyDiffuse(NdotV, NdotL, LdotH, perceptualRoughness) * NdotL;

	float roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
	roughness = max(roughness, 0.002);

	half3 specular = SmithJointGGXVisibilityTerm(NdotL, NdotV, roughness) * GGXTerm(NdotH, roughness) * UNITY_PI;
	specular *= NdotL;
	specular *= FresnelTerm (specColor, LdotH);
	specular *= radianceScaling;

	gi.specular *= occlusion;

	gi.diffuse += (gi.diffuse * giBoost);
	gi.specular += (gi.specular * giBoost) * radianceScaling;

	half3 color = diffColor * (gi.diffuse + (light.color * diffuseTerm)) + (specular * light.color) + (gi.specular * specColor);
	color *= 1.0 - lightingBypass;
	color += diffColor * lightingBypass;
	return half4(color, alpha);
}

half4 AuroraBRDF5(half3 albedo, half3 diffColor, half3 specColor, half alpha, half oneMinusReflectivity, half metallic, half roughness,
			half occlusion, float3 normal, float3 viewDir, UnityLight light, UnityIndirect gi, int lightingBypass, float giBoost, float radianceScaling)
{
	// SETUP
	half3 halfDir = Unity_SafeNormalize(light.dir + viewDir);
	half NdotV = abs(dot(normal, viewDir));
	float NdotL = saturate(dot(normal, light.dir));
	float NdotH = saturate(dot(normal, halfDir));
	half LdotH = saturate(dot(light.dir, halfDir));
	half VdotH = saturate(dot(viewDir, halfDir));
	half3 viewRefl = reflect(viewDir, normal);

	float3 fresnelTerm = FresnelTerm (specColor, LdotH);
	half3 output = half3(0,0,0);

	// ROUGHNESS
	roughness = max(roughness * roughness, 0.002);

	// DIRECT LIGHTING
	float orenNayarDiffuse = max(OrenNayar(normal, viewDir, light.dir, roughness), 0.0);
	float3 diffuseLighting =  diffColor * orenNayarDiffuse * light.color;

	float3 specularLighting = CookTorrance_GF(NdotL, NdotV, VdotH, NdotH) * GGX_DF(normal, halfDir, roughness) * fresnelTerm;
	specularLighting *= NdotL;
	specularLighting *= light.color;
	specularLighting = max(specularLighting, 0.0);
	specularLighting *= radianceScaling;

	float3 directLighting = diffuseLighting + specularLighting;

	// INDIRECT LIGHTING
	half grazingTerm = saturate((1.0 - roughness) + (1-oneMinusReflectivity));
	float3 indirectDiffuse = gi.diffuse * diffColor;
	float3 indirectSpecular = (gi.specular + (gi.specular * giBoost)) * specColor;
	indirectSpecular *= radianceScaling;

	float3 indirectLighting = indirectDiffuse + indirectSpecular;

	// INVERSE GLOSS
	half inverseGlossTerm = (1.0 - (abs(dot(viewRefl, viewDir)) + (0.5 * dot(lerp( viewRefl, normal, 0), viewDir) + 0.5 / _ViewSpecularGain))) * _ViewSpecular;
	half3 igSpecColor = rgb2hsv(specColor);
	igSpecColor = hsv2rgb(half3(specColor.r, lerp(specColor.g, 1.0, _ViewSpecularSpecSaturation), lerp(specColor.b, 1.0, _ViewSpecularSpecValue)));
	half3 inverseGloss = lerp(igSpecColor, _ViewSpecularColor.rgb, _ViewSpecularColorMixing) * max(0.0, inverseGlossTerm) * radianceScaling * occlusion * lerp(1.0, grazingTerm, _ViewSpecularRoughnessTerm);

	output = directLighting + indirectLighting + (inverseGloss * _ViewSpecularEnabled);

	return half4(output, 1.0);
}

half FakeOutlineEffect(float i, float t)
{
	float x = clamp(i, 0.0, 1.0);
    float x2 = x*x;
    return step(t, x2*x2*x);
}

half4 AuroraCel(half3 albedo, half3 diffColor, half3 specColor, half alpha, half oneMinusReflectivity, half metallic, half roughness,
			half occlusion, float3 normal, float3 viewDir, UnityLight light, UnityIndirect gi, int lightingBypass, float giBoost, float radianceScaling)
{
	half3 halfDir = Unity_SafeNormalize(light.dir + viewDir);
	half NdotV = abs(dot(normal, viewDir));
	float NdotL = saturate(dot(normal, light.dir));
	float NdotH = saturate(dot(normal, halfDir));
	half LdotH = saturate(dot(light.dir, halfDir));
	half VdotH = saturate(dot(viewDir, halfDir));
	half3 viewRefl = reflect(viewDir, normal);

	half3 output = half3(0,0,0);
	roughness = max(roughness * roughness, 0.002);

	float lightIntensity = smoothstep(0.005, 0.01, NdotL);
	lightIntensity += smoothstep(0.0025, 0.005, NdotL) * 0.5;
	lightIntensity += smoothstep(0.0, 0.0025, NdotL) * 0.25;
	float3 diffuseLighting = light.color * lightIntensity * _ACELDiffuseStrength;

	half3 specular = SmithJointGGXVisibilityTerm(NdotL, NdotV, roughness) * GGXTerm(NdotH, roughness) * UNITY_PI;
	float specularIntensitySmooth = smoothstep(0.1, 0.2, specular.r);
	specularIntensitySmooth += smoothstep(0.05, 0.1, specular.r) * 0.5;
	specularIntensitySmooth += smoothstep(0.025, 0.05, specular.r) * 0.25;
	specular *= NdotL;
	specular *= FresnelTerm (specColor, LdotH);
	specular *= radianceScaling;
	half3 specularLighting = specular * specularIntensitySmooth * _ACELSpecularStrength;

	float rimDotLighting = 1.0 - dot(viewDir, normal);
	float rimIntensity = rimDotLighting * pow(NdotL, _RimLightingPower);
	float3 rimLight = rimIntensity * _RimLightingStrength * radianceScaling * lightIntensity;
	rimLight = lerp(rimLight, diffColor * rimLight, _RimLightingDiffuseInfluence);
	rimLight = lerp(rimLight, _RimLightingColor * rimLight, _RimLightingColorInfluence);

	float ambientLightIntensity = smoothstep(0, 0.01, saturate(gi.diffuse));
	float3 ambientDiffuse = gi.diffuse * ambientLightIntensity * _ACELAmbientDiffuseStrength;

	float ambientSpecularIntensity = smoothstep(0.1, 0.2, saturate(gi.specular));
	float3 ambientSpecular = gi.specular * ambientSpecularIntensity * metallic * _ACELAmbientSpecularStrength;

	float3 ambientValue = (diffColor * ambientDiffuse) + (specColor * ambientSpecular);

	output = diffColor * diffuseLighting + specularLighting + ambientValue + rimLight;

	half grazingTerm = saturate((1.0 - roughness) + (1-oneMinusReflectivity));

	// INVERSE GLOSS
	half inverseGlossTerm = (1.0 - (abs(dot(viewRefl, viewDir)) + (0.5 * dot(lerp( viewRefl, normal, 0), viewDir) + 0.5 / _ViewSpecularGain))) * _ViewSpecular;
	half3 igSpecColor = rgb2hsv(specColor);
	igSpecColor = hsv2rgb(half3(specColor.r, lerp(specColor.g, 1.0, _ViewSpecularSpecSaturation), lerp(specColor.b, 1.0, _ViewSpecularSpecValue)));
	half3 inverseGloss = lerp(igSpecColor, _ViewSpecularColor.rgb, _ViewSpecularColorMixing) * max(0.0, inverseGlossTerm) * radianceScaling * occlusion * lerp(1.0, grazingTerm, _ViewSpecularRoughnessTerm);

	return half4(output + (inverseGloss * _ViewSpecularEnabled), 1);
}

half3 GetProbeLightDir()
{
	return normalize(mul(unity_ColorSpaceLuminance.rgb, half3x3(unity_SHAr.xyz, unity_SHAg.xyz, unity_SHAb.xyz)));
}

// Thanks to RetroGEO - https://github.com/RetroGEO/reroStandard
// Accounts for baked lighting (in complete abscence of realtime lighting) by supplanting the light direction
// and color with those extracted from the spherical harmonics enivronment
// Allows for baked lights to provide light direction to the shader
// Supplanted light color is taken from the indirect GI (usually light probe color or ambient color/skybox)
half3 AuroraBLSHAccount(UnityGIInput data, inout UnityGI gi, int toggle)
{
	half3 probeLightDir = GetProbeLightDir();

    gi.light.dir += probeLightDir * step(length(_WorldSpaceLightPos0), 0.01) * toggle;
	gi.light.color += gi.indirect.diffuse * step(length(_WorldSpaceLightPos0), 0.01) * toggle;

    #if defined(VERTEXLIGHT_ON)
		half3 vertexLightDir = getVertexLightsDir(data.worldPos);
        gi.light.dir += (probeLightDir + vertexLightDir) * step(length(_WorldSpaceLightPos0), 0.01) * toggle;
    #endif

	return probeLightDir;
}