#include "demo.h"
#include <spdlog/spdlog.h>

namespace tenstorrent {

Demo::Demo() {
    spdlog::info("Demo object created");
}

std::string Demo::getMessage() const {
    return "Hello from Tenstorrent C++ Demo!";
}

int Demo::add(int a, int b) const {
    spdlog::debug("Adding {} + {}", a, b);
    return a + b;
}

} // namespace tenstorrent
