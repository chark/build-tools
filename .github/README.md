# Scriptable Scenes

[![Unity 2022.3+](https://img.shields.io/badge/unity-2022.3%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![Actions Status](https://github.com/chark/build-tools/workflows/CI/badge.svg)](https://github.com/chark/build-tools/actions)

 Editor utilities for quickly automating builds.

<p align="center">
  <img src="screenshot.png"/>
</p>

:warning: **Warning, this is a preview package, expect breaking changes between releases!**

## Features

- Quickly create build configurations via Scriptable Objects
- Run multiple builds in sequence
- Archive support

## Installation

This package can be installed via the Package Manager by [Installing from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html):

```text
https://github.com/chark/build-tools.git#upm
```

Alternatively, manually install by adding the following entry to `Packages/manifest.json`:
```json
{
  "com.chark.scriptable-scenes": "https://github.com/chark/build-tools.git#upm"
}
```

If you'd like to install a specific release, replace `upm` suffix with version number, e.g., `v0.0.1`. You can find all releases [here](https://github.com/chark/build-tools/releases).

## Links

- [Documentation](../Packages/com.chark.build-tools/Documentation~/README.md)
- [Contributing](CONTRIBUTING.md)
- [Changelog](../Packages/com.chark.build-tools/CHANGELOG.md)
- [License](../Packages/com.chark.build-tools/LICENSE.md)
