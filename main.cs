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
        }
        resetColors(sb);
        return sb.ToString();
    }

    public List<Car> cars = new List<Car>();
    public String SEPARATOR = "";
    string Path = null;
    bool IsDirPath = false;
    String config = null;

    public Prompt(string location)
    {
        Path = location;
        // SEPARATOR = "";
        SEPARATOR = "";
        try
        {
            FileInfo file = new FileInfo(location);
            Path = file.FullName;
            config = LoadConfig(Path);
            IsDirPath = true;
        }

        catch (Exception) { }

        cars.Add(new Car(255, 255, 255, 60, 60, 60, "Mindborn"));
        // cars.Add(new Car(30, 170, 255, 255, 255, 255, ""));
        cars.Add(new Car(30, 170, 255, 255, 255, 255, ""));
        BuildCondaCar();
        BuildDirCars(Path);
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


    public void BuildDirCars(string location)
    {
        string[] parts = Path.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        int r, g, b;
        // Console.WriteLine(((char)27) + "[31m" + filename + ((char)27) + "[0m");
        if (config != null)
        {
            String[] cparts = config.Split(new char[] { ',' });
            r = int.Parse(cparts[0]);
            g = int.Parse(cparts[1]);
            b = int.Parse(cparts[2]);
        }
        else
        {
            int seed = 0;
            // foreach(char c in filename.ToCharArray()) seed += c;
            for (int x = 0; x < parts.Length && x < 2; x++)
            {
                seed += parts[x].GetHashCode();
            }

            Random random = new Random(seed);
            r = random.Next(1, 6) * 25;
            g = random.Next(1, 6) * 25;
            b = random.Next(1, 6) * 25;
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


            r = (int)(r * 1.3);
            g = (int)(g * 1.3);
            b = (int)(b * 1.3);

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
            cars.Add(new Car(255, 255, 255, 50, 150, 50, ce));
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

    public static void Main(string[] args)
    {
        string location = args.Length == 0 ? "." : args[0];
        System.Console.WriteLine(new Prompt(location).ToString());
    }

}