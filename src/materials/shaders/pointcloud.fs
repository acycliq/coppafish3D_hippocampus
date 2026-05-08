
#if defined paraboloid_point_shape
	#extension GL_EXT_frag_depth : enable
#endif

precision highp float;
precision highp int;

uniform mat4 viewMatrix;
uniform mat4 uViewInv;
uniform mat4 uProjInv;
uniform vec3 cameraPosition;


uniform mat4 projectionMatrix;
uniform float uOpacity;

uniform float blendHardness;
uniform float blendDepthSupplement;
uniform float fov;
uniform float uSpacing;
uniform float near;
uniform float far;
uniform float uPCIndex;
uniform float uScreenWidth;
uniform float uScreenHeight;

varying vec3	vColor;
varying float	vLogDepth;
varying vec3	vViewPosition;
varying float	vRadius;
varying float 	vPointSize;
varying vec3 	vPosition;
varying float   vPointSourceID;
varying float   vClassification;


float specularStrength = 1.0;
float PI = 3.14159265359;

float lineSegment(vec2 p, vec2 a, vec2 b) {
    float thickness = 1.0/100.0;
    vec2 pa = p - a, ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return step(0.05, length(pa - ba*h));
}

mat2 rot(float a){
    return mat2( cos(a), sin(a), -sin(a), cos(a));
}

