#include "../src/demo.h"
#include <catch2/catch_test_macros.hpp>

TEST_CASE("Demo getMessage", "[demo]") {
    tenstorrent::Demo demo;
    REQUIRE(demo.getMessage() == "Hello from Tenstorrent C++ Demo!");
}

TEST_CASE("Demo add function", "[demo]") {
    tenstorrent::Demo demo;
    
    SECTION("positive numbers") {
        REQUIRE(demo.add(2, 3) == 5);
        REQUIRE(demo.add(10, 20) == 30);
    }
    
    SECTION("negative numbers") {
        REQUIRE(demo.add(-5, -3) == -8);
        REQUIRE(demo.add(-10, 5) == -5);
    }
    
    SECTION("zero") {
        REQUIRE(demo.add(0, 0) == 0);
        REQUIRE(demo.add(42, 0) == 42);
    }
}
