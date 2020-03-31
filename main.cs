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
            Car c2 = cars[i+1];
            c.Fill(sb);
            escapeBGColor(sb, c2.bgr, c2.bgg, c2.bgb);
            escapeFGColor(sb, c.bgr, c.bgg, c.bgb);
            sb.Append(SEPARATOR);
        }
        return sb.ToString();
    }

    public List<Car> cars = new List<Car>();
    public String SEPARATOR = "";

    public Prompt()
    {
        SEPARATOR = "";
        cars.Add(new Car(255, 255, 255, 0, 0, 0, "Mindborn"));
        cars.Add(new Car(50, 50, 255, 255, 255, 255, ""));
        BuildCondaCar();
        BuildDirCars();
        BuildGitCar();
        cars.Add(new Car(255, 255, 255, 0, 0, 0, ""));
    }

    public void BuildDirCars()
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

        parts[0] = "Drive " + parts[0];
        foreach (string part in parts)
        {
            Car car = new Car();
            car.text = part;

            car.bgr = r;
            car.bgg = g;
            car.bgb = b;
            double y = 0.38 * r + 0.51 * g + 0.11 * b;
            System.Console.WriteLine(y);
            if (y < 180)
            {
                car.fgr = car.fgg = car.fgb = 255;
            }
            else
            {
                car.fgr = car.fgg = car.fgb = 0;
            }
            r = (int)(r * 1.3);
            g = (int)(g * 1.3);
            b = (int)(b * 1.3);

            cars.Add(car);
        }
    }

    public void BuildCondaCar()
    {
        String condaenv = Environment.GetEnvironmentVariable("CONDA_PROMPT_MODIFIER");
        if (condaenv != null && !condaenv.Equals("(base)"))
        {
            cars.Add(new Car(255, 255, 255, 50, 150, 50, condaenv));
        }
    }

    public void BuildGitCar()
    {
        try
        {
            DirectoryInfo git = new DirectoryInfo(".git");
            if (git.Exists)
            {
                String text = File.ReadAllText(".git\\HEAD").Trim();
                String branch = text.Substring(text.LastIndexOf('/') + 1);
                cars.Add(new Car(50, 50, 200, 255, 255, 255, " " + branch));
                // var proc = new Process
                // {
                //     StartInfo = new ProcessStartInfo
                //     {
                //         FileName = "git.exe",
                //         Arguments = "status --porcelain=v1",
                //         UseShellExecute = false,
                //         RedirectStandardOutput = true,
                //         CreateNoWindow = true
                //     }
                // };
                // proc.Start();

                // while (!proc.StandardOutput.EndOfStream)
                // {
                //     string line = proc.StandardOutput.ReadLine();
                //     // do something with line
                // }
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
        System.Console.WriteLine(new Prompt().ToString());
    }

}