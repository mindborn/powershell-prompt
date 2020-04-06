using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;


public class Prompt
{
    public class Car
    {
        public int fgr, fgg, fgb;
        public int bgr, bgg, bgb;
        public String text;

        public Car() { }
        public Car(int fgr, int fgg, int fgb, int bgr, int bgg, int bgb, String text)
        {
            this.fgr = fgr;
            this.fgg = fgg;
            this.fgb = fgb;
            this.bgr = bgr;
            this.bgg = bgg;
            this.bgb = bgb;
            this.text = text;
        }

        public void Fill(StringBuilder sb)
        {
            escapeBGColor(sb, bgr, bgg, bgb);
            escapeFGColor(sb, fgr, fgg, fgb);
            sb.Append(' ');
            sb.Append(text);
            sb.Append(' ');
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        // cars.ForEach((c) => c.Fill(sb));
        int n = cars.Count - 1;
        for (int i = 0; i < n; i++)
        {
            Car c = cars[i];
            Car c2 = cars[i + 1];
            c.Fill(sb);

            escapeBGColor(sb, c2.bgr, c2.bgg, c2.bgb);
            escapeFGColor(sb, c.bgr, c.bgg, c.bgb);
            sb.Append(SEPARATOR);
            // sb.Append(i%2==1 ? SEPARATOR : SEPARATOR2);
        }
        resetColors(sb);
        return sb.ToString();
    }

    public List<Car> cars = new List<Car>();
    public String SEPARATOR = "";
    // public String SEPARATOR2 = "";
    string Path = null;
    string Provider = null;
    bool IsDirPath = false;
    String config = null;

    public Prompt(string location, string provider)
    {
        Path = location;
        Provider = provider;
        SEPARATOR = "";
        // SEPARATOR = "";
        // SEPARATOR2 = "";
        // SEPARATOR = "";
        // SEPARATOR = "";
        // SEPARATOR = "";
        // SEPARATOR = "";
        // SEPARATOR = "";
        // SEPARATOR = "";
        try
        {
            FileInfo file = new FileInfo(location);
            Path = file.FullName;
            config = LoadConfig(Path);
            IsDirPath = true;
        }

        catch (Exception) { }

        cars.Add(new Car(255, 255, 255, 60, 60, 60, "Mindborn"));
        // cars.Add(new Car(30, 170, 255, 255, 255, 255, ""));
        cars.Add(new Car(30, 170, 255, 255, 255, 255, ""));
        BuildCondaCar();
        BuildDirCars();
        if (IsDirPath) BuildGitCar();
        cars.Add(new Car(255, 255, 255, 0, 0, 0, ""));
    }

    public String LoadConfig(string path)
    {
        try
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            if (!directory.Exists) return null;
            while (directory != null)
            {
                FileInfo f = new FileInfo(directory.FullName + System.IO.Path.DirectorySeparatorChar + ".CONFIG");
                if (f.Exists)
                {
                    return File.ReadAllText(f.FullName);
                }
                directory = directory.Parent;
            }
        }
        catch (Exception)
        {

        }
        return null;

    }


    public void BuildDirCars()
    {
        if (Provider != null)
        {
            // Random random = new Random(Provider.GetHashCode());
            // int rp, gp, bp;
            // HSBToRGB(random.Next(360), 100, 30, out rp, out gp, out bp);
            // cars.Add(new Car(255, 255, 255, rp, gp, bp, Provider));
            Path = Provider + System.IO.Path.DirectorySeparatorChar + Path;
        }
        string[] parts = Path.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        int r, g, b;
        float h, s, l;
        if (config != null)
        {
            String[] cparts = config.Split(new char[] { ',' });
            r = int.Parse(cparts[0]);
            g = int.Parse(cparts[1]);
            b = int.Parse(cparts[2]);
            RGBToHSB(r, g, b, out h, out s, out l);
        }
        else
        {
            int seed = 6677;
            // foreach(char c in filename.ToCharArray()) seed += c;
            for (int x = 0; x < parts.Length && x < 3; x++)
            {
                // seed ^= parts[x].GetHashCode();
                string p = parts[x];
                foreach (char c in p)
                    seed *= c;
            }

            Random random = new Random(seed);
            // r = random.Next(1, 6) * 25;
            // g = random.Next(1, 6) * 25;
            // b = random.Next(1, 6) * 25;
            h = random.Next(360);
            s = 50; //random.Next(50);
            l = random.Next(16, 17);
            HSBToRGB(h, s, l, out r, out g, out b);
        }

        // parts[0] = "Drive " + parts[0];
        foreach (string part in parts)
        {
            r = (r < 0) ? 0 : (r > 255) ? 255 : r;
            g = (g < 0) ? 0 : (g > 255) ? 255 : g;
            b = (b < 0) ? 0 : (b > 255) ? 255 : b;

            Car car = new Car();
            car.text = part;

            car.bgr = r;
            car.bgg = g;
            car.bgb = b;
            double y = 0.31 * r + 0.58 * g + 0.11 * b;
            // System.Console.Write(y + " ");
            if (y < 128)
            {
                car.fgr = car.fgg = car.fgb = 255;
            }
            else
            {
                car.fgr = car.fgg = car.fgb = 0;
            }

            // car.fgr = (int)(128+y);
            // car.fgg = (int)(128+y);
            // car.fgb = (int)(128+y);


            // r = (int)(r * 1.4);
            // g = (int)(g * 1.4);
            // b = (int)(b * 1.4);
            //l *= 1.30f;
            l=(float)(l+(100-l)/5.5);
            // System.Console.Write(l+" ");
            // h += 40;
            HSBToRGB(h, s, l, out r, out g, out b);

            cars.Add(car);
        }
    }

