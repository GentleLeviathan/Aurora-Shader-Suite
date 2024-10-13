# Aurora Shader Suite
Requires Unity 2019.1.x or greater.
Includes output template for Substance Painter.

A suite of tools and a shader for a personal project. Takes advantage of a texture swizzle ('Aurora') to represent Metallic, Roughness, Illumination (single channel emission), and Occlusion.
AR3.0+ has made significant changes to the Aurora shader. The new A3 swizzle *must* be utilized.

A helper to upgrade existing Aurora A2 textures to the A3 swizzle will be included at a later time.
For now, you must export your textures from your texture program again, or utilize the included Aurora Packer helper to generate a new A3 texture.

New lighting model is energy conserving* (Options allow modification to more closely match AR2)
(There are clearly marked configurable options that violate energy-conservation but may appear more 'grounded' in some situations)

### Unique features include:
  * 4 channel color masking via an RGBA color control texture (3 channels have color properties)
  * Per color mask channel pattern support (useful for camouflage or fabric)
  * Color 'depth' for increasing texture contrast via slider
  * ~~Detail diffuse and normal with strength slider and tiling/offset vector~~ (Deprecated in AR3.0+)
  * Decal textures applied via mesh UV2 for text/image clarity
  * 'Rave' section with 4 channel HDR Color emission + a scrolling mask texture. ([AudioLink](https://github.com/llealloo/audiolink) supported)
  
### UDIM=5 Support
  * Supports up to 5 unique texture sets, each with their own Diffuse, Normal, CC, Aurora, and Pattern textures!
  * Texture sets are tied to keywords, keeping the shader performant
  * Each texture set gets it's own 1.0 range on the U-dimension. (see image below)
  
![Aurora_UDIM_Layout](https://user-images.githubusercontent.com/17507902/236114265-5fb332d8-f964-4fad-92e7-0313eee6a15c.png)

### Includes a set of 'Helper' tools, including:
  * OpenGL to DirectX normal format (and vice-versa)
  * Tool to pack traditional Metallic, Smoothness, Emission, and Occlusion textures into the 'Aurora' swizzle format.
  * Material baking (for applying shader color zones, patterns, and depth to a new diffuse texture to be used elsewhere.) (Not yet(?) supported in AR3.0+)

### Additional helpers to enable the use of assets inspired by Microsoft's 'Halo' games. (Via legacy AR2 interface)
Supports armor sets and textures from the following 'Halo' games:

* Halo Reach
* Halo: Combat Evolved
* Halo 2
* Halo 3: ODST
* Halo 3
* Halo 4 (Storm)
* Halo 5: Forge

## Substance Painter - Workflow Changes
  * Color Control (CC) textures are generated from User0, User1, User2, and User3 channels
  * Height channel will be exported as normal information into the normal map
  
  
## Rave CC + Mask
Rave CC is a 4 channel texture with an HDR color property for each channel.
Rave Mask is a 4 channel texture which is multiplied by the Rave CC color result to modify it.
The Rave section has per channel x+y speed properties which control the UV scrolling applied to the Rave Mask texture channels.
If [AudioLink](https://github.com/llealloo/vrc-udon-audio-link) is installed in the project, the option to enable it will appear at the bottom of the rave section, along with other options such as chronotensity scrolling.


This suite is licensed under the MIT License.

A quick disclaimer:
I do not condone reverse-engineering or pirating software, assets, or intellectual property.
This suite is only designed for working with assets inspired by Microsoft's 'Halo' universe.
You will not find any intellectual property in this repository.


<img src="https://user-images.githubusercontent.com/17507902/236127779-849a8a72-af06-45eb-9dc4-f0cf55b848a1.png" width="30%"></img>
<img src="https://user-images.githubusercontent.com/17507902/236127792-f60cdbaa-2411-4ca6-b6d2-59ab8ba83649.png" width="30%"></img>
