# Verification

## Completed without Unity licensing

| Check | Result | Scope |
| --- | --- | --- |
| C# syntax parser | Passed: 43 asset source files, zero parse errors | Syntax and class/file name consistency, including editor source |
| Runtime C# type check with API stubs | Passed: zero warnings or errors | Runtime signatures and internal references against local Unity, VRChat and Udon stubs; not an UdonSharp compile |
| TOTP vectors | Passed: six RFC 6238 SHA-1 vectors plus Base32 and member login checks | Actual `TOTPAuthManager.cs` built with stubs |
| Package source | VRChat Base and Worlds 3.10.4 archives matched official VPM SHA-256 metadata | Dependency integrity for this checkout |

## Pending Unity license

The installed Unity 2022.3.22f1 editor rejected batch import because no valid editor license was present. Consequently there is no successful Unity import, UdonSharp compile, generated `.asset`/`.prefab`/demo scene, `.unitypackage`, ClientSim run, or multi-client VRChat test to report. The earlier import log is at `unity-import.log` in this checkout and is ignored by Git.

After license activation, run `tools/Build-Package.ps1` from PowerShell. It restores the pinned VRChat SDK packages, imports the project, invokes `BaseBuildPipeline.Run`, compiles UdonSharp, generates assets, validates static references, and exports `Releases/BH2VSQ_BASE_Core.unitypackage`. Inspect `unity-build.log` and the Unity Console on failure. Then open the demo scene, run ClientSim, and run VRChat Build & Test with at least two clients to exercise ownership and synchronization paths.

## Known runtime limits to test

The shared synchronized player registry and area arrays can lose simultaneous updates. Teleport requests use one shared slot. These choices need a multi-client stress test. Udon TOTP secrets and authority cannot protect real secrets or paid access; see `Assets/BH2VSQ_BASE/Documentation/SECURITY.md`.
