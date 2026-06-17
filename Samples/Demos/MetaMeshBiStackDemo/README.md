# MetaMesh BI Stack Demo

This demo is a small checked-in MetaMesh workspace that maps a representative BI stack to stable handles.

From the `meta-bi` repo root, after building `../meta/MetaMesh.sln`, run:

```cmd
..\meta\MetaMesh\Cli\bin\Debug\net8.0\meta-mesh.exe show --mesh Samples\Demos\MetaMeshBiStackDemo\BIStackDemo.MetaMesh
..\meta\MetaMesh\Cli\bin\Debug\net8.0\meta-mesh.exe check --mesh Samples\Demos\MetaMeshBiStackDemo\BIStackDemo.MetaMesh
..\meta\MetaMesh\Cli\bin\Debug\net8.0\meta-mesh.exe impact --mesh Samples\Demos\MetaMeshBiStackDemo\BIStackDemo.MetaMesh --workspace transform
```

The point is not to make these folders the only possible BI stack. The point is that users can operate by logical handles instead of remembering every physical workspace folder.
