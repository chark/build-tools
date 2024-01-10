# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project
adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [v0.0.2](https://github.com/chark/build-tools/compare/v0.0.1...v0.0.2) - 2023-XX-XX

### Added

- Build Step support with "Player" and "Archive" steps. This reworks the old monolithic architecture where all build configuration options were all cobbled together in one script.
- Progress bar shown when files are being archived.

### Changed

- Menu item order to use `150` instead of `-1000`. This way `CHARK` won't dominate existing entries.
- Documentation and samples to utilize new Build Step architecture.

## [v0.0.1](https://github.com/chark/build-tools/compare/v0.0.1) - 2023-10-04

Initial preview version.

### Added

- `BuildConfiguration` Scriptable Object type which can be used to store build configs.
- `BuildToolsManagerWindow` which can be used to trigger builds.
- Documentation.
