# C++ Guidelines

This document provides guidelines for C++ projects in the Tenstorrent demo monorepo.

## Standards

- **C++ Version**: C++17 (minimum)
- **Build System**: CMake 3.20+
- **Package Manager**: vcpkg
- **Compiler**: GCC 9+, Clang 10+, MSVC 2019+

## Code Style

- **Formatter**: clang-format with Google style
- **Naming Conventions**:
  - Classes: PascalCase
  - Functions: camelCase
  - Variables: snake_case
  - Constants: UPPER_SNAKE_CASE
  - Namespaces: lowercase

## Compiler Warnings

All projects must compile with the following warnings enabled:
```cmake
-Wall -Wextra -Wpedantic -Werror
```

## Project Structure

```
project/
├── CMakeLists.txt
├── vcpkg.json
├── src/
│   ├── CMakeLists.txt
│   ├── *.cpp
│   └── *.h
├── tests/
│   ├── CMakeLists.txt
│   └── test_*.cpp
└── .clang-format
```

## Testing

- **Framework**: Catch2 v3
- **Coverage**: Minimum 80% line coverage
- **Test Naming**: test_<component>_<scenario>

## Dependencies

Manage dependencies using vcpkg.json manifest:
```json
{
  "name": "project-name",
  "version": "1.0.0",
  "dependencies": ["fmt", "spdlog"]
}
```

## CMake Best Practices

1. Use target-based approach
2. Set C++ standard via `CMAKE_CXX_STANDARD`
3. Use `find_package` for external dependencies
4. Enable testing with `enable_testing()`
5. Use `target_link_libraries` with visibility specifiers

## Documentation

- Use Doxygen-style comments for public APIs
- Document all public functions, classes, and methods
- Include examples in documentation

## Example

```cpp
/// @brief Adds two integers
/// @param a First integer
/// @param b Second integer
/// @return Sum of a and b
int add(int a, int b) {
    return a + b;
}
```
