add_library(demo_lib
    demo.cpp
)

target_include_directories(demo_lib PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}
)

# Link dependencies
find_package(fmt CONFIG REQUIRED)
find_package(spdlog CONFIG REQUIRED)

target_link_libraries(demo_lib PUBLIC
    fmt::fmt
    spdlog::spdlog
)

# Main executable
add_executable(demo_app
    main.cpp
)

target_link_libraries(demo_app PRIVATE
    demo_lib
)
