//----------------------------------
//Hidden Vertex Program
//----------------------------------
float4 hiddenVP() : SV_POSITION
{
	return float4(0.0f, 0.0f, 0.0f, 0.0f);
}

//----------------------------------
//Hidden fragment program
//----------------------------------
float4 hiddenFP() : SV_TARGET
{
	return float4(0.0f, 0.0f, 0.0f, 0.0f);
}