    public void BuildCondaCar()
    {
        string condaenv = Environment.GetEnvironmentVariable("CONDA_PROMPT_MODIFIER");
        string ce;
        if (condaenv != null && !(ce = condaenv.Trim()).Equals("(base)"))
        {
            if (ce[0] == '(') ce = ce.Substring(1, ce.Length - 2);
            cars.Add(new Car(255, 255, 255, 50, 150, 50, " " + ce));
        }
    }

    public DirectoryInfo FindGitFolder()
    {
        DirectoryInfo directory = new DirectoryInfo(".");
        while (directory != null)
        {
            DirectoryInfo d = new DirectoryInfo(directory.FullName + System.IO.Path.DirectorySeparatorChar + ".git");
            if (d.Exists)
            {
                return d;
            }
            directory = directory.Parent;
        }
        return null;
    }

    public void BuildGitCar()
    {
        try
        {
            DirectoryInfo git = FindGitFolder(); //new DirectoryInfo(".git");
            if (git != null)
            {
                String text = File.ReadAllText(git.FullName + System.IO.Path.DirectorySeparatorChar + "HEAD").Trim();
                String branch = text.Substring(text.LastIndexOf('/') + 1);
                cars.Add(new Car(0, 0, 0, 255, 255, 255, " " + branch));
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "git.exe",
                        Arguments = "status --porcelain=v1",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();

                int a = 0;
                int m = 0;
                int d = 0;
                int unm = 0;
                int unt = 0;
                int staged = 0;

                while (!proc.StandardOutput.EndOfStream)
                {
                    string line = proc.StandardOutput.ReadLine();
                    // do something with line
                    char c = line[1];
                    if (c == 'A') a++;
                    if (c == 'M') m++;
                    if (c == 'D') d++;
                    if (c == 'U') unm++;
                    if (c == '?') unt++;
                    if (c == ' ') staged++;
                }
                int total = a + m + d + unm + unt;
                // System.Console.WriteLine(a+","+m+","+d+","+u);
                if (total == 0 && staged == 0)
                {
                    cars.Add(new Car(255, 255, 255, 0, 220, 0, ""));
                }
                if (total > 0)
                {
                    cars.Add(new Car(255, 255, 255, 250, 0, 0, " " + total));
                }
                if (staged > 0)
                {
                    cars.Add(new Car(255, 255, 255, 250, 150, 0, " " + staged));
                }
            }
        }
        catch (Exception)
        {

        }
    }

    public static void escapeFGColor(StringBuilder sb, int r1, int g1, int b1)
    {
        escapeColor(sb, "38;2;", r1, g1, b1);
    }

    public static void escapeBGColor(StringBuilder sb, int r1, int g1, int b1)
    {
        escapeColor(sb, "48;2;", r1, g1, b1);
    }

    public static void escapeColor(StringBuilder sb, string prefix, int r1, int g1, int b1)
    {
        sb.Append((char)27).Append('[');
        sb.Append(prefix).Append(r1).Append(';').Append(g1).Append(';').Append(b1);
        sb.Append('m');
    }

    public static void resetColors(StringBuilder sb)
    {
        sb.Append((char)27).Append("[0m");
    }

    public static void RGBToHSB(float r, float g, float b, out float h, out float s, out float l)
    {
        r = r / 255f;
        g = g / 255f;
        b = b / 255f;

        float max = (r > g && r > b) ? r : (g > b) ? g : b;
        float min = (r < g && r < b) ? r : (g < b) ? g : b;

        l = (max + min) / 2.0f;

        if (max == min)
        {
            h = s = 0.0f;
        }
        else
        {
            float d = max - min;
            s = (l > 0.5f) ? d / (2.0f - max - min) : d / (max + min);

            if (r > g && r > b)
                h = (g - b) / d + (g < b ? 6.0f : 0.0f);

            else if (g > b)
                h = (b - r) / d + 2.0f;

            else
                h = (r - g) / d + 4.0f;

            h /= 6.0f;
        }

        // float[] hsl = { h, s, l };
        // return hsl; 
        h = (float)Math.Round(h * 360);
        s = (float)Math.Round(s * 100);
        l = (float)Math.Round(l * 100);

        // return [h, s, l];
    }

    public static void HSBToRGB(float hue, float saturation, float brightness, out int red__, out int green, out int blue_)
    {
        hue = hue / 360f;
        saturation = saturation / 100f;
        brightness = brightness / 100f;
        // float red, green, blue;

        if (saturation == 0)
        {
            red__ = green = blue_ = (int)Math.Round(brightness * 255); // achromatic
        }
        else
        {
            var q = brightness < 0.5 ? brightness * (1 + saturation) : brightness + saturation - brightness * saturation;
            var p = 2 * brightness - q;
            red__ = (int)Math.Round(255 * hue2rgb(p, q, hue + 1f / 3));
            green = (int)Math.Round(255 * hue2rgb(p, q, hue));
            blue_ = (int)Math.Round(255 * hue2rgb(p, q, hue - 1f / 3));
        }

        // return Color.FromArgb((int)Math.Round(red * 255), (int)Math.Round(green * 255), (int)Math.Round(blue * 255));
    }

    public static float hue2rgb(float p, float q, float t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1f / 6) return p + (q - p) * 6 * t;
        if (t < 1f / 2) return q;
        if (t < 2f / 3) return p + (q - p) * (2f / 3 - t) * 6;
        return p;
    }

    public static void Main(string[] args)
    {
        string location = ".", provider = null;
        if (args.Length >= 1) location = args[0];
        if (args.Length >= 2) provider = args[1];

        System.Console.WriteLine(new Prompt(location, provider).ToString());
    }
}