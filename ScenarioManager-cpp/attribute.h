#ifndef ATTRIBUTE_H
#define ATTRIBUTE_H

#include <string>

enum AttributeType {
    INTEGER,
    FLOAT,
    STRING
};

struct Attribute {
    AttributeType attributeType;
    std::string attributeValue;
};

#endif // ATTRIBUTE_H
