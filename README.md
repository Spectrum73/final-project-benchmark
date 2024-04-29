# Benchmarking Application for Final Year Project LOD meshes
This application allows for performance testing of large quantities of LOD objects.

## Creating an LOD prefab
Before anything can be spawned a prefab must first be made.
- Add an empty GameObject to the scene and give it the LOD Group Component
- Add any number of meshes as separate children of the empty
- In the LOD Group properties, adjust each level appropriatly and set the (Mesh Renderer) fields of each level to the appropriate mesh.
- To convert the gameobject into a prefab, drag the object from the hierarchy to the project folder in `Assets/Prefabs`
## Spawning Objects
At the top of the screen go to `Tools/SpawnMenu` to access the spawn menu.
- Assign a noise texture to the field or press the "Generate Noise" button instead.
- Adjust the density as needed, setting it to 1 will ensure trees are placed wherever possible.
- Assign the "Object Prefab" to the prefab in `Assets/Prefabs` you have made.
- Adjust euler angle variance as needed. NOTE: This isn't required for the benchmark but makes the scene more accurate to a potential dense forest. A good default set for these values is: X:`10` Y:`180` Z:`10`
- The size of the area to generate the trees can be adjusted by pressing on "Terrain" in the hierarchy then adjusting the Terrain width and length in the terrain component's settings. By default this is `100x100`
- Press "Place Objects"
## Benchmarking
It is recommended to build the scene with the pre-placed prefab for the best rendering results.
Before doing so however, the following window needs to be opened:
- The Profiler which can be accessed at `Window/Analysis/Profiler`

Whilst this window is open, any build run should automatically link to it and display the appropriate data selected, for render times "GPU Usage" and "Rendering" can be used. NOTE: The profiler will still work if running in "Play Mode" in the editor, but performance may be reduced.

If additional analysis needs to be performed on the data, it can be saved as a binary file with the floppy disk symbol to the top right of the Profiler window. From this the AnalyzeToCsvWindow can be used via `Tools/UTJ/ProfileReader/AnalyzeToCsvWindow`. In this window the binary file can be selected and the chosen profiler modules can be converted into CSV files.
The ProfileReader is also available from: https://github.com/unity3d-jp/ProfilerReader
