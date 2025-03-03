using System;
using System.IO;

namespace SwaggerPathsVisualizer;

class Program
{
    static void Main(string[] args)
    {
        string? swaggerJsonPath;
        string? outputPath;

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

        try
        {
            VisualizeSwaggerJson(swaggerJsonPath, outputPath);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex}");
        }
    }

    public static void VisualizeSwaggerJson(string jsonFilePath, string outputFilePath, string diagramDirection = MermaidDiagramDirection.LeftRight)
    {
        try
        {
            string json = File.ReadAllText(jsonFilePath);
            string mermaidCode = SwaggerMermaidGenerator.GenerateMermaidFromSwagger(json, diagramDirection);
            File.WriteAllText(outputFilePath, mermaidCode);
            Console.WriteLine($"Mermaid diagram generated successfully at: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex}");
        }
    }
}
