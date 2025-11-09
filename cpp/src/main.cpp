#include "demo.h"
#include <fmt/core.h>
#include <iostream>

int main() {
    tenstorrent::Demo demo;
    
    fmt::print("{}\n", demo.getMessage());
    
    int result = demo.add(42, 8);
    fmt::print("42 + 8 = {}\n", result);
    
    return 0;
}
