#ifndef DEMO_H
#define DEMO_H

#include <string>

namespace tenstorrent {

class Demo {
public:
    Demo();
    std::string getMessage() const;
    int add(int a, int b) const;
};

} // namespace tenstorrent

#endif // DEMO_H
