#include <iostream>
#include <cstdlib>
#include <vector>

using namespace std;

int main(int argc, char **argv, char **env)
{
    vector<string> v;

    string str = getenv("PWD");

    cout << str;
    int r = 120, g = 30, b = 30;
    int previndex = 0;
    while (true)
    {
        int index = str.find("\\", previndex);
        string s = str.substr(previndex, index-previndex);
        cout << ((char)27) << "[48;2;" << r << ';' << g << ';' << b << 'm';
        cout << s << "";
        cout << ((char)27) << "[0m";
        previndex = index + 1;
        r += 40;
        g += 40;
        b += 40;
        if(index<0)
            break;
    }

    // cout << ((char)27) << "[1;41m";
    // cout << getenv("PWD") << ">";
    // cout << ((char)27) << "[0m";

    // for (int i = 0; env[i];i++)
    // {
    //     printf("%s\n", env[i]);
    // }

    return 0;
}