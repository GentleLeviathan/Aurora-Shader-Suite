# Aurora Shader Suite
Requires Unity 2019.1.x or greater.
Includes output template for Substance Painter.

A suite of tools and a shader for a personal project. Takes advantage of a texture swizzle ('Aurora') to represent Metallic, Occlusion, Emission, and Curvature.

### Unique features include:
  * 4 channel color masking via an RGBA color control texture (3 channels have color properties)
  * Per color mask channel pattern support (useful for camouflage or fabric)
  * Color 'depth' for increasing texture contrast via slider
  * Detail diffuse and normal with strength slider and tiling/offset vector
  * Decal textures applied via mesh UV2 for text/image clarity

### Includes a set of 'Helper' tools, including:
  * OpenGL to DirectX normal format (and vice-versa)
  * Tool to pack traditional Metallic_Smoothness, Occlusion, Emission, and Curvature textures into the 'Aurora' swizzle format.
  * Material baking (for applying shader color zones, patterns, and depth to a new diffuse texture to be used elsewhere.)

### Additional helpers to enable the use of assets inspired by Microsoft's 'Halo' games.
Supports armor sets and textures from the following 'Halo' games:

* Halo Reach
* Halo: Combat Evolved
* Halo 2
* Halo 3: ODST
* Halo 3
* Halo 4 (Storm)
* Halo 5: Forge

## Substance Painter - Workflow Changes
  * Color Control (CC) textures are generated from User0, User1, and User2 channels
  * Height channel will be exported as normal information into the normal map
  * Be sure to bake curvature maps


This suite is licensed under the MIT License.

A quick disclaimer:
I do not condone reverse-engineering or pirating software, assets, or intellectual property.
This suite is only designed for working with assets inspired by Microsoft's 'Halo' universe.
You will not find any intellectual property in this repository.
