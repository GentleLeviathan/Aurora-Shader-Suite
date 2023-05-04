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

/*//Hybrid Aurora AR2-GGXTerm experiment
half4 AuroraBRDF2(half3 albedo, half3 diffColor, half3 specColor, half oneMinusReflectivity, half metallic, half rough,
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

	half3 albedoHSV = rgb2hsv(albedo);
	half3 specMetalColor = hsv2rgb(half3(albedoHSV.r, albedoHSV.g, 1));
	specMetalColor = EnergyConservationBetweenDiffuseAndSpecular(specMetalColor, albedo, oneMinusReflectivity);

	float specularDistribution = GGXTerm(NdotH, roughness);

	float shadowDistribution = WalterEtAlGSF(NdotL, NdotV, roughness);
	shadowDistribution = pow(shadowDistribution, 3);

	float fresnel = SphericalGFF(LdotH, SpecularStrength(specColor));

	half3 specular = (specularDistribution * fresnel * shadowDistribution) / (1.0 * (NdotL * NdotV));
	specular *= UNITY_PI;
	specular *= FresnelTerm (specColor, LdotH);

	gi.specular *= occlusion;

	gi.diffuse += (gi.diffuse * giBoost * 2);
	gi.specular += (gi.specular * giBoost * 2);

	half3 color = diffColor * (gi.diffuse + (light.color * diffuseTerm)) + (specular * light.color) + (gi.specular * specColor);
	color *= 1.0 - lightingBypass;
	color += diffColor * lightingBypass;
	return half4(color, 1);
}*/