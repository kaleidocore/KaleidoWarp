shader_type canvas_item;

uniform float progress : hint_range(0, 1) = 0.0;
uniform sampler2D image : hint_default_transparent;
uniform int image_fit = 0; // 0=None, 1=Contain, 2=Cover, 3=Stretch
uniform sampler2D dissolve_texture : hint_default_transparent;
uniform bool flip_x = false;
uniform bool flip_y = false;
uniform bool invert = false;
uniform float feather : hint_range(0.0, 0.25) = 0.1f;

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

void fragment() {
    vec2 uv = UV;
    if (flip_x) uv.x = 1.0 - uv.x;
    if (flip_y) uv.y = 1.0 - uv.y;
    
    float sample = texture(dissolve_texture, uv).r;
	if(invert) sample = 1.0 - sample;
	float td = mix(-feather, 1.0 + feather, progress);
	float alpha = smoothstep(td - feather, td + feather, sample);
   
    vec4 ti = sample_image(UV, SCREEN_PIXEL_SIZE);
    vec4 base = mix(COLOR, ti, ti.a);
    COLOR = vec4(base.rgb, base.a * alpha);
}