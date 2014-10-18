// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Adapted for COMP30019 by Jeremy Nicholson, 10 Sep 2012
// Adapted further by Chris Ewin, 23 Sep 2013

// these won't change in a given iteration of the shader
float4x4 World;
float4x4 View;
float4x4 Projection;
float4 cameraPos;
float4 lightAmbCol = float4(0.4f, 0.4f, 0.4f, 1.0f);
float4 lightPntPos = float4(0.0f, 0.0f, -2.0f, 1.0f);
float4 lightPntCol = float4(1.0f, 1.0f, 1.0f, 1.0f);
float4x4 worldInvTrp;
//

struct VS_IN
{
	float4 pos : SV_POSITION;
	float4 nrm : NORMAL;
	float4 col : COLOR;
// Other vertex properties, e.g. texture co-ords, surface Kd, Ks, etc
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;

	// Convert Vertex position and corresponding normal into world coords
	// Note that we have to multiply the normal by the transposed inverse of the world 
	// transformation matrix (for cases where we have non-uniform scaling; we also don't
	// care about the "fourth" dimension, because translations don't affect the normal) 
	float4 worldVertex = mul(input.pos, World);
	float3 worldNormal = normalize(mul(input.nrm.xyz, (float3x3)worldInvTrp));

	// Calculate ambient RGB intensities
	float Ka = 1;
	float3 amb = input.col.rgb*lightAmbCol.rgb*Ka;

	// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
	// (when calculating the reflected ray in our specular component)
	float fAtt = 1;
	float Kd = 1;
	float3 L = normalize(lightPntPos.xyz - worldVertex.xyz);
	float LdotN = saturate(dot(L,worldNormal.xyz));
	float3 dif = fAtt*lightPntCol.rgb*Kd*input.col.rgb*LdotN;

	// Calculate specular reflections
	float Ks = 1;
	float specN = 5; // Values>>1 give tighter highlights
	float3 V = normalize(cameraPos.xyz - worldVertex.xyz);
	float3 R = normalize(2*LdotN*worldNormal.xyz - L.xyz);
	//float3 R = normalize(0.5*(L.xyz+V.xyz)); //Blinn-Phong equivalent
	float3 spe = fAtt*lightPntCol.rgb*Ks*pow(saturate(dot(V,R)),specN);

	// Combine reflection components
	output.col.rgb = amb.rgb+dif.rgb+spe.rgb;
	output.col.a = input.col.a;

	// Transform vertex in world coordinates to camera coordinates
	float4 worldPos = mul(input.pos, World);
    float4 viewPos = mul(worldPos, View);
    output.pos = mul(viewPos, Projection);

	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	return input.col;
}


technique Lighting
{
    pass Pass1
    {
		Profile = 9.1;
        VertexShader = VS;
        PixelShader = PS;
    }
}