#ifndef STRINGUTIL_H
#define STRINGUTIL_H

#include <vector>
#include <string>

using namespace std;

class StringUtil
{
public:
    static void tokenize(const string& str, vector<string>& tokens, const string& delimiters = " ");
};

#endif // STRINGUTIL_H