void main() {

	// gl_FragColor = vec4(vColor, 1.0);

	vec3 color = vColor;
	float depth = gl_FragCoord.z;

	#if defined color_type_indices
		gl_FragColor = vec4(color, uPCIndex / 255.0);
	#else
		gl_FragColor = vec4(color, uOpacity);
	#endif

	#if defined(circle_point_shape) || defined(paraboloid_point_shape) 
		float u = 2.0 * gl_PointCoord.x - 1.0;
		float v = 2.0 * gl_PointCoord.y - 1.0;
	#endif
	
	#if defined(circle_point_shape)
        if (-vViewPosition.z > 800.0){
            // points at the far back
            gl_FragColor = vec4(vColor.r, vColor.g, vColor.b, gl_FragColor.a);
        }
        else {
            float cc = u*u + v*v;
//             if(cc > 1.0){
//                 discard;
//             }

            if(vClassification == 0.0){
            // draw the asterisk glyph
                float diag_1 = 1.0 - lineSegment(gl_PointCoord, vec2(0.30, 0.30), vec2(0.70, 0.70));
                float diag_2 = 1.0 - lineSegment(gl_PointCoord, vec2(0.30, 0.70), vec2(0.70, 0.30));
                float cross = diag_1 + diag_2;

                float vertical = 1.0 - lineSegment(gl_PointCoord, vec2(0.5, 0.05), vec2(0.5, 0.95));
                float horizontal = 1.0 - lineSegment(gl_PointCoord, vec2(0.10, 0.5), vec2(0.90, 0.5));
                float plus = vertical + horizontal;

                float shaper = plus + cross;

                gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
                if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
            }

            if(vClassification == 1.0){
            // draw the circle glyph
                if(cc > 1.0){
                    discard;
                }
                if(cc < 0.65){
                    discard;
                }
            }

            if (vClassification == 2.0){
            // draw the cross glyph
                float diag_1 = 1.0 - lineSegment(gl_PointCoord, vec2(0.05, 0.05), vec2(0.95, 0.95));
                float diag_2 = 1.0 - lineSegment(gl_PointCoord, vec2(0.05, 0.95), vec2(0.95, 0.05));
                float shaper = diag_1 + diag_2;
                gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
                if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
            }

            if (vClassification == 3.0){
            // draw the diamond glyph
                float side_1 = 1.0 - lineSegment(gl_PointCoord, vec2(0.5, 0.05), vec2(0.9, 0.5));
                float side_2 = 1.0 - lineSegment(gl_PointCoord, vec2(0.9, 0.5), vec2(0.5, 0.95));
                float side_3 = 1.0 - lineSegment(gl_PointCoord, vec2(0.5, 0.95), vec2(0.1, 0.5));
                float side_4 = 1.0 - lineSegment(gl_PointCoord, vec2(0.1, 0.5), vec2(0.5, 0.05));

                float shaper = side_1 + side_2 + side_3 + side_4;
                gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
                if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
            }

            if (vClassification == 4.0){
            // draw the plus glyph
                float vertical = 1.0 - lineSegment(gl_PointCoord, vec2(0.5, 0.05), vec2(0.5, 0.95));
                float horizontal = 1.0 - lineSegment(gl_PointCoord, vec2(0.05, 0.5), vec2(0.95, 0.5));
                float shaper = vertical + horizontal;
                gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
                if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
            }

            if (vClassification == 5.0){
            // draw the point glyph
                if(cc > 0.35){
                    discard;
                }
                if(cc < 0.05){
                    discard;
                }


            }

            if (vClassification == 6.0){
            // draw the square glyph
            	float top = 1.0 - lineSegment(gl_PointCoord, vec2(0.05, 0.05), vec2(0.95, 0.05));
                float right = 1.0 - lineSegment(gl_PointCoord, vec2(0.95, 0.05), vec2(0.95, 0.95));
                float bottom = 1.0 - lineSegment(gl_PointCoord, vec2(0.95, 0.95), vec2(0.05, 0.95));
                float left = 1.0 - lineSegment(gl_PointCoord, vec2(0.05, 0.95), vec2(0.05, 0.05));
                float shaper = top + right + bottom + left;
                gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
                if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;

            }

            if (vClassification == 7.0){
            // draw the star5 glyph
            	float side = 1.0;  // fragment has side length = 1.0
                float r = side/2.0;
                vec2 p = vec2(r);  // center of the fragment
                float eps = 0.1;   // will be adjusting the tips of the star to avoid clipping

                // Points start at the bottom left tip of the star and move counter clockwise
                vec2 A = vec2(p.x + 0.618027443 * r, p.y + (1.0 - eps) * r); //
                vec2 B = vec2(p.x + 0.5 * r, p.y + 0.27637816 * r);
                vec2 C = vec2(p.x + (1.0 - eps) * r, p.y - 0.236080209 * r);
                vec2 D = vec2(p.x + 0.309026865 * r, p.y - 0.341634306 * r);
                vec2 E = vec2(p.x + 0.0 * r, p.y - (1.0 - eps) * r);
                vec2 F = vec2(p.x - 0.309026865 * r, p.y - 0.341634306 * r);
                vec2 G = vec2(p.x - (1.0 - eps) * r, p.y - 0.236080209 * r);
                vec2 H = vec2(p.x - 0.5 * r, p.y + 0.27637816 * r);
                vec2 I = vec2(p.x - 0.618027443 * r, p.y + (1.0 - eps) * r);
                vec2 J = vec2(p.x, p.y + 0.658351875 * r);
                vec2 K = vec2(p.x, p.y + 0.658351875 * r);

                // Draw now the star
                float line_1  = 1.0 - lineSegment(gl_PointCoord, A, B);
                float line_2  = 1.0 - lineSegment(gl_PointCoord, B, C);
                float line_3  = 1.0 - lineSegment(gl_PointCoord, C, D);
                float line_4  = 1.0 - lineSegment(gl_PointCoord, D, E);
                float line_5  = 1.0 - lineSegment(gl_PointCoord, E, F);
                float line_6  = 1.0 - lineSegment(gl_PointCoord, F, G);
                float line_7  = 1.0 - lineSegment(gl_PointCoord, G, H);
                float line_8  = 1.0 - lineSegment(gl_PointCoord, H, I);
                float line_9  = 1.0 - lineSegment(gl_PointCoord, I, J);
                float line_10 = 1.0 - lineSegment(gl_PointCoord, J, K);
                float line_11 = 1.0 - lineSegment(gl_PointCoord, K, A);

                float shaper = line_1 + line_2 + line_3 + line_4 + line_5 + line_6 + line_7 + line_8 + line_9 + line_10 + line_11;
                gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
                if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
            }

            if (vClassification == 8.0){
            // draw the star6 glyph
                float side = 1.0;  // fragment has side length = 1.0
                float r = side/2.0;
                vec2 p = vec2(r);  // center of the fragment
                float eps = 0.05;   // will be adjusting the tips of the star to avoid clipping

                // Points start at the bottom left tip of the star and move clockwise
                vec2 A = vec2(p.x + 0.50 * r, p.y + 0.87 * r);
                vec2 B = vec2(p.x, p.y + 0.50 * r);
                vec2 C = vec2(p.x - 0.50 * r, p.y + 0.87 * r);
                vec2 D = vec2(p.x - 0.43 * r, p.y + 0.25 * r);
                vec2 E = vec2(p.x - r, p.y);
                vec2 F = vec2(p.x - 0.43 * r, p.y - 0.25 * r);
                vec2 G = vec2(p.x - 0.50 * r, p.y - 0.87 * r);
                vec2 H = vec2(p.x, p.y - 0.50 * r);
                vec2 I = vec2(p.x + 0.50 * r, p.y - 0.87 * r);
                vec2 J = vec2(p.x + 0.43 * r, p.y - 0.25 * r);
                vec2 K = vec2(p.x + r, p.y);
                vec2 L = vec2(p.x + 0.43 * r, p.y + 0.25 * r);

                // Draw now the star
                float line_1  = 1.0 - lineSegment(gl_PointCoord, A, B);
                float line_2  = 1.0 - lineSegment(gl_PointCoord, B, C);
                float line_3  = 1.0 - lineSegment(gl_PointCoord, C, D);
                float line_4  = 1.0 - lineSegment(gl_PointCoord, D, E);
                float line_5  = 1.0 - lineSegment(gl_PointCoord, E, F);
                float line_6  = 1.0 - lineSegment(gl_PointCoord, F, G);
                float line_7  = 1.0 - lineSegment(gl_PointCoord, G, H);
                float line_8  = 1.0 - lineSegment(gl_PointCoord, H, I);
                float line_9  = 1.0 - lineSegment(gl_PointCoord, I, J);
                float line_10 = 1.0 - lineSegment(gl_PointCoord, J, K);
                float line_11 = 1.0 - lineSegment(gl_PointCoord, K, L);
                float line_12 = 1.0 - lineSegment(gl_PointCoord, L, A);


                float shaper = line_1 + line_2 + line_3 + line_4 + line_5 + line_6 + line_7 + line_8 + line_9 + line_10 + line_11 + line_12;
                gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
                if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
            }

            if (vClassification == 9.0){
            // draw the traingleDown glyph
                float line_1 = 1.0 - lineSegment(gl_PointCoord, vec2(0.05), vec2(0.5, 0.95));
                float line_2 = 1.0 - lineSegment(gl_PointCoord, vec2(0.5, 0.95), vec2(0.95, 0.05));
                float line_3 = 1.0 - lineSegment(gl_PointCoord, vec2(0.95, 0.05), vec2(0.05, 0.05));
                float shaper = line_1 + line_2 + line_3;
                gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
                if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
            }

            if (vClassification == 10.0){
            // draw the traingleLeft glyph
            	vec2 uv = (gl_PointCoord - vec2(0.5)) * rot(PI * 0.5); // rotate
                uv = uv + vec2(0.5);
                float line_1 = 1.0 - lineSegment(uv, vec2(0.05), vec2(0.5, 0.95));
                float line_2 = 1.0 - lineSegment(uv, vec2(0.5, 0.95), vec2(0.95, 0.05));
                float line_3 = 1.0 - lineSegment(uv, vec2(0.95, 0.05), vec2(0.05, 0.05));
                float shaper = line_1 + line_2 + line_3;
                gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
                if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
            }

            if (vClassification == 11.0){
            // draw the traingleRight glyph
                vec2 uv = (gl_PointCoord - vec2(0.5)) * rot(1.5 * PI); // rotate
                uv = uv + vec2(0.5);
                float line_1 = 1.0 - lineSegment(uv, vec2(0.05), vec2(0.5, 0.95));
                float line_2 = 1.0 - lineSegment(uv, vec2(0.5, 0.95), vec2(0.95, 0.05));
                float line_3 = 1.0 - lineSegment(uv, vec2(0.95, 0.05), vec2(0.05, 0.05));
                float shaper = line_1 + line_2 + line_3;
                gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
                if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
            }

            if (vClassification == 12.0){
            // draw the traingleUp glyph
            	vec2 uv = (gl_PointCoord - vec2(0.5)) * rot(PI); // rotate
                uv = uv + vec2(0.5);
                float line_1 = 1.0 - lineSegment(uv, vec2(0.05), vec2(0.5, 0.95));
                float line_2 = 1.0 - lineSegment(uv, vec2(0.5, 0.95), vec2(0.95, 0.05));
                float line_3 = 1.0 - lineSegment(uv, vec2(0.95, 0.05), vec2(0.05, 0.05));
                float shaper = line_1 + line_2 + line_3;
                gl_FragColor = vec4(vec3(shaper) * color, gl_FragColor.a);
                if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
            }

        }
	#endif

	#if defined paraboloid_point_shape
		float wi = 0.0 - ( u*u + v*v);
		vec4 pos = vec4(vViewPosition, 1.0);
		pos.z += wi * vRadius;
		float linearDepth = -pos.z;
		pos = projectionMatrix * pos;
		pos = pos / pos.w;
		float expDepth = pos.z;
		depth = (pos.z + 1.0) / 2.0;
		gl_FragDepthEXT = depth;
		
		#if defined(color_type_depth)
			color.r = linearDepth;
			color.g = expDepth;
		#endif
		
		#if defined(use_edl)
			gl_FragColor.a = log2(linearDepth);
		#endif
		
	#else
		#if defined(use_edl)
			gl_FragColor.a = vLogDepth;
		#endif
	#endif

	#if defined(weighted_splats)
		float distance = 2.0 * length(gl_PointCoord.xy - 0.5);
		float weight = max(0.0, 1.0 - distance);
		weight = pow(weight, 1.5);

		gl_FragColor.a = weight;
		gl_FragColor.xyz = gl_FragColor.xyz * weight;
	#endif

	#if defined(cross_point_shape) 
		float diag_1 = 1.0 - lineSegment(gl_PointCoord, vec2(0.05, 0.05), vec2(0.95, 0.95));
		float diag_2 = 1.0 - lineSegment(gl_PointCoord, vec2(0.05, 0.95), vec2(0.95, 0.05));
		float shaper = diag_1 + diag_2;
		gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
		if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
	#endif

	#if defined(diamond_point_shape) 
		float side_1 = 1.0 - lineSegment(gl_PointCoord, vec2(0.5, 0.05), vec2(0.9, 0.5));
		float side_2 = 1.0 - lineSegment(gl_PointCoord, vec2(0.9, 0.5), vec2(0.5, 0.95));
		float side_3 = 1.0 - lineSegment(gl_PointCoord, vec2(0.5, 0.95), vec2(0.1, 0.5));
		float side_4 = 1.0 - lineSegment(gl_PointCoord, vec2(0.1, 0.5), vec2(0.5, 0.05));

		float shaper = side_1 + side_2 + side_3 + side_4;
		gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
		if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
	#endif

	#if defined(plus_point_shape) 
		float vertical = 1.0 - lineSegment(gl_PointCoord, vec2(0.5, 0.05), vec2(0.5, 0.95));
		float horizontal = 1.0 - lineSegment(gl_PointCoord, vec2(0.05, 0.5), vec2(0.95, 0.5));
		float shaper = vertical + horizontal;
		gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
		if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
	#endif

	#if defined(my_square_point_shape) 
		float top = 1.0 - lineSegment(gl_PointCoord, vec2(0.05, 0.05), vec2(0.95, 0.05));
		float right = 1.0 - lineSegment(gl_PointCoord, vec2(0.95, 0.05), vec2(0.95, 0.95));
		float bottom = 1.0 - lineSegment(gl_PointCoord, vec2(0.95, 0.95), vec2(0.05, 0.95));
		float left = 1.0 - lineSegment(gl_PointCoord, vec2(0.05, 0.95), vec2(0.05, 0.05));
		float shaper = top + right + bottom + left;
		gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
		if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
	#endif
	
	#if defined(star_5_point_shape) 
	    float side = 1.0;  // fragment has side length = 1.0
		float r = side/2.0;
		vec2 p = vec2(r);  // center of the fragment
		float eps = 0.1;   // will be adjusting the tips of the star to avoid clipping
		
		// Points start at the bottom left tip of the star and move counter clockwise
		vec2 A = vec2(p.x + 0.618027443 * r, p.y + (1.0 - eps) * r); //
		vec2 B = vec2(p.x + 0.5 * r, p.y + 0.27637816 * r);
		vec2 C = vec2(p.x + (1.0 - eps) * r, p.y - 0.236080209 * r);
		vec2 D = vec2(p.x + 0.309026865 * r, p.y - 0.341634306 * r);
		vec2 E = vec2(p.x + 0.0 * r, p.y - (1.0 - eps) * r);
		vec2 F = vec2(p.x - 0.309026865 * r, p.y - 0.341634306 * r);
		vec2 G = vec2(p.x - (1.0 - eps) * r, p.y - 0.236080209 * r);
		vec2 H = vec2(p.x - 0.5 * r, p.y + 0.27637816 * r);
		vec2 I = vec2(p.x - 0.618027443 * r, p.y + (1.0 - eps) * r);
		vec2 J = vec2(p.x, p.y + 0.658351875 * r);
		vec2 K = vec2(p.x, p.y + 0.658351875 * r);
		
		// Draw now the star
		float line_1  = 1.0 - lineSegment(gl_PointCoord, A, B);
		float line_2  = 1.0 - lineSegment(gl_PointCoord, B, C);
		float line_3  = 1.0 - lineSegment(gl_PointCoord, C, D);
		float line_4  = 1.0 - lineSegment(gl_PointCoord, D, E);
		float line_5  = 1.0 - lineSegment(gl_PointCoord, E, F);
		float line_6  = 1.0 - lineSegment(gl_PointCoord, F, G);
		float line_7  = 1.0 - lineSegment(gl_PointCoord, G, H);
		float line_8  = 1.0 - lineSegment(gl_PointCoord, H, I);
		float line_9  = 1.0 - lineSegment(gl_PointCoord, I, J);
		float line_10 = 1.0 - lineSegment(gl_PointCoord, J, K);
		float line_11 = 1.0 - lineSegment(gl_PointCoord, K, A);
		
		float shaper = line_1 + line_2 + line_3 + line_4 + line_5 + line_6 + line_7 + line_8 + line_9 + line_10 + line_11;
		gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
		if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
	#endif

	#if defined(star_6_point_shape) 
		float side = 1.0;  // fragment has side length = 1.0
		float r = side/2.0;
		vec2 p = vec2(r);  // center of the fragment
		float eps = 0.05;   // will be adjusting the tips of the star to avoid clipping
		
		// Points start at the bottom left tip of the star and move clockwise
		vec2 A = vec2(p.x + 0.50 * r, p.y + 0.87 * r);
		vec2 B = vec2(p.x, p.y + 0.50 * r);
		vec2 C = vec2(p.x - 0.50 * r, p.y + 0.87 * r);
		vec2 D = vec2(p.x - 0.43 * r, p.y + 0.25 * r);
		vec2 E = vec2(p.x - r, p.y);
		vec2 F = vec2(p.x - 0.43 * r, p.y - 0.25 * r);
		vec2 G = vec2(p.x - 0.50 * r, p.y - 0.87 * r);
		vec2 H = vec2(p.x, p.y - 0.50 * r);
		vec2 I = vec2(p.x + 0.50 * r, p.y - 0.87 * r);
		vec2 J = vec2(p.x + 0.43 * r, p.y - 0.25 * r);
		vec2 K = vec2(p.x + r, p.y);
		vec2 L = vec2(p.x + 0.43 * r, p.y + 0.25 * r);
		
		// Draw now the star
		float line_1  = 1.0 - lineSegment(gl_PointCoord, A, B);
		float line_2  = 1.0 - lineSegment(gl_PointCoord, B, C);
		float line_3  = 1.0 - lineSegment(gl_PointCoord, C, D);
		float line_4  = 1.0 - lineSegment(gl_PointCoord, D, E);
		float line_5  = 1.0 - lineSegment(gl_PointCoord, E, F);
		float line_6  = 1.0 - lineSegment(gl_PointCoord, F, G);
		float line_7  = 1.0 - lineSegment(gl_PointCoord, G, H);
		float line_8  = 1.0 - lineSegment(gl_PointCoord, H, I);
		float line_9  = 1.0 - lineSegment(gl_PointCoord, I, J);
		float line_10 = 1.0 - lineSegment(gl_PointCoord, J, K);
		float line_11 = 1.0 - lineSegment(gl_PointCoord, K, L);
		float line_12 = 1.0 - lineSegment(gl_PointCoord, L, A);
		
		
		float shaper = line_1 + line_2 + line_3 + line_4 + line_5 + line_6 + line_7 + line_8 + line_9 + line_10 + line_11 + line_12;
		gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
		if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
	#endif

	#if defined(triangle_down_point_shape) 
		float line_1 = 1.0 - lineSegment(gl_PointCoord, vec2(0.05), vec2(0.5, 0.95));
		float line_2 = 1.0 - lineSegment(gl_PointCoord, vec2(0.5, 0.95), vec2(0.95, 0.05));
		float line_3 = 1.0 - lineSegment(gl_PointCoord, vec2(0.95, 0.05), vec2(0.05, 0.05));
		float shaper = line_1 + line_2 + line_3;
		gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
		if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
	#endif

	#if defined(triangle_left_point_shape) 
		vec2 uv = (gl_PointCoord - vec2(0.5)) * rot(PI * 0.5); // rotate
		uv = uv + vec2(0.5);
		float line_1 = 1.0 - lineSegment(uv, vec2(0.05), vec2(0.5, 0.95));
		float line_2 = 1.0 - lineSegment(uv, vec2(0.5, 0.95), vec2(0.95, 0.05));
		float line_3 = 1.0 - lineSegment(uv, vec2(0.95, 0.05), vec2(0.05, 0.05));
		float shaper = line_1 + line_2 + line_3;
		gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
		if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
	#endif

	#if defined(triangle_right_point_shape) 
		vec2 uv = (gl_PointCoord - vec2(0.5)) * rot(1.5 * PI); // rotate
		uv = uv + vec2(0.5);
		float line_1 = 1.0 - lineSegment(uv, vec2(0.05), vec2(0.5, 0.95));
		float line_2 = 1.0 - lineSegment(uv, vec2(0.5, 0.95), vec2(0.95, 0.05));
		float line_3 = 1.0 - lineSegment(uv, vec2(0.95, 0.05), vec2(0.05, 0.05));
		float shaper = line_1 + line_2 + line_3;
		gl_FragColor = vec4(vec3(shaper) * vColor, gl_FragColor.a);
		if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
	#endif

	#if defined(triangle_up_point_shape)
		vec2 uv = (gl_PointCoord - vec2(0.5)) * rot(PI); // rotate
		uv = uv + vec2(0.5);
		float line_1 = 1.0 - lineSegment(uv, vec2(0.05), vec2(0.5, 0.95));
		float line_2 = 1.0 - lineSegment(uv, vec2(0.5, 0.95), vec2(0.95, 0.05));
		float line_3 = 1.0 - lineSegment(uv, vec2(0.95, 0.05), vec2(0.05, 0.05));
		float shaper = line_1 + line_2 + line_3;
		gl_FragColor = vec4(vec3(shaper) * color, gl_FragColor.a);
		if (gl_FragColor.r + gl_FragColor.g + gl_FragColor.b == 0.0) discard;
	#endif
	//gl_FragColor = vec4(0.0, 0.7, 0.0, 1.0);
	
}


