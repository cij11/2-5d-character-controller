Shader "Unlit Color Only" {
 
Properties {
    _Color ("Color", Color) = (1,1,1)

}

Category {
	Tags { "Queue"="Geometry" }
	Lighting Off
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
	}
SubShader {
    Color [_Color]
    Pass {}
}
}
 
}