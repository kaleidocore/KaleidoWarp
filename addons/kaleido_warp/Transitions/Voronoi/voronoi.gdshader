shader_type canvas_item;

uniform float progress : hint_range(0, 1) = 0.0;
uniform float angle : hint_range(0, 6.2832) = 0.0;
uniform sampler2D image : hint_default_transparent;
uniform int image_fit = 3; // 0=None, 1=Contain, 2=Cover, 3=Stretch

vec4 sample_image(vec2 uv, vec2 screen_pixel_size) {
    vec2 screen_size = 1.0 / screen_pixel_size;
    vec2 image_size = vec2(textureSize(image, 0));

    if (image_fit == 0) { // None
        vec2 scale = image_size / screen_size;
        uv = (uv - 0.5) / scale + 0.5;
    } else if (image_fit == 1) { // Contain
        float scale = min(screen_size.x / image_size.x, screen_size.y / image_size.y);
        uv = (uv - 0.5) / (image_size * scale / screen_size) + 0.5;
    } else if (image_fit == 2) { // Cover
        float scale = max(screen_size.x / image_size.x, screen_size.y / image_size.y);
        uv = (uv - 0.5) / (image_size * scale / screen_size) + 0.5;
    }

    bool oob = image_fit != 3 && (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0);
    return oob ? vec4(0.0) : texture(image, uv);
}

vec2 random(vec2 uv) {
	return vec2(fract(sin(dot(uv.xy,
		vec2(12.9898,78.233))) * 43758.5453123));
}

float worley(vec2 uv, float columns, float rows) {
	vec2 index_uv = floor(vec2(uv.x * columns, uv.y * rows));
	vec2 fract_uv = fract(vec2(uv.x * columns, uv.y * rows));
	float minimum_dist = 1.0;
	for (int y = -1; y <= 1; y++) {
		for (int x = -1; x <= 1; x++) {
			vec2 neighbor = vec2(float(x), float(y));
			vec2 point = random(index_uv + neighbor);
			vec2 diff = neighbor + point - fract_uv;
			float dist = length(diff);
			minimum_dist = min(minimum_dist, dist);
		}
	}
	return minimum_dist;
}

void fragment() {
    float worley = worley(UV + vec2(-TIME * 0.1, 0), 16.0, 9.0);
    vec2 dir = vec2(cos(angle), sin(angle));
    float gradient = dot(UV - 0.5, dir) + 0.5;
    float alpha = (worley * 0.5 + gradient * 1.5) / 2.0;
    float threshold = mix(-.1, .9, progress) + .1;
    float result = step(alpha, threshold);

	vec4 t = sample_image(UV, SCREEN_PIXEL_SIZE);
    vec4 base = mix(COLOR, t, t.a);
    COLOR = vec4(base.rgb, base.a * result);
}
