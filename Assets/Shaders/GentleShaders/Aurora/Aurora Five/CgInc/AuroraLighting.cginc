uniform uint _lightingBypass;
uniform uint _accountForBLSH;
uniform fixed _giBoost;
uniform uint _giBoostEnabled;


void LightingAurora_GI (AuroraOutput s, UnityGIInput data, inout UnityGI gi)
{
	#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
		gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
	#else
		//GLOSSY ENVRIONMENT MIPMAP TO SMOOTHNESS CORRELATION
		s.Roughness *= 1.7 - 0.7 * s.Roughness;
		Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(1.0 - s.Roughness, data.worldViewDir, s.Normal, lerp(unity_ColorSpaceDielectricSpec.rgb, s.Albedo, s.Metal));
		gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal, g);
		half3 probeLightDir = AuroraBLSHAccount(data, gi, _accountForBLSH);
	#endif
}

float4 LightingAurora(AuroraOutput s, float3 viewDir, UnityGI gi)
{
	s.Normal = normalize(s.Normal);

	half oneMinusReflectivity;
	half3 specColor;

	s.Metal *= 1.7 - 0.7 * s.Metal;
	half3 diffuse = DiffuseAndSpecularFromMetallic (s.Albedo, s.Metal, /*out*/ specColor, /*out*/ oneMinusReflectivity);

	half4 c;
	#ifdef _BRDF5
		c = AuroraBRDF5(s.Albedo, diffuse, specColor, s.Alpha, oneMinusReflectivity, s.Metal, s.Roughness, s.Occlusion, s.Normal, viewDir, gi.light, gi.indirect, _lightingBypass, _giBoost * _giBoostEnabled, s.RadianceScaling);
	#endif
	#ifdef _BRDF4
		c = AuroraBRDF4(s.Albedo, diffuse, specColor, s.Alpha, oneMinusReflectivity, s.Metal, s.Roughness, s.Occlusion, s.Normal, viewDir, gi.light, gi.indirect, _lightingBypass, _giBoost * _giBoostEnabled, s.RadianceScaling);
	#endif
	#ifdef _ACEL
		c = AuroraCel(s.Albedo, diffuse, specColor, s.Alpha, oneMinusReflectivity, s.Metal, s.Roughness, s.Occlusion, s.Normal, viewDir, gi.light, gi.indirect, _lightingBypass, _giBoost * _giBoostEnabled, s.RadianceScaling);
	#endif
	float3 worldNorm = mul(float4(s.Normal, 0), unity_ObjectToWorld);
	return c;
}