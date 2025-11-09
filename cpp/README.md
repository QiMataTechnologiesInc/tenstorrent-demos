# C++ Demo

Tenstorrent C++ demo using CMake and vcpkg.

## Prerequisites

- CMake 3.20+
- C++17 compatible compiler
- vcpkg (optional but recommended)

## Building

```bash
# With vcpkg
cmake -B build -S . -DCMAKE_TOOLCHAIN_FILE=$VCPKG_ROOT/scripts/buildsystems/vcpkg.cmake
cmake --build build

# Without vcpkg (requires dependencies installed separately)
cmake -B build -S .
cmake --build build
```

## Testing

```bash
cd build
ctest --output-on-failure
```

## Project Structure

- `src/` - Source files
- `tests/` - Unit tests
- `CMakeLists.txt` - Build configuration
- `vcpkg.json` - Dependency manifest
