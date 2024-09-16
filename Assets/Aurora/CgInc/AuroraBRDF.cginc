//Unity Standard BRDF1 lighting model
half4 AuroraBRDF(half3 albedo, half3 diffColor, half3 specColor, half oneMinusReflectivity, half metallic, half rough,
			half occlusion, float3 normal, float3 viewDir, UnityLight light, UnityIndirect gi, int lightingBypass, int giBoost)
{
	half3 halfDir = Unity_SafeNormalize(light.dir + viewDir);
	//This abs() should help eliminate artifacts
	half NdotV = abs(dot(normal, viewDir));
	float NdotL = saturate(dot(normal, light.dir));
	float NdotH = saturate(dot(normal, halfDir));
	half LdotV = saturate(dot(light.dir, viewDir));
	half LdotH = saturate(dot(light.dir, halfDir));

	float perceptualRoughness = SmoothnessToPerceptualRoughness (1.0 - rough);
	half diffuseTerm = DisneyDiffuse(NdotV, NdotL, LdotH, perceptualRoughness) * NdotL;

	float roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
	roughness = max(roughness, 0.002);

	half3 specular = SmithJointGGXVisibilityTerm(NdotL, NdotV, roughness) * GGXTerm(NdotH, roughness) * UNITY_PI;
	specular *= NdotL;
	specular *= FresnelTerm (specColor, LdotH);

	gi.specular *= occlusion;

	gi.diffuse += (gi.diffuse * giBoost * 2);
	gi.specular += (gi.specular * giBoost * 2);

	half3 color = diffColor * (gi.diffuse + (light.color * diffuseTerm)) + (specular * light.color) + (gi.specular * specColor);
	color *= 1.0 - lightingBypass;
	color += diffColor * lightingBypass;
	return half4(color, 1);
}

//Unity Standard BRDF1 lighting model with alpha support
half4 AuroraBRDF4(half3 albedo, half3 diffColor, half3 specColor, half alpha, half oneMinusReflectivity, half metallic, half rough,
			half occlusion, float3 normal, float3 viewDir, UnityLight light, UnityIndirect gi, int lightingBypass, int giBoost)
{
	half3 halfDir = Unity_SafeNormalize(light.dir + viewDir);
	//This abs() should help eliminate artifacts
	half NdotV = abs(dot(normal, viewDir));
	float NdotL = saturate(dot(normal, light.dir));
	float NdotH = saturate(dot(normal, halfDir));
	half LdotV = saturate(dot(light.dir, viewDir));
	half LdotH = saturate(dot(light.dir, halfDir));

	float perceptualRoughness = SmoothnessToPerceptualRoughness (1.0 - rough);
	half diffuseTerm = DisneyDiffuse(NdotV, NdotL, LdotH, perceptualRoughness) * NdotL;

	float roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
	roughness = max(roughness, 0.002);

	half3 specular = SmithJointGGXVisibilityTerm(NdotL, NdotV, roughness) * GGXTerm(NdotH, roughness) * UNITY_PI;
	specular *= NdotL;
	specular *= FresnelTerm (specColor, LdotH);

	gi.specular *= occlusion;

	gi.diffuse += (gi.diffuse * giBoost * 2);
	gi.specular += (gi.specular * giBoost * 2);

	half3 color = diffColor * (gi.diffuse + (light.color * diffuseTerm)) + (specular * light.color) + (gi.specular * specColor);
	color *= 1.0 - lightingBypass;
	color += diffColor * lightingBypass;
	return half4(color, alpha);
}

//Thanks to RetroGEO - https://github.com/RetroGEO/reroStandard
//Accounts for baked lighting by supplanting the light direction and color with those
//									extracted from the spherical harmonics enivronment
//Allows for baked lights to provide light direction to the shader
//Supplanted light color is taken from the indirect GI (usually light probe color or ambient color/skybox)
half3 AuroraBLSHAccount(UnityGIInput data, inout UnityGI gi, int toggle)
{
	half3 probeLightDir = normalize(mul(unity_ColorSpaceLuminance.rgb, half3x3(unity_SHAr.xyz, unity_SHAg.xyz, unity_SHAb.xyz)));

    gi.light.dir += probeLightDir * step(length(_WorldSpaceLightPos0), 0.01) * toggle;
	gi.light.color += gi.indirect.diffuse * step(length(_WorldSpaceLightPos0), 0.01) * toggle;

    #if defined(VERTEXLIGHT_ON)
		half3 vertexLightDir = getVertexLightsDir(data.worldPos);
        gi.light.dir += (probeLightDir + vertexLightDir) * step(length(_WorldSpaceLightPos0), 0.01) * toggle;
    #endif

	return probeLightDir;
}