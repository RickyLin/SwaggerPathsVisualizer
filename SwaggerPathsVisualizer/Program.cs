using System;
using System.IO;

namespace SwaggerPathsVisualizer;

class Program
{
    static void Main(string[] args)
    {
        string? swaggerJsonPath;
        string? outputPath;
        bool includeHttpMethods = true;

        if (args.Length >= 2)
        {
            swaggerJsonPath = args[0];
            outputPath = args[1];
        }
        else
        {
            Console.WriteLine("Enter the path to the Swagger JSON file:");
            swaggerJsonPath = Console.ReadLine();

            Console.WriteLine("Enter the path to save the Mermaid diagram:");
            outputPath = Console.ReadLine();
        }

        if (string.IsNullOrEmpty(swaggerJsonPath))
        {
            Console.Error.WriteLine("JSON file path cannot be null or empty.");
            return;
        }

        if (string.IsNullOrEmpty(outputPath))
        {
            Console.Error.WriteLine("Output file path cannot be null or empty. The extension name of mermaid file is .mmd usually.");
            return;
        }

        if (args.Length >= 3)
        {
            if (bool.TryParse(args[2], out bool argIncludeHttpMethods))
            {
                includeHttpMethods = argIncludeHttpMethods;
            }
            else if (args[2].Equals("y", StringComparison.OrdinalIgnoreCase) || args[2].Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                includeHttpMethods = true;
            }
            else
            {
                Console.Error.WriteLine("Invalid value for includeHttpMethods. Use 'true', 'false', 'y', or 'yes'.");
                return;
            }
        }

        try
        {
            VisualizeSwaggerJson(swaggerJsonPath, outputPath, includeHttpMethods: includeHttpMethods);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex}");
        }
    }

    public static void VisualizeSwaggerJson(string jsonFilePath, string outputFilePath, string diagramDirection = MermaidDiagramDirection.LeftRight, bool includeHttpMethods = true)
    {
        try
        {
            string json = File.ReadAllText(jsonFilePath);
            string mermaidCode = SwaggerMermaidGenerator.GenerateMermaidFromSwagger(json, diagramDirection, includeHttpMethods);
            File.WriteAllText(outputFilePath, mermaidCode);
            Console.WriteLine($"Mermaid diagram generated successfully at: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex}");
        }
    }
}
