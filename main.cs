using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class Car
{
    int fgr, fgg, fgb;
    int bgr, bgg, bgg;
    String text;
}

class Prompt
{

    List<Car> cars = new List<Car>();


    public override ToString()
    {

    }


    public Prompt()
    {
        FileInfo file = new FileInfo(".");
        string filename = file.FullName;
        // Console.WriteLine(((char)27) + "[31m" + filename + ((char)27) + "[0m");
        int seed = 0;
        // foreach(char c in filename.ToCharArray()) seed += c;
        string[] parts = filename.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
        for (int x = 0; x < parts.Length && x < 2; x++)
        {
            seed += parts[x].GetHashCode();
        }
        Random random = new Random(seed);
        int r = random.Next(1, 6) * 25;
        int g = random.Next(1, 6) * 25;
        int b = random.Next(1, 6) * 25;


        StringBuilder sb = new StringBuilder(200);

        // sb.Append(escapeFGColor(255, 255, 255));
        int i = 0;
        parts[0] = "Drive " + parts[0];
        foreach (string part in parts)
        {
            if (i < 5 && (r + g + b) < 400)
            {
                sb.Append(escapeFGColor(255, 255, 255)).Append(escapeBGColor(r, g, b)).Append(' ').Append(part).Append(' ');
            }
            else
            {
                sb.Append(escapeFGColor(0, 0, 0)).Append(escapeBGColor(r, g, b)).Append(' ').Append(part).Append(' ');
            }

            // sb.Append('\\');

            sb.Append(escapeFGColor(r, g, b));

            r = (int)(r * 1.3);
            g = (int)(g * 1.3);
            b = (int)(b * 1.3);

            // r = (int)(r * 1.5 * ((i % 2 == 1) ? 0.5 : 1));
            // g = (int)(g * 1.5 * ((i % 2 == 1) ? 0.5 : 1));
            // b = (int)(b * 1.5 * ((i % 2 == 1) ? 0.5 : 1));
            // r = random.Next(0, 10)*25;
            // g = random.Next(0, 10)*25;
            // b = random.Next(0, 10)*25;

            if (i == parts.Length - 1)
            {
                sb.Append(escapeBGColor(0, 0, 0));
            }
            else
            {
                sb.Append(escapeBGColor(r, g, b));
            }
            // sb.Append('');
            // sb.Append('');
            sb.Append('');
            // sb.Append('');
            // sb.Append('');
            // sb.Append('');
            // sb.Append('');

            i++;
        }
        sb.Append(resetColors());
        sb.Append(' ');
        System.Console.WriteLine(sb);
    }
    public static string escapeFGColor(int r1, int g1, int b1)
    {
        return escapeColor("38;2;", r1, g1, b1);
    }

    public static string escapeBGColor(int r1, int g1, int b1)
    {
        return escapeColor("48;2;", r1, g1, b1);
    }

    public static string escapeColor(string prefix, int r1, int g1, int b1)
    {
        StringBuilder sb = new StringBuilder(30);
        sb.Append((char)27).Append('[');
        sb.Append(prefix).Append(r1).Append(';').Append(g1).Append(';').Append(b1);
        sb.Append('m');
        return sb.ToString();
    }

    public static string resetColors()
    {
        return ((char)27) + "[0m";
    }

    public static void Main(string[] args)
    {
        System.Console.WriteLine(new Prompt().ToString());
    }

}