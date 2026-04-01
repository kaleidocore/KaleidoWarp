shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0) = 0.0;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform sampler2D image : hint_default_transparent;
uniform int image_fit = 0; // 0=None, 1=Contain, 2=Cover, 3=Stretch
uniform float speed : hint_range(1.0, 100.0) = 50.0;

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
    float p = progress * 2.0;

    vec2 resolution = 1.0 / SCREEN_PIXEL_SIZE;
    float block_size = floor(pow(p, 1.4) * speed) * 2.0 + 0.01;

    vec2 uv = SCREEN_UV;
    uv *= resolution / block_size;
    uv = floor(uv);
    uv /= resolution / block_size;
    uv += block_size * 0.002;

    vec4 pixelated = texture(SCREEN_TEXTURE, uv);

    float mix_factor = smoothstep(1.0, 2.0, p);
	vec4 t = sample_image(UV, SCREEN_PIXEL_SIZE);
    vec4 target = mix(COLOR, t, t.a);

    COLOR = mix(pixelated, target, mix_factor);
}