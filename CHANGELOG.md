# Changelog

## [2.0.0-alpha.1] - 2023-03-14

### Added
- option to bind factory methods for instance resolution
- children are now remembered within the container
- container has a name

### Changed
- bibliotheca added as dependency
- internally bindings are IResolver now

### Removed
- type relationship binding (might come back if proved necessary)

## [1.1.0] - 2023-03-05

### Changed
- all variants of BindWithInterfaces are now extensions methods

## [1.0.3] - 2023-03-04

### Added
- TypeInjectionInfo now also works with non-public members

## [1.0.2] - 2023-02-26

### Fixed
- Fixed InjectionIndexer instance always being created instead of being cached

## [1.0.1] - 2023-02-25

### Added
- Creating a instance based on existing constructors, including constructor and instance injection

## [1.0.0] - 2023-02-24

### Added
- Inject attribute to mark field and properties as injectable
- Type injection indexer
- Dependency injection container and interface
